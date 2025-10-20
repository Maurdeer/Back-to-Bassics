using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using static EnemyStateMachine;
using static PositionStateMachine;

public class BossAI : Conductable
{
    [Header("Config")]
    [SerializeField] protected EnemyStageData[] _enemyStages;
    [SerializeField] protected bool useDistanceOverBlock;
    protected int _lastAction; // prevents using same attack twice in a row
    protected int _currentStage;
    public event System.Action OnEnemyStageTransition;
    protected int _beatsPerDecision;
    protected int idx;
    // references
    protected EnemyBattlePawn _enemyBattlePawn;
    protected float _decisionTime;
    private void Awake()
    {
        _enemyBattlePawn = GetComponent<EnemyBattlePawn>();

        if (_enemyStages == null || _enemyStages.Length == 0)
        {
            Debug.LogError($"EnemyBattlePawn {_enemyBattlePawn.Data.Name} must contain at least 1 battle stage");
        }

        _lastAction = -1;
        _currentStage = 0;
    }

    protected virtual void Start()
    {
        _enemyBattlePawn.OnPawnDeath += _enemyBattlePawn.Director.Stop;
        _enemyBattlePawn.OnEnterBattle += Enable;
        _enemyBattlePawn.OnExitBattle += Disable;
        _enemyBattlePawn.OnDamage += delegate
        {
            if (_currentStage <= 0) return;
            if (!_enemyBattlePawn.esm.IsOnState<Idle>()) return;
            PreventPlayerAttack();
        };
    }

    protected override void OnFirstBeat()
    {
        base.OnFirstBeat();
        _currentStage = -1;
        PhaseChange();
    }
    protected override void OnFullBeat()
    {
        // (Ryan) Should't need to check for death here, just disable the conducatable conductor connection
        if (Conductor.Instance.Beat <= 1)
        {
            OnFirstBeat();
        }
        PhaseChange();

        if (_enemyBattlePawn.Director.state == PlayState.Playing
            || _enemyBattlePawn.IsDead || _enemyBattlePawn.IsStaggered || DialogueManager.Instance.IsDialogueRunning) return;

        if (_decisionTime > 0)
        {
            // counting down time between attacks
            _decisionTime--;
            return;
        }

        EnemyAttackPattern move = MakeDecision();
        // may want to abstract enemy actions away from just timelines in the future?
        UseMove(move);
        // Reset Stagger Health Only on Stage Request!
        if (_enemyStages[_currentStage].ResetStaggerHealth)
            _enemyBattlePawn.currentStaggerHealth = _enemyBattlePawn.maxStaggerHealth;
    }

    // (Joseph 10/19/2025) Defines default 
    protected virtual EnemyAttackPattern MakeDecision()
    {
        EnemyAttackPattern[] actions = _enemyStages[_currentStage].EnemyAttackPatterns;

        idx = Random.Range(0, actions != null ? actions.Length : 0);
        if (idx == _lastAction)
            // doesnt use same attack twice consecutively
            idx = (idx + 1) % actions.Length;
        _lastAction = idx;

        return actions[idx];
    }
    
    protected virtual void PhaseChange()
    {
        if (_currentStage + 1 < _enemyStages.Length &&
            _enemyStages[_currentStage + 1].HealthThreshold >= (float)_enemyBattlePawn.HP / _enemyBattlePawn.MaxHP)

        {
            _currentStage++;
            // Debug.Log($"Phase: {_currentStage}");
            _beatsPerDecision = _enemyStages[_currentStage].BeatsPerDecision;
            Conductor.Instance.ChangeMusicPhase(_currentStage + 1);
            _enemyBattlePawn.UnStagger();
            PreventPlayerAttack();
            _enemyBattlePawn.maxStaggerHealth = _enemyStages[_currentStage].StaggerHealth;
            _enemyBattlePawn.currentStaggerHealth = _enemyStages[_currentStage].StaggerHealth;
            if (_enemyStages[_currentStage].DialogueNode.Trim() != "")
            {
                DialogueManager.Instance.RunDialogueNode(_enemyStages[_currentStage].DialogueNode);
            }
            if (_enemyStages[_currentStage].PhaseTransitionMove != null)
            {
                // (10/14/25 Joseph) I think this transition animation doesn't work because the boss decides to act immediately 
                UseMove(_enemyStages[_currentStage].PhaseTransitionMove);
            }
            OnEnemyStageTransition?.Invoke();
        }
    }
    
    protected void UseMove(EnemyAttackPattern enemyMove)
    {
         // may want to abstract enemy actions away from just timelines in the future?
        _enemyBattlePawn.interruptable = enemyMove.Interruptable;
        
        _enemyBattlePawn.esm.Transition<Attacking>();
        _enemyBattlePawn.Director.playableAsset = enemyMove.ActionSequence;
        _enemyBattlePawn.Director.Play();
        var handle = _enemyBattlePawn.Director.ScheduleToBeat();

        _decisionTime = _beatsPerDecision;
    }

    private void PreventPlayerAttack()
    {
        if (useDistanceOverBlock && _enemyBattlePawn.psm.IsOnState<Center>())
        {
            _enemyBattlePawn.psm.Transition<Distant>();
        }
        else if (!useDistanceOverBlock)
        {
            _enemyBattlePawn.esm.Transition<Block>();
        }
    }
}

