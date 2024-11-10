using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FireProjectileAction : EnemyAction
{
    [Header("Fire Projectile Action")]
    [SerializeField, Range(1, 100)] private int projectilePoolAmount;
    [SerializeField] private string animationName;
    [SerializeField] private bool performAnimationBeforeFire;
    [Header("Default Projectile Settings")]
    [SerializeField] private FirePatternChoice firePattern;
    [SerializeField] private GameObject[] projectileFabs;

    private int idx;
    private GameObject fabSelection;
    //amount of built stagger on successful deflect
    private int _staggerDamage = 15;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="proj">Reference to Projectile to Fire</param>
    /// <param name="speed">In Beats</param>
    /// <param name="position"> Relative to player</param>
    private void Start()
    {
        if (!parentPawnSprite.Animator.HasState(0, Animator.StringToHash(animationName)))
        {
            Debug.LogError($"Animation State \"{animationName}\" for FireProjectileAction does not exist.");
        }
    }
    public void FireProjectileAtPlayer(FireProjectileNode node)
    {

        if (performAnimationBeforeFire)
        {
            StartCoroutine(AnimateBeforeFire(node));
            return;
        }

        LaunchProjectile(node);
        AnimateProjectileLaunch(node); 
    }
    private void AnimateProjectileLaunch(FireProjectileNode node)
    {
        parentPawnSprite.FaceDirection(new Vector3(node.relativeSpawnPosition.x, 0, -1));
        parentPawnSprite.Animator.SetFloat("speed", 1 / Conductor.Instance.spb);
        parentPawnSprite.Animator.Play(animationName);
    }
    private void LaunchProjectile(FireProjectileNode node)
    {
        GameObject objRef = null;
        if (node.useDefault)
        {
            if (projectileFabs == null || projectileFabs.Length == 0)
            {
                Debug.LogError("Couldn't Fire Default Projectiles, no default fabs were set.");
                return;
            }

            switch (firePattern)
            {
                case FirePatternChoice.Linear:
                    fabSelection = projectileFabs[idx];
                    idx = (idx + 1) % projectileFabs.Length;
                    break;
                case FirePatternChoice.RandomUniform:
                    idx = Random.Range(0, projectileFabs.Length);
                    fabSelection = projectileFabs[idx];
                    break;
                case FirePatternChoice.RandomWeighted:
                    objRef = null;
                    break;
                default:
                    break;
            }
            if (fabSelection == null)
            {
                Debug.LogError($"Couldn't Fire Default Projectile from index {idx}");
                return;
            }
        }
        else
        {
            fabSelection = node.projRef;
            if (fabSelection == null)
            {
                Debug.LogError($"Projectile Asset Node has no reference to a projectile prefab.");
                return;
            }
        }
        objRef = Pooler.Instance.Pool(fabSelection);
        if (objRef == null)
        {
            Pooler.Instance.PoolGameObject(fabSelection, projectilePoolAmount);
            objRef = Pooler.Instance.Pool(fabSelection);
        }
        Projectile proj = objRef.GetComponent<Projectile>();
        proj.SetTargetEnemy(parentPawn);
        proj.transform.position = BattleManager.Instance.Player.playerCollider.position + node.relativeSpawnPosition;
        // Debug.Log($"{proj}: {proj.transform.position}");
        proj.Fire(BattleManager.Instance.Player.playerCollider.position - proj.transform.position, node.duration);
    }

    private IEnumerator AnimateBeforeFire(FireProjectileNode node)
    {
        AnimateProjectileLaunch(node);
        yield return new WaitUntil(() => parentPawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_battle"));
        LaunchProjectile(node);
    }
}

[SerializeField]
public struct FireProjectileNode
{
    public bool useDefault;
    public GameObject projRef;
    [Tooltip("In Beats")] public float duration;
    public Vector3 relativeSpawnPosition;
}
public enum ProjectileSourceChoice
{
    UseDefault = 0,
    Custom
}
public enum FirePatternChoice
{
    None = 0,
    Linear,
    RandomUniform,
    RandomWeighted
    
}
