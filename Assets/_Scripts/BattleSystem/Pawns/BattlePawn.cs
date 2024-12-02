using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Threading;

[DisallowMultipleComponent]
public class BattlePawn : Conductable
{
    [Header("References")]
    [SerializeField] private BattlePawnData _data;
    public BattlePawnData Data => _data;
    protected Animator _pawnAnimator;
    protected PawnSprite _pawnSprite;
    protected ParticleSystem _paperShredBurst;
    protected ParticleSystem _staggerVFX;

    [Header("Battle Pawn Data")]
    [SerializeField] protected int _currHP;
    public int HP => _currHP;
    public int MaxHP => _data.HP;

    #region BattlePawn Boolean States
    public bool IsDead { get; protected set; }
    public bool IsStaggered { get; protected set; }
    #endregion

    // events
    [SerializeField] protected UnityEvent onPawnDefeat;
    public event Action OnPawnDeath;
    public event Action OnEnterBattle;
    public event Action OnExitBattle;
    public event Action OnDamage;

    // Extra
    protected Coroutine selfStaggerInstance;

    #region Unity Messages
    protected virtual void Awake()
    {
        _currHP = MaxHP;
        _pawnAnimator = GetComponent<Animator>();
        _pawnSprite = GetComponentInChildren<PawnSprite>();
        _paperShredBurst = GameObject.Find("ShreddedPaperParticles").GetComponent<ParticleSystem>();
        _staggerVFX = transform.Find("StaggerVFX")?.GetComponent<ParticleSystem>();
    }
    #endregion
    #region Modification Methods
    public virtual void Damage(int amount)
    {
        if (IsDead) return;
        _currHP -= amount;
        UIManager.Instance.UpdateHP(this);
        OnDamage?.Invoke();
        if (_currHP <= 0) 
        {
            // Battle Pawn Death
            _currHP = 0;
            IsDead = true;
            // Handling of Death animation and battlemanger Broadcast happen in OnDeath()
            UnStagger();
            BattleManager.Instance.OnPawnDeath(this);
            OnPawnDeath?.Invoke();
            onPawnDefeat?.Invoke();
            OnDeath();
        }
    }
    public virtual void Heal(int amount)
    {
        _currHP += amount;
        if (_currHP > MaxHP) _currHP = MaxHP;
        UIManager.Instance.UpdateHP(this);
    }
    public virtual void Stagger()
    {
        StaggerFor(_data.StaggerDuration);
    }
    public virtual void StaggerFor(float duration)
    {
        if (selfStaggerInstance != null) StopCoroutine(selfStaggerInstance);
        selfStaggerInstance = StartCoroutine(StaggerSelf(duration));
    }
    public virtual void UnStagger()
    {
        if (selfStaggerInstance == null) return;

        StopCoroutine(selfStaggerInstance);
        _staggerVFX?.Stop();
        _staggerVFX?.Clear();
        IsStaggered = false;
        OnUnstagger();
        _pawnSprite.Animator.Play("recover");
    }
    public virtual void ApplyStatusAilment<SA>() 
        where SA : StatusAilment
    {
        if (gameObject.GetComponent<SA>() != null) return;
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
    protected virtual List<Coroutine> OnStagger()
    {
        // TODO: Things that occur on battle pawn stagger
        return null;
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
    protected virtual IEnumerator StaggerSelf(float duration)
    {
        IsStaggered = true;
        List<Coroutine> completionThreads = OnStagger();
        foreach(Coroutine thread in completionThreads)
        {
            yield return thread;
        }
        _pawnSprite.Animator.Play("stagger");
        _staggerVFX?.Play();
        // TODO: Notify BattleManager to broadcast this BattlePawn's stagger
        yield return new WaitForSeconds(duration);
        _staggerVFX?.Stop();
        _staggerVFX?.Clear();
        //_currSP = _data.SP;
        //UIManager.Instance.UpdateSP(this);
        IsStaggered = false;
        OnUnstagger();
        _pawnSprite.Animator.Play("recover");
    }
}
