using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TreeEditor;
using UnityEngine;

public class FireProjectileAction : EnemyAction
{
    [Header("Fire Projectile Action")]
    [SerializeField, Range(1, 100)] private int projectilePoolAmount;
    [SerializeField] protected string enterFireName;
    [SerializeField] protected string animationName;
    [SerializeField] protected string exitFireName;
    [SerializeField] private bool performAnimationBeforeFire;
    [Header("Default Projectile Settings")]
    [SerializeField] private FirePatternChoice firePattern;
    [SerializeField] private GameObject[] projectileFabs;

    protected Coroutine currentlyFiring;
    protected float timeElapsed;
    private FireProjectileNode prevNode;

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
            if (currentlyFiring != null)
            {
                StopCoroutine(currentlyFiring);
                timeElapsed = Conductor.Instance.Beat - timeElapsed;
                LaunchProjectile(prevNode, prevNode.duration - timeElapsed);
            }
            prevNode = node;
            currentlyFiring = StartCoroutine(AnimateBeforeFire(node));
            return;
        }

        LaunchProjectile(node, node.duration);
        AnimateProjectileLaunch(node); 
    }
    protected virtual void AnimateProjectileLaunch(FireProjectileNode node)
    {
        parentPawnSprite.FaceDirection(new Vector3(node.relativeSpawnPosition.x, 0, -1));
        parentPawnSprite.Animator.SetFloat("speed", 1 / Conductor.Instance.spb);
        AnimatorStateInfo animStateInfo = parentPawnSprite.Animator.GetCurrentAnimatorStateInfo(0);
        if (enterFireName.Trim() != "" && exitFireName.Trim() != ""
            && (animStateInfo.IsName(enterFireName)
            || animStateInfo.IsName(animationName)
            || animStateInfo.IsName(exitFireName)))
        {
            parentPawnSprite.Animator.Play(animationName);
        }
        else
        {
            parentPawnSprite.Animator.Play(enterFireName);
        }
    }
    protected void LaunchProjectile(FireProjectileNode node, float duration)
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

        // Define the offset for the purposes of making things more visually cohesive
        // This defines the adjustment scaling, so not actually all that useful. Maybe use the base player transform?
        // Nope. Just physically cast the transforms at this point
        // Double check to make sure that the total displacement remains the same;
        Vector3 offset = new Vector3(0, 0, 0);

        switch (node.direction)
        {
            case Direction.North:
                offset.y = 1.6f;
                break;
            case Direction.South:
                offset.y = -1.6f;
                break;
            case Direction.East:
                offset.x = 0.6f;
                break;
            case Direction.West:
                offset.x = -0.6f;
                break;
        }

        proj.transform.position = BattleManager.Instance.Player.playerCollider.position + offset + node.relativeSpawnPosition;
        proj.Fire(BattleManager.Instance.Player.playerCollider.position + offset - proj.transform.position, duration);

        // proj.transform.position = BattleManager.Instance.Player.playerCollider.position + node.relativeSpawnPosition;        
        // proj.Fire(BattleManager.Instance.Player.playerCollider.position - proj.transform.position, node.duration);
    }

    protected virtual IEnumerator AnimateBeforeFire(FireProjectileNode node)
    {
        AnimateProjectileLaunch(node);
        timeElapsed = Conductor.Instance.Beat;
        string exit_name = exitFireName.Trim() == "" ? "idle_battle" : exitFireName;
        yield return new WaitUntil(() => parentPawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName(exit_name) && !parentPawnSprite.Animator.IsInTransition(0));
        timeElapsed = Conductor.Instance.Beat - timeElapsed;
        LaunchProjectile(node, node.duration - timeElapsed);
        currentlyFiring = null;
    }
}

[SerializeField]
public struct FireProjectileNode
{
    public bool useDefault;
    public GameObject projRef;
    [Tooltip("In Beats")] public float duration;
    public Vector3 relativeSpawnPosition;
    public Direction direction;
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
