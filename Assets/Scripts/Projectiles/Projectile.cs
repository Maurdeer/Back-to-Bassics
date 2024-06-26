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
        _hitPlayerPawn = collision.GetComponentInParent<PlayerBattlePawn>();
        if (_hitPlayerPawn == null) return;
        _hitPlayerPawn.ReceiveAttackRequest(this);
        if (!isDestroyed)
        {
            // (TEMP) Manual DEBUG UI Tracker -------
            UIManager.Instance.IncrementMissTracker();
            //---------------------------------------

            _hitPlayerPawn.Damage(_dmg);

            _hitPlayerPawn.CompleteAttackRequest(this);
            Destroy();
        }

    }
    public void OnRequestDeflect(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        if (player == null) return;
        // Did receiver deflect in correct direction?
        if (!DirectionHelper.MaxAngleBetweenVectors(-_rb.velocity, player.SlashDirection, 5f)) return;

        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementParryTracker();
        //---------------------------------------
        _hitPlayerPawn.CompleteAttackRequest(this);
        Destroy();
    }
    public void OnRequestBlock(IAttackReceiver receiver)
    {
        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementBlockTracker();
        //---------------------------------------
        //_hitPlayerPawn.Lurch(_dmg);
        _hitPlayerPawn.CompleteAttackRequest(this);
        Destroy();
    }
    public void OnRequestDodge(IAttackReceiver receiver) 
    { 
        // Nothing Happens Here :o
    }
    public void Destroy()
    {
        isDestroyed = true;
        _hitPlayerPawn = null;
        gameObject.SetActive(false);
    }
}
