using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private PlayableDirector _director;
    private float _decisionTime;
    
    private void Awake()
    {
        _enemyBattlePawn = GetComponent<EnemyBattlePawn>();
        _director = GetComponent<PlayableDirector>();
        _enemyBattlePawn.OnEnemyStaggerEvent += _director.Stop;
        
        if (_director == null)
        {
            Debug.LogError($"Enemy Battle Pawn \"{_enemyBattlePawn.Data.name}\" has no playable director referenced!");
            return;
        }

        _lastAction = -1;
        _currentStage = 0;
        _beatsPerDecision =  _enemyStages[_currentStage].BeatsPerDecision;
        _enemyBattlePawn.StaggerArmor = _enemyStages[_currentStage].StaggerArmor;
    }

    private void Start()
    {
        _enemyBattlePawn.OnPawnDeath += _director.Stop;
        _enemyBattlePawn.OnEnterBattle += Enable;
        _enemyBattlePawn.OnExitBattle += Disable;
        _enemyBattlePawn.OnDamage += delegate
        { 
            if (_enemyBattlePawn.esm.IsOnState<Idle>() && _enemyBattlePawn.psm.IsOnState<Center>() && _currentStage > 0)
            {
                // _enemyBattlePawn.psm.Transition<Distant>();
                _enemyBattlePawn.esm.Transition<Block>();
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

        if (_director.state == PlayState.Playing 
            || _enemyBattlePawn.IsDead || _enemyBattlePawn.IsStaggered || DialogueManager.Instance.IsDialogueRunning) return;
        
        if (_decisionTime > 0) {
            // counting down time between attacks
            _decisionTime--;
            return;
        }
            
        TimelineAsset[] actions = _enemyStages[_currentStage].EnemyActionSequences;
        
        int idx = Random.Range(0, actions != null ? actions.Length : 0);
        if (idx == _lastAction)
        // doesnt use same attack twice consecutively
            idx = (idx + 1) % actions.Length;
        _lastAction = idx;

        // may want to abstract enemy actions away from just timelines in the future?
        _enemyBattlePawn.CurrentStaggerHealth = _enemyBattlePawn.EnemyData.StaggerHealth;
        _enemyBattlePawn.esm.Transition<Attacking>();
        _director.playableAsset = actions[idx];
        _director.Play();
        var handle = _director.ScheduleToBeat();

        _decisionTime = _beatsPerDecision;
    }
    protected void PhaseChange()
    {
        if (_currentStage + 1 < _enemyStages.Length &&
            _enemyStages[_currentStage + 1].HealthThreshold >= (float)_enemyBattlePawn.HP / _enemyBattlePawn.MaxHP)

        {
            _currentStage++;
            //Debug.Log($"Phase: {_currentStage}");
            _beatsPerDecision = _enemyStages[_currentStage].BeatsPerDecision;
            Conductor.Instance.ChangeMusicPhase(_currentStage + 1);
            _enemyBattlePawn.UnStagger();
            _enemyBattlePawn.psm.Transition<Distant>();
            _enemyBattlePawn.StaggerArmor = _enemyStages[_currentStage].StaggerArmor;
            if (DialogueManager.Instance.RunDialogueNode(_enemyStages[_currentStage].DialogueNode))
            {
                //Debug.Log("Dialogue is running");
            }
            else
            {
                //Debug.Log("Dialogue is not running");
            }
            OnEnemyStageTransition?.Invoke();
        }
    }
}

