using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStateMachine;

public class TutorialProjectile : Projectile
{
    // Necessary to check if this been paused for the tutorial before?
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
                // Debug.Log($"I'm currently at {state._elapsedProgressCount}");
                if (TutorialManager.Instance.TutorialEnabled && state._elapsedProgressCount > .92 && !isPausedForTutorial)
                {
                    // Debug.Log("HEY I SHOULD STOP NOW");
                    // Debug.Log("Pausing for the tutorial now!");
                    // Debug.Log("Direction is ")
                    if (TutorialManager.Instance.CheckForSlash)
                    {
                        string dir = "";
                        if (_slashDirection.x > 0) dir = "Right";
                        if (_slashDirection.x < 0) dir = "Left";
                        if (_slashDirection.y > 0) dir = "Up";
                        DialogueManager.Instance.RunDialogueNode("bassics-noteHit" + dir);
                        TutorialManager.Instance.Pause(_slashDirection);
                    } else if (TutorialManager.Instance.CheckForDodge)
                    {

                        DialogueManager.Instance.RunDialogueNode("bassics-battle_unblockable");
                        TutorialManager.Instance.Pause(new Vector3(0, -1, 0));
                    }
                    isPausedForTutorial = true;
                    
                }
                // _rb.position = originalLocation + lifetimeDisplacement * state._elapsedProgressCount;
                // _rb.velocity = lifetimeDisplacement / ctxState.spb; // current SPB at the update
            },
            onCompleted: (state, ctxState) =>
            {
                if (BattleManager.Instance.Player.ReceiveAttackRequest(this))
                {
                    // TutorialManager.Instance.InTutorialState = true;
                    if (TutorialManager.Instance.CheckForSlash) TutorialManager.Instance.ModifyNumOfMisses(1);
                    if (TutorialManager.Instance.CheckForDodge) TutorialManager.Instance.ModifyNumOfMisses(5);
                }
            },
            onAborted: (state) =>
            {
                 
            }
        );

        Conductor.Instance.ScheduleActionAsap(duration, Conductor.Instance.Beat, activeScheduable, forceStart: true);
        // Debug.Log("THIS FUNCTION IS FIRED");
    }

    public override bool OnRequestDeflect(IAttackReceiver receiver)
    {
        TutorialManager.Instance.ModifyNumOfMisses(-1);
        return base.OnRequestDeflect(receiver);
    }

    public virtual bool OnRequestDodge(IAttackReceiver receiver)
    {
        TutorialManager.Instance.ModifyNumOfMisses(-5);
        return base.OnRequestDodge(receiver);
    }
    
    public override void Reset()
    {
        base.Reset();
        isPausedForTutorial = false;
    }
}
