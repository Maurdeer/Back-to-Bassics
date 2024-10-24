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
    [SerializeField] private EnemyStageData[] _enemyStages;
    private int _lastAction; // prevents using same attack twice in a row
    private int _currentStage;
    public event System.Action OnEnemyStageTransition;
    private int _beatsPerDecision;
    
    // references
    private EnemyBattlePawn _enemyBattlePawn;
    private float _decisionTime;
    private bool staggeredBefore;
    [SerializeField] private UnityEvent firstTimeStagger;
    private void Awake()
    {
        _enemyBattlePawn = GetComponent<EnemyBattlePawn>();

        _lastAction = -1;
        _currentStage = 0;
    }

    private void Start()
    {
        _enemyBattlePawn.OnPawnDeath += _enemyBattlePawn.Director.Stop;
        _enemyBattlePawn.OnEnterBattle += Enable;
        _enemyBattlePawn.OnExitBattle += Disable;
        _enemyBattlePawn.OnDamage += delegate
        { 
            if (_enemyBattlePawn.esm.IsOnState<Idle>() && _enemyBattlePawn.psm.IsOnState<Center>() && _currentStage > 0)
            {
                _enemyBattlePawn.psm.Transition<Distant>();
                //_enemyBattlePawn.esm.Transition<Block>();
            }
        };
        _enemyBattlePawn.OnEnemyStaggerEvent += delegate
        {
            if (!staggeredBefore)
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
        _enemyBattlePawn.currentStaggerHealth = _enemyBattlePawn.maxStaggerHealth;
        _enemyBattlePawn.esm.Transition<Attacking>();
        _enemyBattlePawn.Director.playableAsset = actions[idx].ActionSequence;
        _enemyBattlePawn.Director.Play();
        var handle = _enemyBattlePawn.Director.ScheduleToBeat();

        _decisionTime = _beatsPerDecision;
    }
    protected void PhaseChange()
    {
        if (_currentStage + 1 < _enemyStages.Length &&
            _enemyStages[_currentStage + 1].HealthThreshold >= (float)_enemyBattlePawn.HP / _enemyBattlePawn.MaxHP)

        {
            _currentStage++;
            // Debug.Log($"Phase: {_currentStage}");
            _beatsPerDecision = _enemyStages[_currentStage].BeatsPerDecision;
            Conductor.Instance.ChangeMusicPhase(_currentStage + 1);
            _enemyBattlePawn.UnStagger();
            _enemyBattlePawn.psm.Transition<Distant>();
            _enemyBattlePawn.maxStaggerHealth = _enemyStages[_currentStage].StaggerHealth;
            _enemyBattlePawn.currentStaggerHealth = _enemyStages[_currentStage].StaggerHealth;
            if (_enemyStages[_currentStage].DialogueNode.Trim() != "")
            {
                DialogueManager.Instance.RunDialogueNode(_enemyStages[_currentStage].DialogueNode);
            }
            
            OnEnemyStageTransition?.Invoke();
        }
    }
}

