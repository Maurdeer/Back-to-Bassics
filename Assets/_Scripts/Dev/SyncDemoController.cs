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

    [SerializeField] private TextMeshProUGUI onTimeText;

    private bool currentAnimationFinished = true;

    [SerializeField]
    private AnimationCurve lerpCurve;

    private Conductor.ConductorSchedulable _schedulable = null;

    [SerializeField]
    private SyncDemoAnimatedSprite animationTarget;

    [SerializeField, Tooltip("Check to auto schedule animation every half note, otherwise press Q")] private bool autoCallHalf;
    [SerializeField, Tooltip("Check to auto schedule animation every one thirds note")] private bool autoCallThirds;
    [SerializeField, Tooltip("Check to auto schedule animation every note, otherwise press W")] private bool autoCallWhole;
    [SerializeField, Tooltip("Check to auto schedule animation every second note (snap to note), otherwise press E")] private bool autoCallDouble;
    
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
        if (autoCallHalf)
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.half, 
                    Conductor.BeatFraction.half,
                    _schedulable);
                currentAnimationFinished = false;
            }
        }
        else if (autoCallThirds)
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    new Conductor.BeatFraction
                    {
                        numerator = 1,
                        denominator = 3
                    }, 
                    new Conductor.BeatFraction
                    {
                        numerator = 1,
                        denominator = 3
                    },
                    _schedulable);
                currentAnimationFinished = false;
            }
        }
        else if (autoCallWhole)
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.full, 
                    Conductor.BeatFraction.full,
                    _schedulable);
                currentAnimationFinished = false;
            }
        }
        else if (autoCallDouble)
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.full, 
                    Conductor.BeatFraction.fullDouble,
                    _schedulable);
                currentAnimationFinished = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space down");
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.half, 
                    Conductor.BeatFraction.half,
                    _schedulable);
                currentAnimationFinished = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.full, 
                    Conductor.BeatFraction.full,
                    _schedulable);
                currentAnimationFinished = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentAnimationFinished)
            {
                Conductor.Instance.ScheduleActionAsap(
                    Conductor.BeatFraction.full, 
                    Conductor.BeatFraction.fullDouble,
                    _schedulable);
                currentAnimationFinished = false;
            }
        }
    }

    void OnUpdateTempo(string tempo)
    {
        tempoText.text = $"Current Tempo: { tempo ?? "N/A" }";
    }
    
    // animation functions mapped to and invoked by the schedulable
    void OnAnimationStart(Conductor.ConductorSchedulableState state)
    {
        animationTarget.AnimateStart();

        var delta = (float)(state._actualStartBeat - state._snappedToStartBeat);

        onTimeText.color = Color.Lerp(Color.red, Color.blue, delta);

        if (Mathf.Abs(delta) < 0.05f)
        {
            onTimeText.color = Color.green;
            onTimeText.text = $"On time~\n - delta (beats)={delta}\n";
        } else if (delta < 0)
        {
            onTimeText.color = Color.red;
            onTimeText.text = $"Too early!\n - delta (beats)={delta}\n";
        } else if (delta > 0)
        {
            onTimeText.color = Color.blue;
            onTimeText.text = $"Too late!\n - delta (beats)={delta}\n";
        }
    }
    
    void OnAnimationUpdate(Conductor.ConductorSchedulableState state)
    {
        var animation = lerpCurve.Evaluate((float)state._elapsedProgressCount);
        animationTarget.AnimateUpdate(animation);
    }
    
    void OnAnimationComplete(Conductor.ConductorSchedulableState state)
    {
        currentAnimationFinished = true;
        animationTarget.AnimateComplete();
    }
    
    void OnAnimationAbort(Conductor.ConductorSchedulableState state)
    {
        currentAnimationFinished = true;
        animationTarget.AnimateAbort();
    }
}
