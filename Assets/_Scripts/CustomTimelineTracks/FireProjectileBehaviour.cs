using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class FireProjectileBehaviour : PlayableBehaviour
{
    public bool useDefault = true;
    public GameObject projectileRef;
    public Direction fireDirection;
    public float fireDistance = 12f;
    private bool _performed;
    private FireProjectileAction _cachedProjectileActionRef;
    private readonly Vector3[] randDirection = new Vector3[3] { Vector3.right, Vector3.left, Vector3.up};
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
        Vector3 firePosition = fireDirection == Direction.None ? 
            randDirection[UnityEngine.Random.Range(0, 3)] * fireDistance : 
            DirectionHelper.GetVectorFromDirection(fireDirection) * fireDistance;

        FireProjectileNode node;
        node.useDefault = useDefault;
        node.projRef = projectileRef;
        node.duration = (float)playable.GetDuration();
        node.relativeSpawnPosition = firePosition;
        node.direction = fireDirection;
        _cachedProjectileActionRef.FireProjectileAtPlayer(node);
    }
}
