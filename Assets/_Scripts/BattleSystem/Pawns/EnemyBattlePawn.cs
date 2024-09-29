using System;
using System.Collections.Generic;
using UnityEngine;
using static EnemyStateMachine;
using static PositionStateMachine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;

/// <summary>
/// Manipulate by an external class, not within the class!!
/// </summary>
public class EnemyBattlePawn : BattlePawn, IAttackReceiver
{
    [field: Header("Enemy References")]
    [field: SerializeField] public EnemyStateMachine esm { get; private set; }
    [field: SerializeField] public PositionStateMachine psm { get; private set; }
    //[SerializeField] private ParticleSystem _particleSystem;
    [field: SerializeField] public Transform targetFightingLocation { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera battleCam { get; private set; }
    public EnemyBattlePawnData EnemyData => (EnemyBattlePawnData)Data;
    private Dictionary<Type, EnemyAction> _enemyActions = new Dictionary<Type, EnemyAction>();

    // Events
    //public event Action OnEnemyActionComplete; --> Hiding for now
    protected override void Awake()
    { 
        if (Data.GetType() != typeof(EnemyBattlePawnData))
        {
            Debug.LogError($"Enemy Battle Pawn \"{Data.name}\" is set incorrectly");
            return;
        }
        esm = GetComponent<EnemyStateMachine>();
        if (esm == null)
        {
            Debug.LogError($"Enemy Battle Pawn \"{Data.name}\" is must have an EnemyStateMachine");
            return;
        }
        psm = GetComponent<PositionStateMachine>();
        if (psm == null)
        {
            Debug.LogError($"Enemy Battle Pawn \"{Data.name}\" is must have a PositionStateMachine");
            return;
        }
        base.Awake();
    }
    public EA GetEnemyAction<EA>() where EA : EnemyAction
    {
        return _enemyActions[typeof(EA)] as EA;
    }
    public void AddEnemyAction(EnemyAction action)
    {
        if (_enemyActions.ContainsKey(action.GetType()))
        {
            Debug.LogError($"Enemy Action {action.GetType()} is redundantly referenced, only one should be.");
            return;
        }
        _enemyActions[action.GetType()] = action;
    }
    /// <summary>
    /// Select from some attack i to perform, and then provide a direction if the attack has variants based on this
    /// </summary>
    /// <param name="i"></param>
    /// <param name="dir"></param>
    //public void PerformBattleAction(int i)
    //{
    //    if (i >= _enemyActions.Length)
    //    {
    //        Debug.Log("Non Existent index call for batlle action!");
    //        return;
    //    }
    //    _esm.Transition<EnemyStateMachine.Attacking>();
    //    _enemyActions[i].StartAction();
    //    _actionIdx = i;
    //}
    #region IAttackReceiver Methods
    public virtual bool ReceiveAttackRequest(IAttackRequester requester, Action onPendingSuccess = null, Action onPendingFail = null)
    {
        if (esm.IsOnState<Dead>() || psm.IsOnState<Distant>()) return false;
        return esm.CurrState.AttackRequestHandler(requester);
    }
    public virtual void CompleteAttackRequest(IAttackRequester requester)
    {
        // Does nothing for now, not waranted an exception!
    }
    #endregion
    #region BattlePawn Overrides
    public override void Damage(int amount)
    {
        amount = esm.CurrState.OnDamage(amount);
        base.Damage(amount);  
    }
    //public override void Lurch(float amount)
    //{
    //    if (IsStaggered) return;
    //    amount = _esm.CurrState.OnLurch(amount);
    //    base.Lurch(amount);
    //}
    protected override void OnStagger()
    {
        if (esm.IsOnState<Dead>()) return;
        base.OnStagger();
        // Staggered Animation (Paper Crumple)
        esm.Transition<Stagger>();
        //_particleSystem?.Play();
    }
    protected override void OnUnstagger()
    {
        if (esm.IsOnState<Dead>()) return;
        base.OnUnstagger();
        // Unstagger Animation transition to idle
        esm.Transition<Idle>();
        //_particleSystem?.Stop();
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        esm.Transition<Dead>();
        //_particleSystem?.Stop();
    }
    // This could get used or not, was intended for random choices :p
    //public void OnActionComplete()
    //{
    //    OnEnemyActionComplete?.Invoke();
    //    if (esm.IsOnState<Dead>() || esm.IsOnState<Stagger>()) return;
    //    esm.Transition<Idle>();
    //}
    #endregion
}
