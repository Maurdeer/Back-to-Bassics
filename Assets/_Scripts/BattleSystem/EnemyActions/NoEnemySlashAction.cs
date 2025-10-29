using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static PositionStateMachine;

public class NoEnemySlashAction : SlashAction
{
    [SerializeField] private PawnSprite sourcePawn;
    [SerializeField] private bool flippable = false;
    protected override IEnumerator SlashInitialization(SlashNode node)
    {
        _currNode = node;
        sourcePawn.Animator.SetFloat("speed", 1 / Conductor.Instance.spb);
        sourcePawn.FaceDirection(new Vector3((inverseFacingDirection ? -1 : 1) * _currNode.slashVector.x, 0, -1), !flippable);
        sourcePawn.Animator.SetFloat("x_slashDir", _currNode.slashVector.x);
        sourcePawn.Animator.SetFloat("y_slashDir", _currNode.slashVector.y);
        syncedAnimationTime = (_currNode.slashLengthInBeats - prehitBeats - posthitBeats) * Conductor.Instance.spb;
        yield return null;
    }

    protected override IEnumerator SlashBroadcast()
    {
        sourcePawn.Animator.SetFloat("speed", 1 / syncedAnimationTime);
        sourcePawn.Animator.Play($"{slashAnimationName}_broadcast");
        yield return new WaitForSeconds(syncedAnimationTime);
    }

    protected override IEnumerator SlashPrehit()
    {
        float prehitSeconds = prehitBeats * Conductor.Instance.spb;
        sourcePawn.Animator.SetFloat("speed", 1 / prehitSeconds);
        sourcePawn.Animator.Play($"{slashAnimationName}_prehit");
        yield return new WaitForSeconds(prehitSeconds);
    }

    protected override void SlashHit()
    {
        sourcePawn.Animator.SetFloat("speed", 1 / (posthitBeats * Conductor.Instance.spb));
        BattleManager.Instance.Player.ReceiveAttackRequest(this);
    }

    public override void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementMissTracker();
        //---------------------------------------

        if (swapFacingDirectionAfterPostHit)
        {
            sourcePawn.FaceDirection(new Vector3(-sourcePawn.Animator.GetFloat("x_faceDir"), 0, 0), true);
        }
        sourcePawn.Animator.Play($"{slashAnimationName}_posthit");
        BattleManager.Instance.Player.Damage(_currNode.dmg);
        StartCoroutine(ResetSpeed(posthitBeats * Conductor.Instance.spb));
    }

    protected override IEnumerator SwapDirectionAfterAnimation()
    {
        string animation_name = $"{slashAnimationName}_posthit";
        sourcePawn.Animator.Play(animation_name);
        yield return new WaitUntil(() => {
            AnimatorStateInfo info = sourcePawn.Animator.GetCurrentAnimatorStateInfo(0);
            return info.IsName(animation_name) && sourcePawn.Animator.IsInTransition(0);
        });
        sourcePawn.Animator.SetFloat("speed", 1);
        sourcePawn.FaceDirection(new Vector3(-sourcePawn.Animator.GetFloat("x_faceDir"), 0, 0), true);
    }

    public override bool OnRequestDeflect(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null
            || !DirectionHelper.MaxAngleBetweenVectors(_currNode.slashVector, player.SlashDirection, 5f))
            return false;

        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementParryTracker();
        //---------------------------------------

        sourcePawn.Animator.Play($"{slashAnimationName}_deflected");
        if (_currNode.staggersParent && parentPawn is EnemyBattlePawn enemyPawn)
        {
            /// [WREKCON] BASSICS_SPECIAL
            if (enemyPawn.EnemyData.Name == "Bassics")
            {
                UIManager.Instance.WreckconQuests.MarkAchievement(1);
            }
            ///============
            enemyPawn.StaggerDamage(_staggerDamage);
        }
        StartCoroutine(ResetSpeed(() => sourcePawn.Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_battle")));
        return true;
    }

    private IEnumerator ResetSpeed(float waitDuration)
    {
        yield return new WaitForSeconds(waitDuration);
        sourcePawn.Animator.SetFloat("speed", 1);
    }

    private IEnumerator ResetSpeed(System.Func<bool> predicate)
    {
        yield return new WaitUntil(predicate);
        sourcePawn.Animator.SetFloat("speed", 1);
    }

    public override bool OnRequestDodge(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null || !_currNode.dodgeDirections.Contains(player.DodgeDirection)) return false;

        sourcePawn.Animator.Play($"{slashAnimationName}_posthit");
        sourcePawn.Animator.SetFloat("speed", 1);
        return true;
    }
}
