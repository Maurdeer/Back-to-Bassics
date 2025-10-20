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
    public PawnSprite PawnSprite => _pawnSprite;

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
    [SerializeField] protected UnityEvent onPawnExitBattle;
    public event Action OnPawnDeath;
    public event Action OnEnterBattle;
    public event Action OnExitBattle;
    public event Action OnDamage;

    // Extra
    protected Coroutine selfStaggerInstance;
    private int _maxRecordedDurationForStagger = 0; //(Joseph 1 / 13 / 25) Attempt at implementing staggerFor better
    [SerializeField] private GameObject tutComboList; //(Joseph 1 / 13 / 25) Hacky Implementation, will try to fix later

    #region Unity Messages
    protected virtual void Awake()
    {
        _currHP = MaxHP;
        _pawnAnimator = GetComponent<Animator>();
        _pawnSprite = GetComponentInChildren<PawnSprite>();
        _paperShredBurst = transform.Find("ShreddedPaperParticles").GetComponent<ParticleSystem>();
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
        // (Joseph 1 / 12 / 25) Trying to address the issue of StaggerFor in BossAi being called and being useless
        // Debug.Log("IsStaggered is " + IsStaggered);
        // IsStaggered = true;
        if (selfStaggerInstance != null)  
        {
            StopCoroutine(selfStaggerInstance);
        } // (Joseph 1 / 12 / 25) I'm not sure this line is doing anything.
          // It doesn't. In the context of the issue we're trying to address, both get called at around the same time and thus selfStaggerInstance is always null.
          // if (IsStaggered) return; // (Joseph 1 / 12 / 25) This solution doesn't work because normal stagger is called first, meaning the second doesn't get registered.
        IsStaggered = true;
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

        // Change this for On Victory
        if (_currHP <= 0)
            onPawnExitBattle?.Invoke();
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
        if (tutComboList != null) {
            _maxRecordedDurationForStagger += 1;
            if (_maxRecordedDurationForStagger >= 1) {
                tutComboList.SetActive(false);
                _maxRecordedDurationForStagger = 0;
            } 
        }
        
    }
    #endregion
    protected virtual IEnumerator StaggerSelf(float duration)
    {
        // if (duration < _maxRecordedDurationForStagger) yield break;
        // else {
        //     Debug.Log("Duration");
        //     _maxRecordedDurationForStagger = duration;
        // }
        yield return null; // <----- Fuck you unity Fuck you
        Debug.Log("Staggering for " + duration + " seconds");
        List<Coroutine> completionThreads = OnStagger();
        foreach(Coroutine thread in completionThreads)
        {
            yield return thread;
        }
        //yield return new WaitUntil(() => _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName("idle_battle") 
        //                            || _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName("idle")
        //                            || _pawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName("staggered"));
        
        if (!_pawnSprite.Animator.GetCurrentAnimatorStateInfo(0).IsName("staggered")) _pawnSprite.Animator.Play("stagger");
        _staggerVFX?.Play();
        // TODO: Notify BattleManager to broadcast this BattlePawn's stagger
        // Debug.Log("StaggeredFor" + duration);
        
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
