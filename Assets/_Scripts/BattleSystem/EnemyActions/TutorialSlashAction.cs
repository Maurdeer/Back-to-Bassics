using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PositionStateMachine;

public class TutorialSlashAction : SlashAction
{
    private bool hasProgressed = false;
    [SerializeField] private float timeBeforeHitPlayer = 0.03f;

    protected override IEnumerator SlashThread(SlashNode node)
    {
        // Slash Initialization
        yield return StartCoroutine(SlashInitialization(node));
        // Broadcast
        yield return StartCoroutine(SlashBroadcast());

        // Prehit
        float prehitSeconds = prehitBeats * Conductor.Instance.spb;
        parentPawnSprite.Animator.SetFloat("speed", 1 / prehitSeconds);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_prehit");
        yield return new WaitForSeconds(prehitSeconds - timeBeforeHitPlayer);

        // (10/14/25 Joseph) Tutorial Management Code
        // Somewhat dirty, still works.
        // This should probably be modified to work on attack materialized?
        // Actually that doesn't make sense
        // There's probably some better logic behind this attack that could be worked in
        if (TutorialManager.Instance.TutorialEnabled)
        {
            bool paused = false;
            if (TutorialManager.Instance.CheckForSlash && !BattleManager.Instance.Player.deflectionWindow)
            {
                TutorialManager.Instance.Pause(_currNode.slashVector);
                string dir = "";
                if (_currNode.slashVector.x > 0) dir = "Right";
                else if (_currNode.slashVector.x < 0) dir = "Left";
                DialogueManager.Instance.RunDialogueNode("bassics-guitarSwing" + dir);
                paused = true;

            }

            else if (TutorialManager.Instance.CheckForDodge && !BattleManager.Instance.Player.dodging)
            {
                TutorialManager.Instance.Pause(new Vector3(0, -1, 0));
                DialogueManager.Instance.RunDialogueNode("bassics-battle_unblockable");
                paused = true;
            }

            if (paused)
            {
                while (TutorialManager.Instance.PausedForTutorial)
                {
                    yield return null;
                }

                TutorialManager.Instance.Unpause();
                hasProgressed = false;
            }
        }


        yield return new WaitForSeconds(timeBeforeHitPlayer);

        // Hit
        parentPawnSprite.Animator.SetFloat("speed", 1 / (posthitBeats * Conductor.Instance.spb));
        BattleManager.Instance.Player.ReceiveAttackRequest(this);
    }

    public override void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // This is bugged because it doesn't know which attack is the bugged attack
        TutorialManager.Instance.ModifyNumOfMisses(5);
        base.OnAttackMaterialize(receiver);
    }

    public override bool OnRequestDeflect(IAttackReceiver receiver)
    {
        TutorialManager.Instance.ModifyNumOfMisses(-5);
        return base.OnRequestDeflect(receiver);
    }

    public override bool OnRequestDodge(IAttackReceiver receiver)
    {
        TutorialManager.Instance.ModifyNumOfMisses(-5);

        if (TutorialManager.Instance.CheckForDodge)
        {
            parentPawn.StaggerDamage(_staggerDamage);
            if (TutorialManager.Instance.TutorialEnabled) DialogueManager.Instance.RunDialogueNode("bassics-battle_one-more");
        }
        return base.OnRequestDodge(receiver);
    }

    // protected override Coroutine OnStopAction()
    // {
    //     StopAllCoroutines();
    //     return null;
    // }
    
}
