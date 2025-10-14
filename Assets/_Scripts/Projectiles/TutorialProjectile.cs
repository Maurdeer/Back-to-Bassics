using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStateMachine;

public class TutorialProjectile : Projectile
{
    // Has this been paused for the tutorial before?
    bool isPausedForTutorial = false;
    public override void Fire(Vector3 lifetimeDisplacement, float duration)
    {
        var originalLocation = transform.position;
        _slashDirection = -lifetimeDisplacement;
        transform.localScale = _initialScale;
        activeScheduable = new Conductor.ConductorSchedulable(
            onStarted: (state, ctxState) =>
            {
                isDestroyed = false;
                gameObject.SetActive(true);
                if (_spriteRenderer != null) _spriteRenderer.enabled = true;
                if (_meshRenderer != null) _meshRenderer.enabled = true;
            },
            onUpdate: (state, ctxState) =>
            {
                // if (isPausedForTutorial) return;
                transform.position = originalLocation + lifetimeDisplacement * state._elapsedProgressCount;
                // Debug.Log($"{state._elapsedProgressCount}");
                if (state._elapsedProgressCount > .95 && !isPausedForTutorial)
                {
                    // Debug.Log("HEY I SHOULD STOP NOW");
                    string dir = "";
                    if (_slashDirection.x > 0) dir = "Right";
                    if (_slashDirection.x < 0) dir = "Left";
                    if (_slashDirection.y > 0) dir = "Up"; 
                    DialogueManager.Instance.RunDialogueNode("bassics-noteHit" + dir);
                    TutorialManager.Instance.Pause();
                    isPausedForTutorial = true;
                }
                // _rb.position = originalLocation + lifetimeDisplacement * state._elapsedProgressCount;
                // _rb.velocity = lifetimeDisplacement / ctxState.spb; // current SPB at the update
            },
            onCompleted: (state, ctxState) =>
            {
                BattleManager.Instance.Player.ReceiveAttackRequest(this);
            },
            onAborted: (state) =>
            {

            }
        );

        Conductor.Instance.ScheduleActionAsap(duration, Conductor.Instance.Beat, activeScheduable, forceStart: true);
        // Debug.Log("THIS FUNCTION IS FIRED");
    }

    void Update()
    {
        // This solution isn't exactly satisfactory I'm ngl
        if (!isPausedForTutorial) return;
        if ((Input.GetKeyDown("right") && _slashDirection.x > 0) ||
            (Input.GetKeyDown("left") && _slashDirection.x < 0)||
            (Input.GetKeyDown("up") && _slashDirection.y > 0))
        {
            TutorialManager.Instance.Unpause();
        }
    }
    
    public override bool OnRequestDeflect(IAttackReceiver receiver)
    {
        return base.OnRequestDeflect(receiver);
    }
}
