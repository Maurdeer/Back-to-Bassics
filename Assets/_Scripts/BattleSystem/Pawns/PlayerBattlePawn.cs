using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static EnemyStateMachine;

/// <summary>
/// Playable Battle Pawn
/// </summary>
[RequireComponent(typeof(PlayerController), typeof(PlayerTraversalPawn))]
public class PlayerBattlePawn : BattlePawn, IAttackRequester, IAttackReceiver
{
    [Header("Player References")]
    [SerializeField] private PlayerWeaponData _weaponData;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private VisualEffect _slashEffect;
    public Transform playerCollider;
    private PlayerTraversalPawn _traversalPawn;
    public PlayerWeaponData WeaponData => _weaponData;
    public Vector2 SlashDirection { get; private set; }
    public Direction DodgeDirection { get; private set; }
    public float AttackDamage { get => _weaponData.Dmg; }
    public bool attacking { get; private set; }
    private bool deflected;
    public bool deflectionWindow { get; private set; }
    public bool dodging { get; set; }
    private ComboManager _comboManager;
    // private Coroutine attackingThread;

    private HashSet<IAttackRequester> ActiveAttacks = new();
    
    protected override void Awake()
    {
        base.Awake();
        _traversalPawn = GetComponent<PlayerTraversalPawn>();
        _comboManager = GetComponent<ComboManager>();
        SlashDirection = Vector2.zero;
    }
    // This will start a battle
    public void EngageEnemy(EnemyBattlePawn enemy)
    {
        BattleManager.Instance.StartBattle(new EnemyBattlePawn[] { enemy });
    }
    #region Player Actions
    public void Dodge(Vector2 direction)
    {
        if (IsDead) return;
        AnimatorStateInfo animatorState = _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0);
        if (!animatorState.IsName("idle")) return;
        // (Past Ryan 1) Figure out a way to make the dodging false later
        // (Past Ryan 2) I'm sorry future ryan, but I have figured it out through very scuffed means
        // Check a file called OnDodgeEnd.cs
        // (Ryan) This really sucky
        // Merge to one state called Open
        DodgeDirection = DirectionHelper.GetVectorDirection(direction);
        updateCombo(false);

        // TODO: refactor coroutine
        StartCoroutine(DodgeThread(DodgeDirection.ToString().ToLower()));
    }
    private IEnumerator DodgeThread(string directionAnimation)
    {
        dodging = true;
        _pawnSprite.Animator.Play("dodge_" + directionAnimation);
        yield return new WaitUntil(() => _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f 
        && _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        dodging = false;
    }
    
    private Conductor.ConductorSchedulable slashHandle;
    private int slashCancelCounter = 0;
    /// <summary>
    /// Slash in a given direction. 
    /// If there are active attack requests, deflect them. 
    /// Otherwise request an attack to the enemy pawn.
    /// </summary>
    /// <param name="slashDirection"></param>
    public void Slash(Vector2 direction)
    {
        if (IsDead) return;

        if (slashHandle != null)
        {
            if (slashCancelCounter > 0) // <-- tweak here for number of cancels allowed
            {
                Debug.LogWarning("Ran out of cancels");
                return;
            }
            
            Debug.Log($"Cancel previous slash");
            slashHandle.SelfAbort();
            slashCancelCounter += 1;
        }
        
        var attackDurationBeats = _weaponData.AttackDuration;
        
        slashHandle = new Conductor.ConductorSchedulable(
            onStarted: (state, contextState) =>
            {
                //AnimatorStateInfo animatorState = _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0);
                //if (!animatorState.IsName("idle")) return;
                _pawnSprite.FaceDirection(new Vector3(direction.x, 0, 1));
                _pawnAnimator.StopPlayback();
                _pawnAnimator.Play($"Slash{DirectionHelper.GetVectorDirection(direction)}", -1, 0);
                _pawnAnimator.speed = contextState.spb;
                _slashEffect.Play();
                // Set the Slash Direction
                AudioManager.Instance.PlayOnShotSound(WeaponData.slashAirSound, transform.position);
                SlashDirection = direction;
                SlashDirection.Normalize();

                BattleManager.Instance.Enemy.ReceiveAttackRequest(this);
                attacking = true;
                deflectionWindow = true;

                // as soon as deflection window starts, clear all dodge-able attacks, considering all of them as "deflected"
                if (ActiveAttacks.Count > 0)
                {
                    ActiveAttacks.RemoveWhere(TryDodgeAttack);
                }
            },
            onUpdate: (state, contextState) => { },
            onCompleted: (state, contextState) =>
            {
                deflectionWindow = false;
                attacking = false;
                _pawnAnimator.Play($"SlashEnd");
                deflected = false;
                slashHandle = null;
                slashCancelCounter = 0;
            },
            onAborted: state =>
            {
                deflectionWindow = false;
                attacking = false;
                deflected = false;
            }
        );
        
        Conductor.Instance.ScheduleActionAsap(attackDurationBeats, Conductor.Instance.Beat, slashHandle, true);
    }
    
    private void updateCombo(bool slash)
    {
        if (slash)
        {
            if (SlashDirection == Vector2.left)
            {
                _comboManager.AppendToCombo('W');
            }
            else if (SlashDirection == Vector2.right)
            {
                _comboManager.AppendToCombo('E');
            }
            else if (SlashDirection == Vector2.up) 
            {
                _comboManager.AppendToCombo('N');
            }
            else if (SlashDirection == Vector2.down) 
            {
                _comboManager.AppendToCombo('S');
            }
        }
        else
        {
            switch(DodgeDirection)
            {
                case Direction.North:
                    _comboManager.AppendToCombo('n');
                    break;
                case Direction.South:
                    _comboManager.AppendToCombo('s');
                    break;
                case Direction.West:
                    _comboManager.AppendToCombo('w');
                    break;
                case Direction.East:
                    _comboManager.AppendToCombo('e');
                    break;
            }
        }
    }
    #endregion
    /// <summary>
    /// Player cannot recover sp while blocking -> Could be brought further upward, in case we have items that use this method...
    /// </summary>
    /// <param name="amount"></param>
    //public override void RecoverSP(float amount)
    //{
    //    // Technically inefficent due to second method call, but good for readablity and modularity!
    //    // o7 sp
    //    if (!blocking && !attacking) base.RecoverSP(amount);
    //}ikjm,,k
    #region IAttackReceiver Methods
    public void ReceiveAttackRequest(IAttackRequester requester)
    {
        if (TryDodgeAttack(requester))
        {
            return;
        }
        
        // Place this requester in the ActiveRequester set (currently disallow two different attacks from the same requester)
        // until completed/aborted callback that removes itself (only evaluates attack upon completion)
        Conductor.ConductorSchedulable handle = new Conductor.ConductorSchedulable(
            onStarted: (state, ctx) => { },
            onUpdate: requester.OnUpdateDuringCoyoteTime,
            onCompleted: (state, ctx) =>
            {
                if (ActiveAttacks.Contains(requester))
                {
                    requester.OnAttackMaterialize(this);
                    ActiveAttacks.Remove(requester);
                }
                else
                {
                    // requester was removed by dodging logic, no need to handle
                }
            },
            onAborted: (state) =>
            {
                if (ActiveAttacks.Contains(requester))
                {
                    ActiveAttacks.Remove(requester);
                }
                else
                {
                    // requester was removed by dodging logic, no need to handle
                }
            });
        
        Conductor.Instance.ScheduleActionAsap(
            requester.GetDeflectionCoyoteTime(), Conductor.Instance.Beat, handle, true);
        
        ActiveAttacks.Add(requester);
    }
    #endregion

    bool TryDodgeAttack(IAttackRequester requester)
    {
        if (/*!deflected && */ deflectionWindow && requester.OnRequestDeflect(this))
        {
            deflected = true;
            AudioManager.Instance.PlayOnShotSound(WeaponData.slashHitSound, transform.position);
            _comboManager.CurrComboMeterAmount += 1;
            return true;
        }
        if (dodging && requester.OnRequestDodge(this))
        {
            return true;
        }
        return false;
    }
    
    #region IAttackRequester Methods

    public void OnAttackMaterialize(IAttackReceiver receiver)
    {
        BattleManager.Instance.Enemy.Damage(_weaponData.Dmg);
        updateCombo(true);
        BattleManager.Instance.Enemy.CompleteAttackRequest(this);
    }

    public float GetDeflectionCoyoteTime()
    {
        throw new NotImplementedException(); // this is intended, play attacks shouldn't be deflected
    }

    public bool OnRequestDeflect(IAttackReceiver receiver)
    {
        throw new NotImplementedException(); // this is intended, play attacks shouldn't be deflected
    }
    public bool OnRequestBlock(IAttackReceiver receiver)
    {
        _pawnSprite.Animator.Play("attack_blocked");
        return true;
    }

    public bool OnRequestDodge(IAttackReceiver receiver)
    {
        return true;
    }
    #endregion
}