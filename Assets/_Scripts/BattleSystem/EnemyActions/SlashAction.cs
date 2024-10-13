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

        // (Ryan) Transition Maybe shouldn't be here
        parentPawnSprite.Animator.SetFloat("speed", 1 / Conductor.Instance.spb);
        parentPawn.psm.Transition<Center>();
    }
    public void Slash(SlashNode node)
    {
        if (node.slashLengthInBeats < 1)
        {
            Debug.LogError("Timeline asset slash is not long enough.");
            return;
        }
        StartCoroutine(SlashThread(node));
    }
    private IEnumerator SlashThread(SlashNode node)
    {
        // Slash Initialization
        _currNode = node;

        // Broadcast
        // Direction setup
        // The y value here is facing forward
        if (parentPawn.psm.IsOnState<Distant>())
        {
            parentPawnSprite.Animator.SetFloat("speed", 1 / Conductor.Instance.spb);
            parentPawn.psm.Transition<Center>();
            yield return new WaitForSeconds(Conductor.Instance.spb);
        } 
        parentPawnSprite.FaceDirection(new Vector3(-_currNode.slashVector.x, 0, -1));
        parentPawnSprite.Animator.SetFloat("x_slashDir", _currNode.slashVector.x);
        parentPawnSprite.Animator.SetFloat("y_slashDir", _currNode.slashVector.y);
        float syncedAnimationTime = (_currNode.slashLengthInBeats - 1) * Conductor.Instance.spb;
        parentPawnSprite.Animator.SetFloat("speed", broadcastClip.length / syncedAnimationTime);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_broadcast");
        Debug.Log("Broadcast");
        //float broadcastHoldTime = (_currNode.slashLengthInBeats * parentPawn.EnemyData.SPB) - minSlashTillHitDuration;
        yield return new WaitForSeconds(syncedAnimationTime);

        // Animation Before Hit -> Setup animation speed
        //int beats = Mathf.RoundToInt(preHitClip.length / Conductor.Instance.spb);
        //if (beats == 0) beats = 1;
        //float syncedAnimationTime = beats * Conductor.Instance.spb;
        //parentPawnSprite.Animator.SetFloat("speed", preHitClip.length / syncedAnimationTime);
        parentPawnSprite.Animator.SetFloat("speed", preHitClip.length / Conductor.Instance.spb);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_prehit");
        Debug.Log("Prehit");
        yield return new WaitForSeconds(Conductor.Instance.spb);
        //yield return new WaitUntil(() => parentPawnSprite.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        // Hit Moment
        Debug.Log("Hitting");
        BattleManager.Instance.Player.ReceiveAttackRequest(this);
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
    }
    
    public void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementMissTracker();
        //---------------------------------------

        parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");
        BattleManager.Instance.Player.Damage(_currNode.dmg);
    }
    
    public bool OnRequestBlock(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null) return false;
        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementBlockTracker();
        //---------------------------------------

        //parentPawnSprite.Animator.SetTrigger("blocked");
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
        return true;
    }
    public bool OnRequestDodge(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null || !_currNode.dodgeDirections.Contains(player.DodgeDirection)) return false;

        parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");
        return true;
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
