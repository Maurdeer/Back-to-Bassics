using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Playables;
using Debug = UnityEngine.Debug;

// AT: FMOD bs portion mainly implemented off of official timeline callback example,
//     as FMOD's internal update loop is 20ms (and seems unadjustable despite docs
//     saying the contrary) other timing methods should be preferred.
//     Other interesting references:
//     - https://drive.google.com/file/d/1r8ROjgsMh-mwKqGTZT7IWMCsJcs3GuU9/view (https://qa.fmod.com/t/perfect-beat-tracking-in-unity/18788/6)

public class Conductor : Singleton<Conductor>
{
    public bool IsPlaying => ctx != null;
    
    public float Beat => (ctx?.currentState.ElapsedBeat ?? throw new ArgumentException("don't poll beat without music playing :)"));

    public float spb => (ctx?.lastMarker.msPerBeat ?? throw new ArgumentException("don't poll spb without music playing :)")) / 1000;

    /// <summary>
    /// A json file containing mapping from each event path to an array of tempo markers, containing the following info:
    /// - id: a string id of the marker itself, can be compared to during marker callbacks
    /// - positionMs: the location of the marker on the FMOD timeline, in miliseconds
    /// - tempoBpm: tempo corresponding to the segment between the current marker and the next
    /// - timeSignatureNumerator
    /// - timeSignatureDenominator
    /// </summary>
    public TextAsset EventMarkerMap;
    
    private ConductorContext _ctx = null;

    private GCHandle ctxHandle;
    private ConductorContext ctx
    {
        get => _ctx;
        set
        {
            if (_ctx != null)
            {
                Debug.Log("=========== STOPPED CONDUCTING ===========");
                _ctx.Stop();
                ctxHandle.Free();
            }
            
            _ctx = value;
            
            if (_ctx != null)
            {
                ctxHandle = GCHandle.Alloc(_ctx);
                _ctx.SetCallback(ctxHandle);
            }
        }
    }
    
    private bool isConducting => ctx != null;
    //
    // public event Action OnQuarterBeat;
    // public event Action OnHalfBeat;
    // public event Action OnFullBeat;
    public event Action OnFullBeat = delegate { };
    public event Action OnFirstBeat = delegate { };
    // public event Action OnLastBeat;

    public event Action<SerializedTempoMarker> OnTempoChange = delegate { };

    private Dictionary<string, SerializedTempoMarker[]> _parsedEvents;
    
    public void Awake()
    {
        InitializeSingleton();

        var everything = JsonUtility.FromJson<SerializedEventCollection>(EventMarkerMap.text);
        _parsedEvents = new();
        foreach (var e in everything.events)
        {
            _parsedEvents.Add(e.id, e.markers);
        }
    }

    private ChannelGroup masterChannelGroup;

    public void BeginConducting(EnemyBattlePawn pawn)
    {
        FMODUnity.RuntimeManager.CoreSystem.getMasterChannelGroup(out masterChannelGroup);
        
        if (isConducting) 
        {
            Debug.LogWarning("Conductor was issued to conduct when it already is conducting");
            return;
        }

        ctx = new ConductorContext(this, pawn)
        {
            pawn = pawn,
            fmodInstance = FMODUnity.RuntimeManager.CreateInstance(pawn.EnemyData.fmodEvent)
        };
        ctx.Start();
        // this.spb = Data.SPB;
        // OnFirstBeat?.Invoke();
        // StartCoroutine(Conduct());
    }
    
    public void StopConducting()
    {
        if (!isConducting)
        {
            Debug.LogWarning("Conductor was issued to stop conducting when it already is not conducting");
            return;
        }
        
        ctx = null;
    }
    public void ChangeMusicPhase(int phase)
    {
        ctx.fmodInstance.setParameterByName("Phase", phase);
    }

    // AT: Run this after the regular update so that all events are definitely triggered within the same interval,
    // instead of in-between regular updates
    public void LateUpdate()
    {
        if (ctx == null)
        {
            return;
        }
        
        if (ctx.fmodInstance.getPlaybackState(out var state) == RESULT.OK)
        {
            if (state is PLAYBACK_STATE.STOPPED or PLAYBACK_STATE.STOPPING)
            {
                StopConducting();
                ctx = null;
                return;
            }
        }
        
        ctx.Tick();
    }

    public struct BeatFraction
    {
        public int numerator;
        public int denominator;
        // public float Eval() => ((float)numerator) / denominator;
        public float Eval() => ((float)numerator) / denominator;
        
        public static readonly BeatFraction fullQuadruple = new BeatFraction() { numerator = 4, denominator = 1 };
        public static readonly BeatFraction fullDouble = new BeatFraction() { numerator = 2, denominator = 1 };
        public static readonly BeatFraction full = new BeatFraction(){ numerator = 1, denominator = 1 };
        public static readonly BeatFraction half = new BeatFraction() { numerator = 1, denominator = 2 };
        public static readonly BeatFraction quarter = new BeatFraction() { numerator = 1, denominator = 4 };
        public static readonly BeatFraction eighth = new BeatFraction() { numerator = 1, denominator = 8 };
        public static readonly BeatFraction sixteenth = new BeatFraction() { numerator = 1, denominator = 16 };
        
        public static implicit operator float(BeatFraction bf)
        {
            return bf.Eval();
        }
    }

    public float SnapToCurrentBeat(BeatFraction granularity)
    {
        return ctx.SnapToCurrent(granularity);
    }
    
    /// <returns>Starting time </returns>
    public void ScheduleActionAsap(float duration, float time, ConductorSchedulable schedulable, bool forceStart)
    {
        if (ctx is null)
        {
            throw new Exception("Trying to schedule beat callback without current playing music");
        }
        
        schedulable._parent = this;

        float startTime;
        if (time <= ctx.currentState.ElapsedBeat || forceStart)
        {
            startTime = ctx.currentState.ElapsedBeat;
        }
        else
        {
            startTime = time;
        }
        
        schedulable._state =
            new ConductorSchedulableState(startTime, duration, time);
        
        // this function might be called from animations, which is in turn called during scheduled queue loop
        // we don't want to modify the queue within the foreach loop, so instead we make a buffer outside
        // and commit the transaction at the end of the frame.
        ctx.transaction.Add((schedulable, schedulable._state._evaluatedEndBeat));

        if (time <= startTime)
        {
            schedulable._state._started = true;
            schedulable.OnStarted?.Invoke(schedulable._state, ctx.currentState);
        }
    }

    public class ConductorSchedulableState
    {
        public bool _started = false;
        public bool _aborted = false;
        
        /// <summary>
        /// The correct beat the action should have started, if not for player delays
        /// </summary>
        public float _scheduledStartBeat;
        /// <summary>
        /// The actual beat the schedulable was added to the system
        /// </summary>
        public float _actualStartBeat;
        /// <summary>
        /// "_scheduledDuration" beats after _snappedToStartBeat. the system guarantees the schedulable will be complete on this beat
        /// </summary>
        public float _evaluatedEndBeat;
        /// <summary>
        /// The number of beats passed in when this schedulable is scheduled
        /// </summary>
        public float _scheduledDuration;

        /// <summary>
        /// The actual duration the schedulable will remain in the system until complete is called (_evaluatedEndBeat - _actualStartBeat)
        /// </summary>
        public float _actualDuration => _evaluatedEndBeat - _actualStartBeat;
        /// <summary>
        /// Percentage (0.0 ~ 1.0) version of _actualDuration. May have errors due to floating point math! Please prefer other measures instead
        /// When there is no ending (duration is positive infinity), progress count is just set to current elapsed beat - starting beat
        /// </summary>
        public float _elapsedProgressCount;

        internal ConductorSchedulableState(float actualStartBeat, float duration, float snappedStart)
        {
            _actualStartBeat = actualStartBeat;
            _scheduledStartBeat = snappedStart;
            _scheduledDuration = duration;
            _evaluatedEndBeat = _scheduledStartBeat + _scheduledDuration;
            _elapsedProgressCount = 0.0f;
        }
    }

    public class ConductorSchedulable
    {
        internal ConductorSchedulableState _state;

        internal Conductor _parent;
        
        public Action<ConductorSchedulableState, ConductorContextState> OnUpdate;
        public Action<ConductorSchedulableState, ConductorContextState> OnStarted;
        public Action<ConductorSchedulableState, ConductorContextState> OnCompleted;
        public Action<ConductorSchedulableState> OnAborted;

        public ConductorSchedulable(
            Action<ConductorSchedulableState, ConductorContextState> onUpdate = null, 
            Action<ConductorSchedulableState, ConductorContextState> onStarted = null, 
            Action<ConductorSchedulableState, ConductorContextState> onCompleted = null, 
            Action<ConductorSchedulableState> onAborted = null)
        {
            OnUpdate = onUpdate;
            OnStarted = onStarted;
            OnCompleted = onCompleted;
            OnAborted = onAborted;
        }

        public void SelfAbort()
        {
            _state._aborted = true;
            OnAborted?.Invoke(_state);
        }
    }

    public class ConductorContextState
    {
        private float elapsedBeat;
        public float deltaBeat;

        public float ElapsedBeat
        {
            get => elapsedBeat;
            set
            {
                deltaBeat = value - elapsedBeat;
                elapsedBeat = value;
            }
        }
        public float bpm;
        public float spb;

        public override string ToString()
        {
            return $"[ctxState: beat {elapsedBeat}; bpm {bpm} <-> spb {spb}]";
        }
    }
    
    private class ConductorContext
    {
        public Conductor parent;
        public EnemyBattlePawn pawn;
        public EventInstance fmodInstance;
        public SerializedTempoMarker[] markers;
        public FMOD.Studio.TIMELINE_BEAT_PROPERTIES lastBeatProperties;
        public SerializedTempoMarker lastMarker;
        public SerializedTempoMarker nextMarker;
        public FMOD.Studio.EVENT_CALLBACK beatCallback;
        public int elapsedWholeBeats = 0;
        // public ulong lastBeatDsp = 0;
        public float elapsedLastTotal = 0.0f;
        public float elapsedSinceLastBeat = 0.0f;

        public ConductorContextState currentState;
        // public int samplerRate;

        internal PriorityQueue<ConductorSchedulable, float> scheduled = new();
        internal List<(ConductorSchedulable, float)> transaction = new();
        
        public ConductorContext(Conductor parent, EnemyBattlePawn pawn)
        {
            this.parent = parent;
            this.pawn = pawn;
            fmodInstance = FMODUnity.RuntimeManager.CreateInstance(pawn.EnemyData.fmodEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(fmodInstance, pawn.transform);
            
            markers = parent._parsedEvents[pawn.EnemyData.fmodEvent.Guid.ToString()];
            
            lastBeatProperties.beat = 0;
            lastBeatProperties.bar = 0;
            lastBeatProperties.position = 0;
            var (lastMarker, nextMarker) = FindPrevNextMarkers(0);
            lastBeatProperties.tempo = (float) lastMarker.tempoBpm;
            lastBeatProperties.timesignaturelower = lastMarker.timeSignatureDenominator;
            lastBeatProperties.timesignatureupper = lastMarker.timeSignatureNumerator;

            this.lastMarker = lastMarker;
            this.nextMarker = nextMarker;

            currentState = new ConductorContextState()
            {
                ElapsedBeat = 0,
                bpm = lastMarker.tempoBpm,
                spb = lastMarker.msPerBeat / 1E3f
            };

            // FMODUnity.RuntimeManager.CoreSystem.getSoftwareFormat(out var samplerRate, out var _, out var __);
            // this.samplerRate = samplerRate;
        }

        internal void Start()
        {
            fmodInstance.start();
            OnTempoChange();
        }

        internal void Tick()
        {
            // TODO: look into duplicate entries issue...
            currentState.ElapsedBeat = elapsedWholeBeats + ElapsedBeatSinceLastBeat();

            // dequeue schedulables until the next upcoming one is not actually completed
            while (scheduled.TryPeek(out var scheduledItem, out var finishTime) && finishTime < currentState.ElapsedBeat)
            {
                if (!scheduledItem._state._aborted)
                {
                    scheduledItem.OnCompleted?.Invoke(scheduledItem._state, currentState);
                }

                scheduled.Dequeue();
            }
            
            // iterate through schedulables in unsorted manner calling their update methods
            foreach (var (scheduledItem, finishTime) in scheduled.UnorderedItems)
            {
                if (scheduledItem._state._aborted)
                {
                    continue;
                }
                
                scheduledItem._state._elapsedProgressCount =
                    (currentState.ElapsedBeat - scheduledItem._state._actualStartBeat) / (scheduledItem._state._actualDuration);

                if (scheduledItem._state._actualStartBeat <= currentState.ElapsedBeat)
                {
                    if (!scheduledItem._state._started)
                    {
                        scheduledItem._state._started = true;
                        scheduledItem.OnStarted?.Invoke(scheduledItem._state, currentState);
                    }
                    
                    scheduledItem.OnUpdate?.Invoke(scheduledItem._state, currentState);
                }
            }
            
            // commit transaction (prevent queue from being modified mid-loop)
            scheduled.EnqueueRange(transaction);
            transaction.Clear();
        }

        internal float SnapToCurrent(BeatFraction granularity)
        {
            var elapsedScaled = currentState.ElapsedBeat / granularity;
            var snapUnitTime = Mathf.RoundToInt((float)elapsedScaled);
            var snapBeatTime = snapUnitTime * granularity;
            return snapBeatTime;
        }

        private float ElapsedBeatSinceLastBeat()
        {
            fmodInstance.getPitch(out var pitch);
            elapsedSinceLastBeat += Time.deltaTime * pitch;
            
            var beat = elapsedSinceLastBeat * 1000 / lastMarker.msPerBeat;
            
            if (beat >= 1)
            {
                return 1 - 1e-6f; // never cross beat boundary without an actual beat callback
            }
            else
            {
                return beat;
            }
        }

        internal void Stop()
        {
            fmodInstance.stop(STOP_MODE.IMMEDIATE);
            
            // iterate through schedulables in unsorted manner, calling their abort method
            foreach (var (scheduledItem, finishTime) in scheduled.UnorderedItems)
            {
                scheduledItem.OnAborted?.Invoke(scheduledItem._state);
            }
        }

        /// <summary>
        /// Note that for timeline positions before the first marker, both the tempo and time signature of prev and next
        /// will be set to the first marker, while the timeline position of prev will be set to 0
        /// ... for timeline positions after the last marker, both the tempo and time signature of prev and next will be
        /// set to the last marker, while the timeline position of next will be set to +inf
        /// ... for anything else, prev and next will be different markers, as long as there are >2 markers!
        /// </summary>
        /// <param name="timelinePositionMs"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="Exception"></exception>
        private (SerializedTempoMarker, SerializedTempoMarker) FindPrevNextMarkers(int timelinePositionMs)
        {
            // we don't really care about the case where no markers are present, that shouldn't happen anyway
            if (markers.Length == 0)
            {
                throw new IndexOutOfRangeException();
            }

            // base cases for first and last markers
            if (markers[0].positionMs > timelinePositionMs)
            {
                return (new SerializedTempoMarker{
                    id=null,
                    positionMs = 0,
                    tempoBpm = markers[0].tempoBpm,
                    timeSignatureNumerator = markers[0].timeSignatureNumerator,
                    timeSignatureDenominator = markers[0].timeSignatureDenominator
                }, markers[0]);
            }

            if (markers[^1].positionMs < timelinePositionMs)
            {
                return (markers[^1], new SerializedTempoMarker{
                    id = null,
                    positionMs = int.MaxValue,
                    tempoBpm = markers[^1].tempoBpm,
                    timeSignatureNumerator = markers[^1].timeSignatureNumerator,
                    timeSignatureDenominator = markers[^1].timeSignatureDenominator
                });
            }

            for (int i = 0; i < markers.Length-1; i++)
            {
                if (markers[i].positionMs >= timelinePositionMs)
                {
                    return (markers[i], markers[i+1]);
                }
            }
            
            return (markers[^1], new SerializedTempoMarker{
                id = null,
                positionMs = int.MaxValue,
                tempoBpm = markers[^1].tempoBpm,
                timeSignatureNumerator = markers[^1].timeSignatureNumerator,
                timeSignatureDenominator = markers[^1].timeSignatureDenominator
            });
        }

        public void SetCallback(GCHandle handle)
        {
            fmodInstance.setUserData(GCHandle.ToIntPtr(handle));
            beatCallback = new EVENT_CALLBACK(BeatEventCallback);
            fmodInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        }

        private void OnBeat()
        {
            parent.OnFullBeat?.Invoke();
        }

        private void OnTempoChange()
        {
            parent.OnTempoChange?.Invoke(lastMarker);
        }
        
        [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
        static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

            // Retrieve the user data
            IntPtr timelineInfoPtr;
            FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);
            if (result != FMOD.RESULT.OK)
            {
                Debug.LogError("Timeline Callback error: " + result);
            }
            else if (timelineInfoPtr != IntPtr.Zero)
            {
                // Get the object to store beat and marker details
                GCHandle ctxHandle = GCHandle.FromIntPtr(timelineInfoPtr);
                ConductorContext ctxObj = (ConductorContext)ctxHandle.Target;

                switch (type)
                {
                    case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    {
                        var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                        ctxObj.lastBeatProperties = parameter;

                        var (prevMarker, nextMarker) = ctxObj.FindPrevNextMarkers(parameter.position);
                        // Debug.Log($"{parameter.position}: prev={prevMarker.tempoBpm}bpm@{prevMarker.positionMs}ms; next={nextMarker.tempoBpm}bpm@{nextMarker.positionMs}ms");

                        var shouldInvokeOnTempoChange = ctxObj.lastMarker != prevMarker;
                        
                        ctxObj.lastMarker = prevMarker;
                        ctxObj.nextMarker = nextMarker;

                        if (shouldInvokeOnTempoChange)
                        {
                            ctxObj.currentState.bpm = prevMarker.tempoBpm;
                            ctxObj.currentState.spb = prevMarker.msPerBeat / 1E3f;
                            ctxObj.OnTempoChange();
                        }
                        ctxObj.elapsedWholeBeats += 1;
                        // ctxObj.parent.masterChannelGroup.getDSPClock(out var dspClock, out var parentDSP);
                        // ctxObj.lastBeatDsp = dspClock;
                        ctxObj.elapsedSinceLastBeat = 0.0f;
                        
                        try
                        {
                            ctxObj.OnBeat();
                        }
                        catch(Exception e)
                        {
                            ctxHandle.Free();
                            Debug.LogError($"Conductor Broke due to exception on beat: \"{e}\"");
                        }
                        break;
                    }
                    case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                    {
                        // Now the event has been destroyed, unpin the timeline memory so it can be garbage collected
                        ctxHandle.Free();
                        break;
                    }
                }
            }
            return FMOD.RESULT.OK;
        }
    }
    
    
    [Serializable]
    public struct SerializedTempoMarker: IEquatable<SerializedTempoMarker>
    {
        public string id;
        public float positionMs;
        public float tempoBpm;
        public float msPerBeat => 60e3f/tempoBpm;
        public int timeSignatureNumerator;
        public int timeSignatureDenominator;

        public static bool operator ==(SerializedTempoMarker obj1, SerializedTempoMarker obj2)
        {
            return obj1.positionMs == obj2.positionMs;
        }
        
        public static bool operator !=(SerializedTempoMarker obj1, SerializedTempoMarker obj2) => !(obj1 == obj2);

        public bool Equals(SerializedTempoMarker other)
        {
            return positionMs == other.positionMs;
        }

        public override bool Equals(object obj)
        {
            return obj is SerializedTempoMarker other && Equals(other);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }

    #region parsingHelperClasses
    /// <summary>
    /// Helper class purely for JSON parsing purposes...
    /// </summary>
    [System.Serializable]
    private struct SerializedEvent
    {
        public string id;
        public SerializedTempoMarker[] markers;
    }
    
    /// <summary>
    /// Helper class purely for JSON parsing purposes...
    /// </summary>
    [System.Serializable]
    private struct SerializedEventCollection
    {
        public SerializedEvent[] events;
    }
    #endregion
}

public static class ConductorExtensions
{
    public static Conductor.ConductorSchedulable ScheduleToBeat(this PlayableDirector director)
    {
        director.timeUpdateMode = DirectorUpdateMode.Manual;
        director.Play();
        var schedulable = new Conductor.ConductorSchedulable();
        schedulable.OnStarted = (state, ctxState) =>
        {
            director.time = 0;
            director.Evaluate();
        };
        schedulable.OnUpdate = (state, ctxState) =>
        {
            if (director.state != PlayState.Playing)
            {
                schedulable.SelfAbort();
                return;
            }
            director.time = ctxState.ElapsedBeat - state._scheduledStartBeat;
            if (director.time > 0)
            {
                director.Evaluate();
            }
        };
        schedulable.OnCompleted = (state, ctxState) =>
        {
            director.Stop();
        };
        var startTime = Conductor.Instance.SnapToCurrentBeat(Conductor.BeatFraction.full);
        Conductor.Instance.ScheduleActionAsap((float) director.duration, startTime, schedulable, forceStart: false);

        return schedulable;
    }
}
