using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using static PositionStateMachine;
public class MultiSlashAction : SlashAction
{
    // Figure out how to modify this function to make it such that I can determine when attacks hit
    [SerializeField] private int numOfSlashes;
    private int currCount;
    protected override IEnumerator SlashThread(SlashNode node)
    {
        currCount = 0;
        Debug.Log("Setting currCount to 0");
        // parentPawnSprite.Animator.StartPlayback();
        yield return base.SlashThread(node);
        Debug.Log("Finished with initial SlashThread!");
    }
    
    public override void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // base.OnAttackMaterialize();
        BattleManager.Instance.Player.Damage(_currNode.dmg);
        UIManager.Instance.IncrementMissTracker();
        parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");

        
        
        if (currCount < numOfSlashes - 1)
        {
            currCount++;
            StartCoroutine(ExtraAttackAfterHit());
        } else
        {
            parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");
        }
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

        if (currCount < numOfSlashes - 1)
        {
            StartCoroutine(ExtraAttackAfterDeflect());
            currCount++;
            
            if (_currNode.staggersParent && parentPawn is EnemyBattlePawn enemyPawn)
            {
                enemyPawn.StaggerDamage(_staggerDamage);
            }
            // Debug.Log("FirstSlash set to false!");
        } else
        {
            parentPawnSprite.Animator.Play($"{slashAnimationName}_deflected");
        }
        return true;
    }


    // Create a dedicated animation for this one? I'm not sure what else to do tbh
    private IEnumerator ExtraAttackAfterHit()
    {
        float prehitSeconds = prehitBeats * Conductor.Instance.spb;
        yield return new WaitForSeconds(prehitSeconds);

        SwitchDirection();
        parentPawnSprite.Animator.SetFloat("speed", 1 / prehitSeconds);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_broadcast");
        yield return new WaitForSeconds(prehitSeconds);

        // // Debug.Log($"Animator time is {parentPawnSprite.Animator.playbackTime}");
        //  parentPawnSprite.Animator.SetFloat("speed", -1 / prehitSeconds);
        // parentPawnSprite.Animator.Play($"{slashAnimationName}_posthit");
        // yield return new WaitForSeconds(prehitSeconds);

        // parentPawnSprite.Animator.speed = 1f;
        parentPawnSprite.Animator.SetFloat("speed", 1 / prehitSeconds);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_prehit");
        yield return new WaitForSeconds(prehitSeconds);

        parentPawnSprite.Animator.SetFloat("speed", 1 / (posthitBeats * Conductor.Instance.spb));
        BattleManager.Instance.Player.ReceiveAttackRequest(this);
    }
    // This works just fine as after a deflect type beat sort of hit
    private IEnumerator ExtraAttackAfterDeflect()
    {
        float prehitSeconds = prehitBeats * Conductor.Instance.spb;

        // Debug.Log($"Animator time is {parentPawnSprite.Animator.playbackTime}");
        parentPawnSprite.Animator.SetFloat("speed", -1 / prehitSeconds);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_prehit");
        yield return new WaitForSeconds(prehitSeconds);

        // parentPawnSprite.Animator.speed = 1f;
        Debug.Log($"Animator time is {parentPawnSprite.Animator.playbackTime}");
        parentPawnSprite.Animator.SetFloat("speed", 1 / prehitSeconds);
        parentPawnSprite.Animator.Play($"{slashAnimationName}_prehit");
        yield return new WaitForSeconds(prehitSeconds);

        parentPawnSprite.Animator.SetFloat("speed", 1 / (posthitBeats * Conductor.Instance.spb));
        BattleManager.Instance.Player.ReceiveAttackRequest(this);
        // yield return new WaitForSeconds(prehitSeconds);
    }

    
    private void SwitchDirection()
    {
        switch(_currNode.slashDirection)
        {
            case Direction.North:
                _currNode.slashDirection = Direction.South;
                break;
            case Direction.South:
                _currNode.slashDirection = Direction.North;
                break;
            case Direction.East:
                _currNode.slashDirection = Direction.West;
                break;
            case Direction.West:
                _currNode.slashDirection = Direction.East;
                break;

        }
        parentPawnSprite.FaceDirection(new Vector3(_currNode.slashVector.x, 0, -1), true);
        parentPawnSprite.Animator.SetFloat("x_slashDir", _currNode.slashVector.x);
        parentPawnSprite.Animator.SetFloat("y_slashDir", _currNode.slashVector.y);
        
        // _currNode.slashDirection == Direction.North ? Direction.South : Direction.North;
        // _currNode.slashDirection == Direction.East ? Direction.West : Direction.East;
    }
}
