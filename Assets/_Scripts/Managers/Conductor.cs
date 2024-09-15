using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Conductor : Singleton<Conductor>
{
    public float Beat { get; private set; }
    
    public float time => Beat * spb;
    public float spb {  get; private set; }

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

    /// <summary>
    /// Don't edit this! will be set automatically... currently public just for viewing in inspector...
    /// </summary>
    public float nextBeatMs = -1;
    
    private bool isConducting => ctx != null;
    //
    // public event Action OnQuarterBeat;
    // public event Action OnHalfBeat;
    // public event Action OnFullBeat;
    public event Action OnFullBeat;
    // public event Action OnFirstBeat;
    // public event Action OnLastBeat;

    public event Action<SerializedTempoMarker> OnTempoChange;

    protected static Dictionary<string, SerializedTempoMarker[]> parsedEvents;
    
    public void Awake()
    {
        InitializeSingleton();

        var everything = JsonUtility.FromJson<SerializedEventCollection>(EventMarkerMap.text);
        parsedEvents = new();
        foreach (var e in everything.events)
        {
            parsedEvents.Add(e.id, e.markers);
        }
    }

    public void BeginConducting(EnemyBattlePawn pawn)
    {
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

        Beat = 0;
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

    // private IEnumerator Conduct()
    // {
    //     float quarterTime = spb / 4f;
    //     OnFirstBeat?.Invoke();
    //     while (isConducting)
    //     {
    //         yield return new WaitForSeconds(quarterTime);
    //         Beat += 0.25f;
    //         OnQuarterBeat?.Invoke();
    //         
    //         yield return new WaitForSeconds(quarterTime);
    //         Beat += 0.25f;
    //         OnQuarterBeat?.Invoke();
    //         OnHalfBeat?.Invoke();
    //         
    //         yield return new WaitForSeconds(quarterTime);
    //         Beat += 0.25f;
    //         OnQuarterBeat?.Invoke();
    //         
    //         yield return new WaitForSeconds(quarterTime);
    //         Beat += 0.25f;
    //         OnQuarterBeat?.Invoke();
    //         OnHalfBeat?.Invoke();
    //         OnFullBeat?.Invoke();
    //         
    //     }
    //     OnLastBeat?.Invoke();
    // }

    public struct BeatFraction
    {
        public int numerator;
        public int denominator;
        public float Eval() => ((float)numerator) / denominator;
        
        public static readonly BeatFraction fullQuadruple = new BeatFraction() { numerator = 4, denominator = 1 };
        public static readonly BeatFraction fullDouble = new BeatFraction() { numerator = 2, denominator = 1 };
        public static readonly BeatFraction full = new BeatFraction(){ numerator = 1, denominator = 1 };
        public static readonly BeatFraction half = new BeatFraction() { numerator = 1, denominator = 2 };
        public static readonly BeatFraction quarter = new BeatFraction() { numerator = 1, denominator = 4 };
        public static readonly BeatFraction eighth = new BeatFraction() { numerator = 1, denominator = 8 };
        public static readonly BeatFraction sixteenth = new BeatFraction() { numerator = 1, denominator = 16 };
    }
    
    public void ScheduleActionAsap(
        BeatFraction scheduleGranularity, BeatFraction duration, ConductorSchedulable schedulable)
    {
        if (ctx is null)
        {
            throw new Exception("Trying to schedule beat callback without current playing music");
        }
        
        schedulable._parent = this;
        schedulable._state =
            new ConductorSchedulableState(ctx.elapsedTotal, duration.Eval(), ctx.SnapToCurrent(scheduleGranularity));
        ctx.scheduled.Enqueue(schedulable, schedulable._state._evaluatedEndBeat);
        
        schedulable.OnStarted(schedulable._state);
    }

    public class ConductorSchedulableState
    {
        public float _snappedToStartBeat;
        public float _actualStartBeat;
        public float _evaluatedEndBeat;
        public float _scheduledDuration;
        public float _actualDuration => _evaluatedEndBeat - _actualStartBeat;
        public float _elapsedProgressCount;
        public float _elapsedBeat;

        internal ConductorSchedulableState(float elapsedTotal, float duration, float snappedStart)
        {
            _actualStartBeat = elapsedTotal;
            _snappedToStartBeat = snappedStart;
            _scheduledDuration = duration;
            _evaluatedEndBeat = _snappedToStartBeat + _scheduledDuration;
            
            _elapsedBeat = 0.0f;
            _elapsedProgressCount = 0.0f;
        }
    }

    public class ConductorSchedulable
    {
        internal ConductorSchedulableState _state;

        internal Conductor _parent;
        
        public Action<ConductorSchedulableState> OnUpdate;
        public Action<ConductorSchedulableState> OnStarted;
        public Action<ConductorSchedulableState> OnCompleted;
        public Action<ConductorSchedulableState> OnAborted;

        public ConductorSchedulable(
            Action<ConductorSchedulableState> onUpdate, 
            Action<ConductorSchedulableState> onStarted, 
            Action<ConductorSchedulableState> onCompleted, 
            Action<ConductorSchedulableState> onAborted)
        {
            OnUpdate = onUpdate;
            OnStarted = onStarted;
            OnCompleted = onCompleted;
            OnAborted = onAborted;
        }

        public void SelfAbort()
        {
            // TODO: figure out how to implement this without iterating through the entire heap...
            throw new NotImplementedException();
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
        public int lastMarkerEncounteredAt = 0;
        public int elapsedWholeBeatsSinceMarker = 0;
        public float elapsedBeatSinceLastWholeBeat = 0.0f;
        public float elapsedTotal = 0.0f;

        internal PriorityQueue<ConductorSchedulable, float> scheduled = new();
        
        public ConductorContext(Conductor parent, EnemyBattlePawn pawn)
        {
            this.parent = parent;
            this.pawn = pawn;
            fmodInstance = FMODUnity.RuntimeManager.CreateInstance(pawn.EnemyData.fmodEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(fmodInstance, pawn.transform);
            
            markers = parsedEvents[pawn.EnemyData.fmodEvent.Guid.ToString()];
            
            lastBeatProperties.beat = 0;
            lastBeatProperties.bar = 0;
            lastBeatProperties.position = 0;
            var marker = FindPrevNextMarkers(0).Item1;
            lastBeatProperties.tempo = marker.tempoBpm;
            lastBeatProperties.timesignaturelower = marker.timeSignatureDenominator;
            lastBeatProperties.timesignatureupper = marker.timeSignatureNumerator;
        }

        internal void Start()
        {
            fmodInstance.start();
            OnTempoChange();
        }

        internal void Tick()
        {
            var elapsedSinceLastMarker = ElapsedFractionalSinceLastMarker();
            elapsedTotal = lastMarkerEncounteredAt + elapsedSinceLastMarker;

            // dequeue schedulables until the next upcoming one is not actually completed
            while (scheduled.TryPeek(out var scheduledItem, out var finishTime) && finishTime < elapsedTotal)
            {
                scheduledItem.OnCompleted(scheduledItem._state);
                
                scheduled.Dequeue();
            }

            // iterate through schedulables in unsorted manner calling their update methods
            foreach (var (scheduledItem, finishTime) in scheduled.UnorderedItems)
            {
                
                scheduledItem._state._elapsedProgressCount =
                    (elapsedTotal - scheduledItem._state._actualStartBeat) / (scheduledItem._state._actualDuration);
                scheduledItem.OnUpdate(scheduledItem._state);
            }
        }

        internal float SnapToCurrent(BeatFraction granularity)
        {
            var elapsedSinceLastMarker = ElapsedFractionalSinceLastMarker();
            var elapsedScaled = elapsedSinceLastMarker % granularity.Eval();
            var snapUnitsSinceLastMarker = Mathf.RoundToInt(elapsedScaled);
            var snapBeatsSinceLastMarker = snapUnitsSinceLastMarker * granularity.Eval();
            return lastMarkerEncounteredAt + snapBeatsSinceLastMarker;
        }

        private float ElapsedFractionalSinceLastMarker()
        {
            // using timeline to determine playback rather than accumulating errors via floating point accumulation
            fmodInstance.getTimelinePosition(out var positionMs);
            var timelineMsDiff = positionMs - lastMarker.positionMs;
            
            // (min/# beats) * ms/min = ms / beat
            var msPerBeat = 60e3f / lastMarker.tempoBpm;
            
            return (timelineMsDiff % msPerBeat) / msPerBeat;
        }

        internal void Stop()
        {
            fmodInstance.stop(STOP_MODE.ALLOWFADEOUT);
            
            // iterate through schedulables in unsorted manner, calling their abort method
            foreach (var (scheduledItem, finishTime) in scheduled.UnorderedItems)
            {
                scheduledItem.OnAborted(scheduledItem._state);
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
        public (SerializedTempoMarker, SerializedTempoMarker) FindPrevNextMarkers(int timelinePositionMs)
        {
            // we don't really care about the case where no markers are present, that shouldn't happen anyway
            if (markers.Length == 0)
            {
                throw new IndexOutOfRangeException();
            }

            // base cases for first and last markers
            if (markers[0].positionMs > timelinePositionMs)
            {
                return (new SerializedTempoMarker
                {
                    id = null,
                    positionMs = 0,
                    tempoBpm = markers[0].timeSignatureNumerator,
                    timeSignatureDenominator = markers[0].timeSignatureDenominator,
                    timeSignatureNumerator = markers[0].timeSignatureNumerator
                }, markers[0]);
            }

            if (markers[^1].positionMs < timelinePositionMs)
            {
                return (markers[^1], new SerializedTempoMarker
                {
                    id = null,
                    positionMs = int.MaxValue,
                    tempoBpm = markers[^1].timeSignatureNumerator,
                    timeSignatureDenominator = markers[^1].timeSignatureNumerator,
                    timeSignatureNumerator = markers[^1].timeSignatureNumerator,
                });
            }
            
            // search sorted markers array where t in inbetween consecutive markers, return the earlier of those markers
            // lol i love implementing binary search from scratch ...
            var l = 1;
            var r = markers.Length;

            while (l < r)
            {
                var m = (l + r) / 2;

                if (markers[m].positionMs >= timelinePositionMs && markers[m - 1].positionMs <= timelinePositionMs)
                {
                    return (markers[m-1], markers[m]);
                }

                if (markers[m].positionMs < timelinePositionMs)
                {
                    l = m + 1;
                }
                else
                {
                    r = m - 1;
                }
            }

            throw new Exception("funny haha error, I can't write binary search :*(");
        }

        public void SetCallback(GCHandle handle)
        {
            fmodInstance.setUserData(GCHandle.ToIntPtr(handle));
            beatCallback = new EVENT_CALLBACK(BeatEventCallback);
            fmodInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
        }

        private void OnBeat()
        {
            parent.OnFullBeat.Invoke();
        }

        private void OnTempoChange()
        {
            parent.OnTempoChange.Invoke(lastMarker);
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
                            ctxObj.lastMarkerEncounteredAt += ctxObj.elapsedWholeBeatsSinceMarker + 1;
                            ctxObj.elapsedWholeBeatsSinceMarker = 0;
                            ctxObj.OnTempoChange();
                        }
                        else
                        {
                            ctxObj.elapsedWholeBeatsSinceMarker += 1;
                        }
                        
                        ctxObj.OnBeat();
                        
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
    
    
    [System.Serializable]
    public struct SerializedTempoMarker: IEquatable<SerializedTempoMarker>
    {
        public string id;
        public float positionMs;
        public float tempoBpm;
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
