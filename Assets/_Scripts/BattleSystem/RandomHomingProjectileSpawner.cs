using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RandomHomingProjectileSpawner : Conductable
{
    [Header("Projectile Spec")]
    [SerializeField] private GameObject _projectileRef;
    [SerializeField] private int _maxProjectileSpawn;
    [SerializeField] private BoxCollider _spawnMedium;
    [SerializeField] private float _spawnRateInBeats;
    [Header("Target Spec")]
    [SerializeField] private Transform _target;
    [SerializeField] private int _beatsToReachTarget;

    private float _currCooldown;
    protected void Start()
    {
        Enable();
        Pooler.Instance.PoolGameObject(_projectileRef, _maxProjectileSpawn);
    }
    protected override void OnQuarterBeat()
    {
        Projectile proj = Pooler.Instance.NextObjectToPool(_projectileRef).GetComponent<Projectile>();
        if (Conductor.Instance.Beat < _currCooldown || !proj.isDestroyed) return;
        Vector2 spawnLocation = new Vector2
            (
                Random.Range(_spawnMedium.bounds.min.x, _spawnMedium.bounds.max.x),
                Random.Range(_spawnMedium.bounds.min.y, _spawnMedium.bounds.max.y)
            );
        // Thank you Physics 1
        Vector3 r = _target.position - (Vector3)spawnLocation;
        // Vector3 v = r / (_beatsToReachTarget * Conductor.Instance.spb);
        Pooler.Instance.Spawn(_projectileRef, spawnLocation).GetComponent<Projectile>();
        proj.Fire(r, _beatsToReachTarget);

        _currCooldown = Conductor.Instance.Beat + _spawnRateInBeats;
    }
}
