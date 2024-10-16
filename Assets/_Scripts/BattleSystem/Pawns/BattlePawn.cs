using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
[DisallowMultipleComponent]
public class BattlePawn : Conductable
{
    [Header("References")]
    [SerializeField] private BattlePawnData _data;
    public BattlePawnData Data => _data;
    protected Animator _pawnAnimator;
    protected PawnSprite _pawnSprite;
    protected ParticleSystem _paperShredBurst;

    [Header("Battle Pawn Data")]
    [SerializeField] protected int _currHP;
    public int HP => _currHP;
    public int MaxHP => _data.HP;

    #region BattlePawn Boolean States
    public bool IsDead { get; private set; }
    public bool IsStaggered { get; private set; }
    #endregion

    // events
    [SerializeField] private UnityEvent onPawnDefeat;
    public event Action OnPawnDeath;
    public event Action OnEnterBattle;
    public event Action OnExitBattle;
    public event Action OnDamage;

    // Extra
    private Coroutine selfStaggerInstance;

    #region Unity Messages
    protected virtual void Awake()
    {
        _currHP = MaxHP;
        _pawnAnimator = GetComponent<Animator>();
        _pawnSprite = GetComponentInChildren<PawnSprite>();
        _paperShredBurst = GetComponentInChildren<ParticleSystem>();
    }
    #endregion
    #region Modification Methods
    public virtual void Damage(int amount)
    {
        if (IsDead) return;
        // Could make this more variable
        if (amount > 0)
        {
            _paperShredBurst.Play();
        }
        _currHP -= amount;
        UIManager.Instance.UpdateHP(this);
        OnDamage?.Invoke();
        if (_currHP <= 0) 
        {
            // Battle Pawn Death
            _currHP = 0;
            IsDead = true;
            // Handling of Death animation and battlemanger Broadcast happen in OnDeath()
            BattleManager.Instance.OnPawnDeath(this);
            OnPawnDeath?.Invoke();
            onPawnDefeat?.Invoke();
            OnDeath();
        }
    }
    public virtual void Heal(int amount)
    {
        if (_currHP < MaxHP)
        {
            _currHP += amount;
            UIManager.Instance.UpdateHP(this);
        }
    }
    public virtual void Stagger()
    {
        selfStaggerInstance = StartCoroutine(StaggerSelf());
    }
    public virtual void UnStagger()
    {
        if (selfStaggerInstance == null) return;

        StopCoroutine(selfStaggerInstance);
        IsStaggered = false;
        OnUnstagger();
    }
    public virtual void ApplyStatusAilment<SA>() 
        where SA : StatusAilment
    {
        gameObject.AddComponent<SA>();
    }
    #endregion
    public virtual void EnterBattle()
    {
        _pawnSprite.Animator.Play("enterbattle");
    }
    public virtual void StartBattle()
    {
        Enable();
        OnEnterBattle?.Invoke();
        UIManager.Instance.UpdateHP(this);
    }
    public virtual void ExitBattle()
    {
        // TODO: Play Some Animation that makes the battle pawn leave the battle
        Disable();
        OnExitBattle?.Invoke();
    }
    #region BattlePawn Messages
    protected virtual void OnStagger()
    {
        // TODO: Things that occur on battle pawn stagger
    }
    protected virtual void OnDeath()
    {
        // TODO: Things that occur on battle pawn death
    }
    protected virtual void OnUnstagger()
    {
        // TODO: Things that occur on battle pawn after unstaggering
    }
    #endregion
    protected virtual IEnumerator StaggerSelf()
    {
        IsStaggered = true;
        OnStagger();
        // TODO: Play Stagger Animation
        // TODO: Notify BattleManager to broadcast this BattlePawn's stagger
        yield return new WaitForSeconds(_data.StaggerDuration);
        //_currSP = _data.SP;
        //UIManager.Instance.UpdateSP(this);
        IsStaggered = false;
        OnUnstagger();
        // TODO: Play StaggerRecovery Animation
    }
}
