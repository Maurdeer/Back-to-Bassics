using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class MinnowsSwarmBehaviour : PlayableBehaviour
{
    public SlashNode slash_node;

    // Fire Projectile
    public Direction fireDirection;
    public Direction minnowsDirection;
    public float fireDistance = 16f;
    private readonly Vector3[] randDirection = new Vector3[3] { Vector3.right, Vector3.left, Vector3.up };

    public MinnowsSwarmActionChoice choice;
    private bool _performed;
    public MinnowsSwarmAction _cachedAction;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        if (_performed) return;
        _performed = true;
        _cachedAction = info.output.GetUserData() as MinnowsSwarmAction;

        if (_cachedAction == null)
        {
            Debug.LogError($"{this} Node did not perform a swarm action");
            return;
        }
        _cachedAction.timelineDurationInBeats = (float)playable.GetDuration();

        switch (choice)
        {
            case MinnowsSwarmActionChoice.Idle:
                _cachedAction.IdleSelf(minnowsDirection);
                break;
            case MinnowsSwarmActionChoice.Fire:
                Vector3 firePosition = fireDirection == Direction.None ?
                randDirection[UnityEngine.Random.Range(0, 3)] * fireDistance :
                DirectionHelper.GetVectorFromDirection(fireDirection) * fireDistance;

                FireProjectileNode fire_node;
                fire_node.useDefault = true;
                fire_node.projRef = null;
                fire_node.duration = (float)playable.GetDuration();
                fire_node.relativeSpawnPosition = firePosition;
                fire_node.direction = fireDirection;
                _cachedAction.ShootPlayer(fire_node);
                break;
            case MinnowsSwarmActionChoice.Slash:
                slash_node.slashLengthInBeats = (float)playable.GetDuration();
                _cachedAction.SlashPlayer(slash_node);
                break;
            case MinnowsSwarmActionChoice.Cover:
                break;
        }
        
        
        
    }
}
