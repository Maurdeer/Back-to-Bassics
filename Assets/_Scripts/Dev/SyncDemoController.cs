using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SyncDemoController : MonoBehaviour
{
    [SerializeField]
    private EnemyBattlePawn demoEnemy;
    
    [SerializeField]
    private TextMeshProUGUI tempoText;

    private bool currentAnimationFinished = true;

    [FormerlySerializedAs("LerpCurve")] [SerializeField]
    private AnimationCurve lerpCurve;

    private Conductor.ConductorSchedulable _schedulable = null;
    
    // Start is called before the first frame update
    void Start()
    {
        _schedulable = new Conductor.ConductorSchedulable(
            onUpdate: o => OnAnimationUpdate(o),
            onStarted: o => OnAnimationStart(o),
            onCompleted: o => OnAnimationComplete(o),
            onAborted: o => OnAnimationAbort(o)
        );
        OnUpdateTempo(null);
        Conductor.Instance.OnTempoChange += m
            => OnUpdateTempo($"{m.tempoBpm} bpm, signature: {m.timeSignatureNumerator}/{m.timeSignatureDenominator}");
        Conductor.Instance.BeginConducting(demoEnemy);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space down");
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.half, 
                    Conductor.BeatFraction.half,
                    _schedulable);
            }

            Debug.Log("half beat");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.full, 
                    Conductor.BeatFraction.full,
                    _schedulable);
            }
            Debug.Log("whole beat");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.full, 
                    Conductor.BeatFraction.fullDouble,
                    _schedulable);
            }
            Debug.Log("two beats - snap to whole beat");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.fullDouble, 
                    Conductor.BeatFraction.fullQuadruple,
                    _schedulable);
            }
            Debug.Log("four beats - snap to 2 beats");
        }
    }

    void OnUpdateTempo(string tempo)
    {
        tempoText.text = $"Current Tempo: { tempo ?? "N/A" }";
    }
    
    // animation functions mapped to and invoked by the schedulable
    void OnAnimationStart(Conductor.ConductorSchedulableState state)
    {
        
    }
    
    void OnAnimationUpdate(Conductor.ConductorSchedulableState state)
    {
        var animation = lerpCurve.Evaluate(state._elapsedProgressCount);
    }
    
    void OnAnimationComplete(Conductor.ConductorSchedulableState state)
    {
        currentAnimationFinished = true;
    }
    
    void OnAnimationAbort(Conductor.ConductorSchedulableState state)
    {
        currentAnimationFinished = true;
    }
}
