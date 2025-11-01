using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static EnemyStateMachine;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// Playable Battle Pawn
/// </summary>
[RequireComponent(typeof(PlayerController), typeof(PlayerTraversalPawn))]
public class PlayerBattlePawn : BattlePawn, IAttackRequester, IAttackReceiver
{
    [Header("Player Specifications")]
    [SerializeField] private int deflectsTillHeal = 3;
    [Header("Player References")]
    [SerializeField] private PlayerWeaponData _weaponData;
    public Transform playerCollider;
    [Header("Player Effects")]
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private VisualEffect _slashEffect;
    [SerializeField] private ParticleSystem _deflectEffect;
    [SerializeField] private GameObject _stickWeapon;
    
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
    public ComboManager ComboManager => _comboManager;

    private HashSet<IAttackRequester> ActiveAttacks = new();
    private int currDeflectsTillHeal;
    private bool intiatedDodge;
    protected override void Awake()
    {
        base.Awake();
        _traversalPawn = GetComponent<PlayerTraversalPawn>();
        _comboManager = GetComponent<ComboManager>();
        SlashDirection = Vector2.zero;
        currDeflectsTillHeal = deflectsTillHeal;
    }
    // This will start a battle
    public void EngageEnemy(EnemyBattlePawn enemy)
    {
        BattleManager.Instance.StartBattle(new EnemyBattlePawn[] { enemy });
    }
    #region Player Actions
    public void Dodge(Vector2 direction)
    {
        if (IsDead || dodging) return;

        if (TutorialManager.Instance != null && TutorialManager.Instance.PausedForTutorial)
        {
            if (!TutorialManager.Instance.CheckForDodge || !DirectionHelper.MaxAngleBetweenVectors(direction, TutorialManager.Instance.ExpectedDirection, 5f))
            {
                return;
            }
            else
            {
                TutorialManager.Instance.Unpause();
            }
        }
        
        AnimatorStateInfo animatorState = _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0);
        // if (!animatorState.IsName("idle")) return;
        DodgeDirection = DirectionHelper.GetVectorDirection(direction);
        // Removed For now
        //updateCombo(false);

        // TODO: refactor coroutine
        StartCoroutine(DodgeThread(DodgeDirection.ToString().ToLower()));
    }
    private IEnumerator DodgeThread(string directionAnimation)
    {
        _pawnSprite.Animator.Play("dodge_" + directionAnimation);
        dodging = true;
        yield return new WaitUntil(() => _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName("dodge_" + directionAnimation));
        
        yield return new WaitUntil(() => _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName("idle"));
        dodging = false;
    }

    public override void EnterBattle()
    {
        base.EnterBattle();
        _stickWeapon.SetActive(true);
    }

    public override void ExitBattle()
    {
        base.EnterBattle();
        _stickWeapon.SetActive(false);
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
        if (IsDead || dodging) return;

        if (TutorialManager.Instance != null && TutorialManager.Instance.PausedForTutorial)
        {
            if (!TutorialManager.Instance.CheckForSlash || !DirectionHelper.MaxAngleBetweenVectors(direction, TutorialManager.Instance.ExpectedDirection, 5f))
            {
                return;
            }
            else
            {
                TutorialManager.Instance.Unpause();
            }
        }

        if (slashHandle != null)
        {
            if (slashCancelCounter > 1) // <-- tweak here for number of cancels allowed
            {
                return;
            } 

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
                _pawnSprite.Animator.SetFloat("speed", 1/ contextState.spb);
                _pawnSprite.Animator.Play("slash");
                _pawnAnimator.StopPlayback();
                _pawnAnimator.Play($"Slash{DirectionHelper.GetVectorDirection(direction)}", -1, 0);
                _pawnAnimator.speed = contextState.spb;
                // Set the Slash Direction
                AudioManager.Instance.PlayOnShotSound(WeaponData.slashAirSound, transform.position);
                SlashDirection = direction;
                SlashDirection.Normalize();

                _slashEffect.transform.eulerAngles = new Vector3(0, (SlashDirection.x == 0? _slashEffect.transform.eulerAngles.y : 90 * SlashDirection.x), 90 * SlashDirection.y);
                _slashEffect.Play();
       
                attacking = true;
                deflectionWindow = true;

                // as soon as deflection window starts, clear all dodge-able attacks, considering all of them as "deflected"
                //if (ActiveAttacks.Count > 0)
                //{
                //    ActiveAttacks.RemoveWhere(TryDodgeAttack);
                //}
            },
            onUpdate: (state, contextState) => { },
            onCompleted: (state, contextState) =>
            {
                deflectionWindow = false;
                attacking = false;
                if (!deflected && BattleManager.Instance.Enemy.ReceiveAttackRequest(this))
                {
                    BattleManager.Instance.Enemy.Damage(_weaponData.Dmg);
                    updateCombo(true);
                }
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
                UIManager.Instance.BeatIndicator.AddArrow(Direction.West);
                _comboManager.AppendToCombo('W');
            }
            else if (SlashDirection == Vector2.right)
            {
                UIManager.Instance.BeatIndicator.AddArrow(Direction.East);
                _comboManager.AppendToCombo('E');
            }
            else if (SlashDirection == Vector2.up) 
            {
                UIManager.Instance.BeatIndicator.AddArrow(Direction.North);
                _comboManager.AppendToCombo('N');
            }
            else if (SlashDirection == Vector2.down) 
            {
                UIManager.Instance.BeatIndicator.AddArrow(Direction.South);
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
    #region IAttackReceiver Methods
    public bool ReceiveAttackRequest(IAttackRequester requester)
    {
        if (TryDodgeAttack(requester))
        {
            return false;
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
        
        if (Conductor.Instance.IsPlaying)
        {
            Conductor.Instance.ScheduleActionAsap(
            requester.GetDeflectionCoyoteTime(), Conductor.Instance.Beat, handle, true);
            ActiveAttacks.Add(requester);
        } 

        return true;
    }
    #endregion

    private bool TryDodgeAttack(IAttackRequester requester)
    {
        if (/*!deflected && */ deflectionWindow && requester.OnRequestDeflect(this))
        {
            deflected = true;
            AudioManager.Instance.PlayOnShotSound(WeaponData.slashHitSound, transform.position);
            _deflectEffect.Play();
            _comboManager.CurrComboMeterAmount += 1;
            BattleManager.Instance.AddPlayerMultiplier(1);
            if (--currDeflectsTillHeal <= 0)
            {
                Heal(1);
            }
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
        EnemyBattlePawn enemy = receiver as EnemyBattlePawn;
        if (enemy != null)
        {
            enemy.Damage(_weaponData.Dmg);
            updateCombo(true);
            enemy.CompleteAttackRequest(this);
        }
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

    public override void Damage(int amount)
    {
        if (IsDead) return;
        // Could make this more variable
        if (amount > 0)
        {
            // Shred VFX
            _paperShredBurst?.Play();
            // Reset Deflects Till Heal
            currDeflectsTillHeal = deflectsTillHeal;
            if (!dodging)
            {
                // Any Damage Taken Resets Multiplier
                BattleManager.Instance.ResetPlayerMultiplier();
                _pawnSprite.Animator.Play(IsStaggered ? "staggered_damaged" : "damaged");
            }
        }
        base.Damage(amount);
    }
}