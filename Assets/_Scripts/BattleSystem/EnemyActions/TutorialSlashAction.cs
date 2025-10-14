using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PositionStateMachine;

public class TutorialSlashAction : SlashAction
{
    private bool hasProgressed = false;
    [SerializeField] private float timeBeforeHitPlayer = 0.08f;

    protected override IEnumerator SlashThread(SlashNode node)
    {
        // Slash Initialization
        _currNode = node;
        parentPawnSprite.Animator.SetFloat("speed", 1 / Conductor.Instance.spb);
        parentPawnSprite.FaceDirection(new Vector3((inverseFacingDirection ? -1 : 1) * _currNode.slashVector.x, 0, -1));
        parentPawnSprite.Animator.SetFloat("x_slashDir", _currNode.slashVector.x);
        parentPawnSprite.Animator.SetFloat("y_slashDir", _currNode.slashVector.y);
        float syncedAnimationTime = (_currNode.slashLengthInBeats - prehitBeats - posthitBeats) * Conductor.Instance.spb;
        if (parentPawn.psm.IsOnState<Distant>())
        {
            parentPawn.psm.Transition<Center>();
            yield return new WaitForSeconds(Conductor.Instance.spb);
            syncedAnimationTime -= Conductor.Instance.spb;
        }

        // Broadcast
        parentPawnSprite.Animator.SetFloat("speed", 1 / syncedAnimationTime);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_broadcast");
        yield return new WaitForSeconds(syncedAnimationTime);

        // Prehit
        float prehitSeconds = prehitBeats * Conductor.Instance.spb;
        parentPawnSprite.Animator.SetFloat("speed", 1 / prehitSeconds);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_prehit");
        yield return new WaitForSeconds(prehitSeconds - timeBeforeHitPlayer);

        // (10/14/25 Joseph) Tutorial Management Code
        // Somewhat dirty, still works.
        TutorialManager.Instance.Pause();
        string dir = "";
        if (_currNode.slashVector.x > 0) dir = "Right";
        else if (_currNode.slashVector.x < 0) dir = "Left";
        DialogueManager.Instance.RunDialogueNode("bassics-guitarSwing" + dir);

        while (!hasProgressed)
        {
            if ((Input.GetKeyDown("right") && _currNode.slashVector.x > 0) ||
            (Input.GetKeyDown("left") && _currNode.slashVector.x < 0)||
            (Input.GetKeyDown("up") && _currNode.slashVector.y > 0))
            {
                hasProgressed = true;
            }
            yield return null;
        }
        TutorialManager.Instance.Unpause();
        hasProgressed = false;

        yield return new WaitForSeconds(timeBeforeHitPlayer);
        
        // Hit
        parentPawnSprite.Animator.SetFloat("speed", 1 / (posthitBeats * Conductor.Instance.spb));
        BattleManager.Instance.Player.ReceiveAttackRequest(this);
    }
}
