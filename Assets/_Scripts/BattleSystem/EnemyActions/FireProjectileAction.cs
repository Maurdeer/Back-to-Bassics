using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FireProjectileAction : EnemyAction
{
    [Header("Fire Projectile Action")]
    [SerializeField, Range(1, 100)] private int projectilePoolAmount;
    [SerializeField] private string animationName;
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
    
    public void FireProjectileAtPlayer(FireProjectileNode node)
    {
        // Source of the projectile
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
        proj.Fire((BattleManager.Instance.Player.playerCollider.position - proj.transform.position) / (node.speed * Conductor.Instance.spb));

   

        //EnemyBattlePawn targetEnemy = parentPawnSprite.GetComponent<EnemyBattlePawn>();
        //if (targetEnemy != null)
        //{
        //    proj.SetTargetEnemy(targetEnemy);
        //}
        //else
        //{
        //    Debug.LogError("Failed to get EnemyBattlePawn from parentPawnSprite.");
        //}

        // This Only talors to bassics, not in general
        parentPawnSprite.FaceDirection(new Vector3(-node.relativeSpawnPosition.x, 0, -1));
        //parentPawnSprite.Animator.SetFloat("x_slashDir", -node.relativeSpawnPosition.x);
        parentPawnSprite.Animator.SetFloat("speed", 1 / Conductor.Instance.spb);
        parentPawnSprite.Animator.Play(animationName);
    }
   
}

[SerializeField]
public struct FireProjectileNode
{
    public bool useDefault;
    public GameObject projRef;
    [Tooltip("In Beats")] public float speed;
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
