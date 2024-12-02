using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static PositionStateMachine;
public class NondeflectableAction : EnemyAction, IAttackRequester
{
    [Header("Nondeflectable Action")]
    [SerializeField] private int damage = 4;
    [SerializeField] private string attackAnimationName;
    [Header("Doge Directions")]
    [SerializeField] private bool east;
    [SerializeField] private bool west;
    [SerializeField] private bool south;
    [SerializeField] private bool north;

    public void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementMissTracker();
        //---------------------------------------
        BattleManager.Instance.Player.Damage(damage);
    }

    public float GetDeflectionCoyoteTime()
    {
        return Conductor.BeatFraction.sixteenth;
    }

    public bool OnRequestBlock(IAttackReceiver receiver)
    {
        return false;
    }
    
    public bool OnRequestDeflect(IAttackReceiver receiver)
    {
        
        return false;
    }
    public bool OnRequestDodge(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null) return true;

        return (east && player.DodgeDirection == Direction.East) ||
        (west && player.DodgeDirection == Direction.West) ||
        (south && player.DodgeDirection == Direction.South) ||
        (north && player.DodgeDirection == Direction.North);
    }

    protected override Coroutine OnStopAction()
    {
        StopAllCoroutines();
        return null;
    }

    protected override void OnStartAction()
    {
        StartCoroutine(SlashThread());
    }

    private IEnumerator SlashThread()
    {
        float attackDuration = timelineDurationInBeats * Conductor.Instance.spb;
        parentPawnSprite.Animator.SetFloat("speed", 1 / attackDuration);
        parentPawnSprite.Animator.Play($"{attackAnimationName}");
        yield return new WaitForSeconds(attackDuration);
        BattleManager.Instance.Player.ReceiveAttackRequest(this);
        parentPawnSprite.Animator.Play("idle_battle");
        StopAction();
    }
}

[Serializable]
public struct NondeflectNode
{
    public Direction slashDirection;
    public bool isCharged;
    public bool staggersParent;
    public int dmg;
    public float slashLengthInBeats;
    public Direction[] dodgeDirections;
    public Vector2 slashVector => DirectionHelper.GetVectorFromDirection(slashDirection);
}
