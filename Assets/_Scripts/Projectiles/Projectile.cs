using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IAttackRequester
{
    [Header("Projectile Specs")]
    [SerializeField] private int _dmg;
    [SerializeField] private int _staggerDamage;
    private float _speed;
    private Rigidbody _rb;
    public bool isDestroyed { get; private set; }
    private PlayerBattlePawn _hitPlayerPawn;
    private EnemyBattlePawn _targetEnemy;
    public float AttackDamage => _dmg;
    public float AttackLurch => _dmg;
    #region Unity Messages
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        Destroy();
    }
    #endregion
    /// <summary>
    /// Spawn a projectile with a particular speed
    /// </summary>
    /// <param name="position"></param>
    /// <param name="velocity"></param>
    public void Fire(Vector3 velocity)
    {
        _rb.velocity = velocity;
        isDestroyed = false;
        gameObject.SetActive(true);
    }
    /// <summary>
    /// Spawn Projectile based on conductor's rule speed
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dir"></param>
    public void Fire(Direction dir)
    {
        _rb.velocity = _speed * DirectionHelper.GetVectorFromDirection(dir);

        // Inefficent as heck, but does the job
        isDestroyed = false;
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider collision)
    {
        _hitPlayerPawn = collision.GetComponent<PlayerBattlePawn>();
        if (_hitPlayerPawn == null) _hitPlayerPawn = collision.GetComponentInParent<PlayerBattlePawn>();
        if (_hitPlayerPawn == null) return;
        if (_hitPlayerPawn.ReceiveAttackRequest(this))
        {
            // (TEMP) Manual DEBUG UI Tracker -------
            UIManager.Instance.IncrementMissTracker();
            //---------------------------------------

            _hitPlayerPawn.Damage(_dmg);

            _hitPlayerPawn.CompleteAttackRequest(this);
            Destroy();
        }

    }
    public bool OnRequestDeflect(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        // Did receiver deflect in correct direction?
        if (player == null 
            ||!DirectionHelper.MaxAngleBetweenVectors(-_rb.velocity, player.SlashDirection, 5f)) 
                return false;

        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementParryTracker();
        //---------------------------------------
        _targetEnemy?.StaggerBuildUp(_staggerDamage);
        Destroy();
        receiver.CompleteAttackRequest(this);
        return true;
    }
    public bool OnRequestBlock(IAttackReceiver receiver)
    {
        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementBlockTracker();
        //---------------------------------------
        //_hitPlayerPawn.Lurch(_dmg);
        Destroy();
        receiver.CompleteAttackRequest(this);
        return true;
    }
    public bool OnRequestDodge(IAttackReceiver receiver) 
    {
        return true;
    }
    public void Destroy()
    {
        isDestroyed = true;
        _hitPlayerPawn = null;
        gameObject.SetActive(false);
    }
    public void SetTargetEnemy(EnemyBattlePawn targetEnemy)
    {
        _targetEnemy = targetEnemy;
        _targetEnemy.OnEnemyStaggerEvent += Destroy;
    }
}
