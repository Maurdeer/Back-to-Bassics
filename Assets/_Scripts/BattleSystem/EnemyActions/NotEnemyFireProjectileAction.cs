using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotEnemyFireProjectileAction : FireProjectileAction
{
    [SerializeField] private Animator _sourceAnimator;
    protected override void AnimateProjectileLaunch(FireProjectileNode node)
    {
        //parentPawnSprite.FaceDirection(new Vector3(node.relativeSpawnPosition.x, 0, -1));
        _sourceAnimator.SetFloat("speed", 1 / Conductor.Instance.spb);
        _sourceAnimator.Play(animationName);
    }

    protected override IEnumerator AnimateBeforeFire(FireProjectileNode node)
    {
        AnimateProjectileLaunch(node);
        string exit_name = exitFireName.Trim() == "" ? "idle_battle" : exitFireName;
        yield return new WaitUntil(() => _sourceAnimator.GetCurrentAnimatorStateInfo(0).IsName(exit_name) && !_sourceAnimator.IsInTransition(0));
        LaunchProjectile(node, timelineDurationInBeats - 1);
        _sourceAnimator.SetFloat("speed", 1);
        currentlyFiring = null;
    }
}
