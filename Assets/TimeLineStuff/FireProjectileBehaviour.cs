using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class FireProjectileBehaviour : PlayableBehaviour
{
    public GameObject projectileRef;
    public Direction fireDirection;
    public float fireDistance = 2f;
    private bool _performed;
    private FireProjectileAction _cachedProjectileActionRef;
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        if (_performed) return;
        _performed = true;
        _cachedProjectileActionRef = info.output.GetUserData() as FireProjectileAction;
        if (_cachedProjectileActionRef == null)
        {
            Debug.LogError($"{this} Node did not shoot a projectile");
            return;
        }
        Vector3 firePosition = -DirectionHelper.GetVectorFromDirection(fireDirection)*fireDistance;
        FireProjectileNode node;
        node.projRef = projectileRef;
        node.speed = (float)playable.GetDuration();
        node.relativeSpawnPosition = firePosition;
        _cachedProjectileActionRef.FireProjectileAtPlayer(node);
    }
}
