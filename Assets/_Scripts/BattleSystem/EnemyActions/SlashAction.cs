using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static PositionStateMachine;
public class SlashAction : EnemyAction, IAttackRequester
{
    [Header("Slash Action")]
    [SerializeField] protected string slashAnimationName;
    [SerializeField] protected bool inverseFacingDirection = false;
    [SerializeField] protected int _staggerDamage = 5;
    [SerializeField] protected float prehitBeats = 0.5f;
    [SerializeField] protected float posthitBeats = 0.5f;

    [Header("Slash References")]
    //[SerializeField] private AnimationClip broadcastClip;
    //[SerializeField] private AnimationClip preHitClip;
    [SerializeField] private ParticleSystem indicatorSpark;
    //public float minSlashTillHitDuration => (preHitClip.length + broadcastClip.length);
    //public float minSlashTillHitInBeats => minSlashTillHitDuration / parentPawn.EnemyData.SPB;
    protected SlashNode _currNode;
    protected float syncedAnimationTime;
    //Amount of stagger damage towards enemy of successful deflect.
    
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
    protected virtual IEnumerator SlashThread(SlashNode node)
    {
        // Slash Initialization
        yield return StartCoroutine(SlashInitialization(node));
        // Broadcast
        yield return StartCoroutine(SlashBroadcast());

        // Prehit
        yield return StartCoroutine(SlashPrehit());

        // Hit
        SlashHit();
    }

    protected virtual IEnumerator SlashInitialization(SlashNode node)
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

    protected virtual IEnumerator SlashBroadcast()
    {
        parentPawnSprite.Animator.SetFloat("speed", 1 / syncedAnimationTime);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_broadcast");
        yield return new WaitForSeconds(syncedAnimationTime);
    }

    protected virtual IEnumerator SlashPrehit()
    {
        float prehitSeconds = prehitBeats * Conductor.Instance.spb;
        parentPawnSprite.Animator.SetFloat("speed", 1 / prehitSeconds);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_prehit");
        yield return new WaitForSeconds(prehitSeconds);
    }
    
    protected virtual void SlashHit()
    {
        parentPawnSprite.Animator.SetFloat("speed", 1 / (posthitBeats * Conductor.Instance.spb));
        BattleManager.Instance.Player.ReceiveAttackRequest(this);
    }
    

    public virtual void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // (TEMP) DEBUG UI Tracker -------
        UIManager.Instance.IncrementMissTracker();
        //---------------------------------------

        parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");
        //parentPawnSprite.Animator.SetFloat("x_faceDir", -parentPawnSprite.Animator.GetFloat("x_faceDir"));
        BattleManager.Instance.Player.Damage(_currNode.dmg);
    }

    public float GetDeflectionCoyoteTime()
    {
        return Conductor.BeatFraction.sixteenth;
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

    protected override Coroutine OnStopAction()
    {
        StopAllCoroutines();
        return null;
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
