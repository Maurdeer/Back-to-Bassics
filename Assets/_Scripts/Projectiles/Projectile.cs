using FMODUnity;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IAttackRequester
{
    [Header("Projectile Specs")]
    [SerializeField] private int _dmg;
    [SerializeField] private int _staggerDamage;
    private float _speed;
    private Rigidbody _rb;
    public bool isDestroyed { get; protected set; }
    private EnemyBattlePawn _targetEnemy;
    public float AttackDamage => _dmg;
    public float AttackLurch => _dmg;
    protected Vector3 _initialScale;
    #region Unity Messages
    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _initialScale = transform.localScale;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _burstEffect = GetComponent<ParticleSystem>();
        Reset();
    }
    #endregion

    [SerializeField] private EventReference playOnMiss;
    private float coyoteTimer = 0;
    protected Vector3 _slashDirection;
    private ParticleSystem _burstEffect;
    protected SpriteRenderer _spriteRenderer;
    protected MeshRenderer _meshRenderer;
    protected Conductor.ConductorSchedulable activeScheduable;
    
    /// <summary>
    /// Spawn a projectile with a predetermined offset
    /// </summary>
    /// <param name="lifetimeDisplacement">total traversal the projectile should go through in its lifetime</param>
    /// <param name="duration">duration in beats</param>
    public virtual void Fire(Vector3 lifetimeDisplacement, float duration)
    {
        var originalLocation = transform.position;
        _slashDirection = -lifetimeDisplacement;
        transform.localScale = _initialScale;
        activeScheduable = new Conductor.ConductorSchedulable(
            onStarted: (state, ctxState) =>
            {
                isDestroyed = false;
                gameObject.SetActive(true);
                if (_spriteRenderer != null) _spriteRenderer.enabled = true;
                if (_meshRenderer != null) _meshRenderer.enabled = true;
            },
            onUpdate: (state, ctxState) =>
            {
                transform.position = originalLocation + lifetimeDisplacement * state._elapsedProgressCount;
                // _rb.position = originalLocation + lifetimeDisplacement * state._elapsedProgressCount;
                // _rb.velocity = lifetimeDisplacement / ctxState.spb; // current SPB at the update
            },
            onCompleted: (state, ctxState) =>
            {
                BattleManager.Instance.Player.ReceiveAttackRequest(this);
            },
            onAborted: (state) => { 

            }
        );
        
        Conductor.Instance.ScheduleActionAsap(duration, Conductor.Instance.Beat, activeScheduable, forceStart: true);
    }

    public virtual void OnAttackMaterialize(IAttackReceiver receiver)
    {
        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementMissTracker();
        BattleManager.Instance.Player.Damage(_dmg);
        FMODUnity.RuntimeManager.PlayOneShot(playOnMiss);
        Reset();
    }

    public float GetDeflectionCoyoteTime()
    {
        return 0.5f;
    }

    public void OnUpdateDuringCoyoteTime(Conductor.ConductorSchedulableState state, Conductor.ConductorContextState ctx)
    {
        transform.localScale = Vector3.one * (1 - state._elapsedProgressCount);
        coyoteTimer = GetDeflectionCoyoteTime() * state._elapsedProgressCount;
    }

    public virtual bool OnRequestDeflect(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        // Did receiver deflect in correct direction?
        if (player == null
            || !DirectionHelper.MaxAngleBetweenVectors(_slashDirection, player.SlashDirection, 5f))
        {
            Reset();
            return false;
        }

        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementParryTracker();
        if (coyoteTimer > 0)
        {
            //Debug.Log($"Note deflected after impact at +{coyoteTimer} beats");
        }
        else
        {
            //Debug.Log($"Note deflected by ongoing slash");
        }

        //---------------------------------------
        _targetEnemy?.StaggerDamage(_staggerDamage);
        Reset();

        return true;
    }
    public bool OnRequestBlock(IAttackReceiver receiver)
    {
         // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementBlockTracker();
        //---------------------------------------
        Reset();

        return true;
    }
    public bool OnRequestDodge(IAttackReceiver receiver) 
    {
        Reset();
        return true;
    }
    public void Reset()
    {
        isDestroyed = true;
        _burstEffect?.Play();
        activeScheduable?.SelfAbort();
        gameObject.SetActive(false);
        if (_spriteRenderer != null) _spriteRenderer.enabled = false;
        if (_meshRenderer != null) _meshRenderer.enabled = false;
    }
    public void SetTargetEnemy(EnemyBattlePawn targetEnemy)
    {
        _targetEnemy = targetEnemy;
        _targetEnemy.OnEnemyStaggerEvent += Reset;
    }
}
