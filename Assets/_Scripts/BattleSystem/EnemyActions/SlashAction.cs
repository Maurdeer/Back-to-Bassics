using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static PositionStateMachine;
public class SlashAction : EnemyAction, IAttackRequester
{
    [Header("Slash Action")]
    [SerializeField] private string slashAnimationName;
    [SerializeField] private AnimationClip broadcastClip;
    [SerializeField] private AnimationClip preHitClip;
    [SerializeField] private AnimationClip postHitClip;
    [SerializeField] private AnimationClip deflectedClip;
    public float minSlashTillHitDuration => (preHitClip.length + broadcastClip.length);
    public float minSlashTillHitInBeats => minSlashTillHitDuration / parentPawn.EnemyData.SPB;
    private SlashNode _currNode;
    public void Broadcast(Direction direction)
    {
        Vector2 slashDirection = DirectionHelper.GetVectorFromDirection(direction);
        // **************Revise this***********
        // The y value here is facing forward
        parentPawnSprite.FaceDirection(new Vector3(-slashDirection.x, 0, -1));
        parentPawnSprite.Animator.SetFloat("x_slashDir", slashDirection.x);
        parentPawnSprite.Animator.SetFloat("y_slashDir", slashDirection.y);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_broadcast");

        // (Ryan) Maybe it shouldn't be here
        parentPawn.psm.Transition<Center>();
    }
    public void Slash(SlashNode node)
    {
        // (Ryan) Nor here :p
        parentPawn.psm.Transition<Center>();
        StartCoroutine(SlashThread(node));
    }
    private IEnumerator SlashThread(SlashNode node)
    {
        // Slash Initialization
        _currNode = node;

        // Broadcast
        // Direction setup
        // The y value here is facing forward
        parentPawnSprite.FaceDirection(new Vector3(-_currNode.slashVector.x, 0, -1));
        parentPawnSprite.Animator.SetFloat("x_slashDir", _currNode.slashVector.x);
        parentPawnSprite.Animator.SetFloat("y_slashDir", _currNode.slashVector.y);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_broadcast");

        float broadcastHoldTime = (_currNode.slashLengthInBeats * parentPawn.EnemyData.SPB) - minSlashTillHitDuration;
        yield return new WaitForSeconds(broadcastHoldTime);

        // Animation Before Hit -> Setup animation speed
        //int beats = Mathf.RoundToInt(node.preHitClip.length / Conductor.Instance.spb);
        //if (beats == 0) beats = 1;
        //float syncedAnimationTime = beats * Conductor.Instance.spb;
        //parentPawn.SpriteAnimator.SetFloat("speed", node.preHitClip.length / syncedAnimationTime);

        parentPawnSprite.Animator.Play($"{slashAnimationName}_prehit");
        yield return new WaitForSeconds(preHitClip.length);
        //yield return new WaitUntil(() => parentPawnSprite.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        // Hit Moment
        if (BattleManager.Instance.Player.ReceiveAttackRequest(this))
        {
            PerformSlashOnPlayer();
            BattleManager.Instance.Player.CompleteAttackRequest(this);
        }
        yield return new WaitUntil(() => parentPawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName($"{slashAnimationName}_posthit") ||
        parentPawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName($"{slashAnimationName}_deflected"));
        if (parentPawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName($"{slashAnimationName}_posthit"))
        {
            yield return new WaitForSeconds(postHitClip.length);
        }
        else
        {
            yield return new WaitForSeconds(deflectedClip.length);
        }
        Debug.Log("SLASH COMPLETE");
        //yield return new WaitUntil(() => parentPawnSprite.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
    }
    public bool OnRequestBlock(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null) return false;
        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementBlockTracker();
        //---------------------------------------

        //parentPawnSprite.Animator.SetTrigger("blocked");

        receiver.CompleteAttackRequest(this);
        return true;
    }
    public bool OnRequestDeflect(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null 
            || !DirectionHelper.MaxAngleBetweenVectors(-_currNode.slashVector, player.SlashDirection, 5f)) 
                return false; 

        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementParryTracker();
        //---------------------------------------

        parentPawnSprite.Animator.Play($"{slashAnimationName}_deflected");
        if (_currNode.staggersParent)
        {
            parentPawn.Stagger();
        }
        receiver.CompleteAttackRequest(this);
        return true;
    }
    public bool OnRequestDodge(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null || !_currNode.dodgeDirections.Contains(player.DodgeDirection)) return false;

        parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");
        receiver.CompleteAttackRequest(this);
        return true;
    }
    private void PerformSlashOnPlayer()
    {
        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementMissTracker();
        //---------------------------------------

        parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");
        BattleManager.Instance.Player.Damage(_currNode.dmg);
    }
}

[Serializable]
public struct SlashNode
{
    public Direction slashDirection;
    public bool isCharged;
    public bool staggersParent;
    public int dmg;
    public float slashLengthInBeats;
    public Direction[] dodgeDirections;
    public Vector2 slashVector => DirectionHelper.GetVectorFromDirection(slashDirection);
}
