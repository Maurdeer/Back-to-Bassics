using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static PositionStateMachine;
public class MinnowSlashAction : SlashAction
{

    // Swords for Minnows;
    [Header("Minnows References")] 
    [SerializeField] private Animator leftSword;
    [SerializeField] private Animator rightSword;

    protected override IEnumerator SlashThread(SlashNode node)
    {
        leftSword.gameObject.SetActive(true);
        // Slash Initialization
        yield return StartCoroutine(SlashInitialization(node));
        // Broadcast
        yield return StartCoroutine(SlashBroadcast());

        // Prehit
        yield return StartCoroutine(SlashPrehit());

        // Hit
        SlashHit();
    }

    protected override IEnumerator SlashInitialization(SlashNode node)
    {
        _currNode = node;
        parentPawnSprite.Animator.SetFloat("speed", 1 / Conductor.Instance.spb);
        parentPawnSprite.FaceDirection(new Vector3((inverseFacingDirection ? -1 : 1) * _currNode.slashVector.x, 0, -1));
        parentPawnSprite.Animator.SetFloat("x_slashDir", _currNode.slashVector.x);
        parentPawnSprite.Animator.SetFloat("y_slashDir", _currNode.slashVector.y);
        syncedAnimationTime = (_currNode.slashLengthInBeats - prehitBeats - posthitBeats) * Conductor.Instance.spb;
        if (parentPawn.psm.IsOnState<Distant>())
        {
            parentPawn.psm.Transition<Center>();
            yield return new WaitForSeconds(Conductor.Instance.spb);
            syncedAnimationTime -= Conductor.Instance.spb;
        }
    }

    protected override IEnumerator SlashBroadcast()
    {
        parentPawnSprite.Animator.SetFloat("speed", 1 / syncedAnimationTime);
        // parentPawnSprite.Animator.Play($"{slashAnimationName}_broadcast");
        leftSword.Play("slash_broadcast");
        yield return new WaitForSeconds(syncedAnimationTime);
    }

    protected override IEnumerator SlashPrehit()
    {
        float prehitSeconds = prehitBeats * Conductor.Instance.spb;
        parentPawnSprite.Animator.SetFloat("speed", 1 / prehitSeconds);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_prehit");
        yield return new WaitForSeconds(prehitSeconds);
    }
    
    protected override void SlashHit()
    {
        parentPawnSprite.Animator.SetFloat("speed", 1 / (posthitBeats * Conductor.Instance.spb));
        BattleManager.Instance.Player.ReceiveAttackRequest(this);
    }
    

    public virtual void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementMissTracker();
        //---------------------------------------

        if (swapFacingDirectionAfterPostHit)
        {
            StartCoroutine(SwapDirectionAfterAnimation());
        }
        else
        {
            // parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");
        }
        BattleManager.Instance.Player.Damage(_currNode.dmg);
    }

    public virtual bool OnRequestDeflect(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null
            || !DirectionHelper.MaxAngleBetweenVectors(_currNode.slashVector, player.SlashDirection, 5f))
            return false;

        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementParryTracker();
        //---------------------------------------

        parentPawnSprite.Animator.Play($"{slashAnimationName}_deflected");
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
        return true;
    }
    public virtual bool OnRequestDodge(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null || !_currNode.dodgeDirections.Contains(player.DodgeDirection)) return false;

        parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");
        return true;
    }

}
