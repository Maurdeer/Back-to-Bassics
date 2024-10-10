using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IAttackRequester
{
    [Header("Projectile Specs")]
    [SerializeField] private int _dmg;
    private float _speed;
    private Rigidbody _rb;
    public bool isDestroyed { get; private set; }
    private PlayerBattlePawn _hitPlayerPawn;
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
    /// Spawn a projectile with a predetermined offset
    /// </summary>
    /// <param name="lifetimeDisplacement">total traversal the projectile should go through in its lifetime</param>
    /// <param name="duration">duration in beats</param>
    public void Fire(Vector3 lifetimeDisplacement, float duration)
    {
        var originalLocation = transform.position;
        
        var schedulable = new Conductor.ConductorSchedulable(
            onStarted: (state, ctxState) =>
            {
                isDestroyed = false;
                gameObject.SetActive(true);
            },
            onUpdate: (state, ctxState) =>
            {
                transform.position = originalLocation + lifetimeDisplacement * state._elapsedProgressCount;
                // _rb.position = originalLocation + lifetimeDisplacement * state._elapsedProgressCount;
                // _rb.velocity = lifetimeDisplacement / ctxState.spb; // current SPB at the update
            },
            onCompleted: (state, ctxState) => { Destroy(); },
            onAborted: (state) => { Destroy(); }
        );
        
        Conductor.Instance.ScheduleActionAsap(duration, Conductor.Instance.Beat, schedulable, forceStart: true);
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
        _hitPlayerPawn.ReceiveAttackRequest(this);
    }

    public void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementMissTracker();
        //---------------------------------------

        _hitPlayerPawn.Damage(_dmg);

        _hitPlayerPawn.CompleteAttackRequest(this);
        Destroy();
    }
    
    public bool OnRequestDeflect(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        // Did receiver deflect in correct direction?
        if (player == null
            || !DirectionHelper.MaxAngleBetweenVectors(-_rb.velocity, player.SlashDirection, 5f))
        {
            return false;
        }

        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementParryTracker();
        //---------------------------------------
        Destroy();
        return true;
    }
    public bool OnRequestBlock(IAttackReceiver receiver)
    {
        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementBlockTracker();
        //---------------------------------------
        //_hitPlayerPawn.Lurch(_dmg);
        Destroy();
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
}
