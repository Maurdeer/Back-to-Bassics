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

    // references
    protected EnemyBattlePawn _enemyBattlePawn;
    protected float _decisionTime;
    protected bool staggeredBefore;
    [SerializeField] protected UnityEvent firstTimeStagger;
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

    protected void Start()
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
        _enemyBattlePawn.OnEnemyStaggerEvent += delegate
        {
            // (Joseph 1 / 11 / 2025) Modifying this to account for Bassic's behavior in the tutorial
            // (10/14/2025 Joseph) No need to repeat dialogue, as the dialogue should persist during the tutorial until the player advances. 
            if (_currentStage == 0 && !staggeredBefore)
            {
                staggeredBefore = true;
                firstTimeStagger.Invoke();
            }
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

        // (Joseph 1 / 11 / 2025) Was running into a stack overflow issue when calling firstStagger through checking enemy stages and making sure it was the first one.
        // I'm going to try to make it so it's only called every beat, and if 
        staggeredBefore = false;

        if (_enemyBattlePawn.Director.state == PlayState.Playing 
            || _enemyBattlePawn.IsDead || _enemyBattlePawn.IsStaggered || DialogueManager.Instance.IsDialogueRunning) return;
        
        if (_decisionTime > 0) {
            // counting down time between attacks
            _decisionTime--;
            return;
        }
            
        EnemyAttackPattern[] actions = _enemyStages[_currentStage].EnemyAttackPatterns;
        
        int idx = Random.Range(0, actions != null ? actions.Length : 0);
        if (idx == _lastAction)
        // doesnt use same attack twice consecutively
            idx = (idx + 1) % actions.Length;
        _lastAction = idx;

        // may want to abstract enemy actions away from just timelines in the future?
        _enemyBattlePawn.interruptable = actions[idx].Interruptable;
        // Reset Stagger Health Only on Stage Request!
        if (_enemyStages[_currentStage].ResetStaggerHealth)
            _enemyBattlePawn.currentStaggerHealth = _enemyBattlePawn.maxStaggerHealth;
        _enemyBattlePawn.esm.Transition<Attacking>();
        _enemyBattlePawn.Director.playableAsset = actions[idx].ActionSequence;
        _enemyBattlePawn.Director.Play();
        var handle = _enemyBattlePawn.Director.ScheduleToBeat();

        _decisionTime = _beatsPerDecision;
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
            if (!string.IsNullOrEmpty(_enemyStages[_currentStage].PhaseTransitionAnimation)) {
                _enemyBattlePawn.PlayTransitionAnimation(_enemyStages[_currentStage].PhaseTransitionAnimation);
                Debug.Log(_enemyStages[_currentStage].PhaseTransitionAnimation);
            }
            OnEnemyStageTransition?.Invoke();
        }
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

