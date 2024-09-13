using System;
using System.Collections;
using System.Collections.Generic;
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

    private ConductorContext ctx
    {
        get => _ctx;
        set
        {
            value?.Stop();
            _ctx = value;
            value?.Start();
        }
    }
    
    private bool isConducting => ctx != null;
    
    public event Action OnQuarterBeat;
    public event Action OnHalfBeat;
    public event Action OnFullBeat;
    public event Action OnFirstBeat;
    public event Action OnLastBeat;
    public static float full = 1f;
    public static float half = 0.5f;
    public static float quarter = 0.25f;
    public static float eighth = 0.125f;
    public static float sixteenth = 0.0625f;

    [System.Serializable]
    public struct SerializedTempoMarker
    {
        public string id;
        public float positionMs;
        public float tempoBpm;
        public int timeSignatureNumerator;
        public int timeSignatureDenominator;
    }

    #region parsingHelperClasses
    /// <summary>
    /// Helper class purely for JSON parsing purposes...
    /// </summary>
    [System.Serializable]
    private struct SerializedEvent
    {
        public string eventPath;
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

    protected static Dictionary<string, SerializedTempoMarker[]> parsedEvents;
    
    public void Awake()
    {
        InitializeSingleton();

        var everything = JsonUtility.FromJson<SerializedEventCollection>(EventMarkerMap.text);
        parsedEvents = new();
        foreach (var e in everything.events)
        {
            parsedEvents.Add(e.eventPath, e.markers);
        }
    }

    public void BeginConducting(EnemyBattlePawn pawn)
    {
        if (isConducting) 
        {
            Debug.LogWarning("Conductor was issued to conduct when it already is conducting");
            return;
        }

        ctx = new ConductorContext(pawn)
        {
            pawn = pawn,
            fmodInstance = FMODUnity.RuntimeManager.CreateInstance(pawn.EnemyData.fmodEvent)
        };
        
        Beat = 0;
        ctx.Start();
        // this.spb = Data.SPB;
        OnFirstBeat?.Invoke();
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
        if (ctx.fmodInstance.getPlaybackState(out var state) == RESULT.OK)
        {
            if (state is PLAYBACK_STATE.STOPPED or PLAYBACK_STATE.STOPPING)
            {
                StopConducting();
                return;
            }
        }

        // timeline position is in miliseconds
        if (ctx.fmodInstance.getTimelinePosition(out int positionMs) == RESULT.OK)
        {
            
        }
    }

    private IEnumerator Conduct()
    {
        float quarterTime = spb / 4f;
        OnFirstBeat?.Invoke();
        while (isConducting)
        {
            yield return new WaitForSeconds(quarterTime);
            Beat += 0.25f;
            OnQuarterBeat?.Invoke();
            
            yield return new WaitForSeconds(quarterTime);
            Beat += 0.25f;
            OnQuarterBeat?.Invoke();
            OnHalfBeat?.Invoke();
            
            yield return new WaitForSeconds(quarterTime);
            Beat += 0.25f;
            OnQuarterBeat?.Invoke();
            
            yield return new WaitForSeconds(quarterTime);
            Beat += 0.25f;
            OnQuarterBeat?.Invoke();
            OnHalfBeat?.Invoke();
            OnFullBeat?.Invoke();
            
        }
        OnLastBeat?.Invoke();
    }
    
    
    private class ConductorContext
    {
        public EnemyBattlePawn pawn;
        public EventInstance fmodInstance;
        public SerializedTempoMarker[] markers;

        public ConductorContext(EnemyBattlePawn pawn)
        {
            this.pawn = pawn;
            fmodInstance = FMODUnity.RuntimeManager.CreateInstance(pawn.EnemyData.fmodEvent);
            markers = parsedEvents[pawn.EnemyData.fmodEvent.Path];
        }

        public void Start()
        {
            fmodInstance.start();
        }

        public void Stop()
        {
            fmodInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }

        public SerializedTempoMarker FindRelevantMarker(int timelinePositionMs)
        {
            // we don't really care about the case where no markers are present, that shouldn't happen anyway
            if (markers.Length == 0)
            {
                throw new IndexOutOfRangeException();
            }

            // base cases for first and last markers
            if (markers[0].positionMs >= timelinePositionMs)
            {
                return markers[0];
            }

            if (markers[^1].positionMs <= timelinePositionMs)
            {
                return markers[^1];
            }
            
            // search sorted markers array where t in inbetween consecutive markers, return the earlier of those markers
            // lol i love implementing binary search from scratch ...
            int l = 1;
            int r = markers.Length;

            while (l < r)
            {
                int m = (l + r) / 2;

                if (markers[m].positionMs >= timelinePositionMs && markers[m - 1].positionMs <= timelinePositionMs)
                {
                    return markers[m-1];
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
    }
}
