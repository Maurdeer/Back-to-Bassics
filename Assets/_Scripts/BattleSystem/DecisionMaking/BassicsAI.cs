using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyStateMachine;
using static PositionStateMachine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


// stage data class
// has health threshold for entering
// text said when entering
// list of moves that can be used
public class BassicsAI : Conductable
{
    [Header("Config")]
    [SerializeField] private int _beatsPerDecision;
    [SerializeField] private EnemyStageData[] _enemyStages;
    private int _lastAction; // prevents using same attack twice in a row
    private int _currentStage;
    public event System.Action OnEnemyStageTransition;

    
    // references
    private EnemyBattlePawn _bassics;
    private PlayableDirector _director;
    private float _decisionTime;
    private void Awake()
    {
        _bassics = GetComponent<EnemyBattlePawn>();
        _director = GetComponent<PlayableDirector>();
        _bassics.OnEnemyStaggerEvent += _director.Stop;
        
        if (_director == null)
        {
            Debug.LogError($"Enemy Battle Pawn \"{_bassics.Data.name}\" has no playable director referenced!");
            return;
        }

        _lastAction = -1;
        _currentStage = 0;
    }
    private void Start()
    {
        _bassics.OnPawnDeath += _director.Stop;
        //_bassics.OnEnemyActionComplete += delegate 
        //{ 
        //    _decisionTime = Conductor.Instance.Beat + _beatsPerDecision; 
        //};
        _bassics.OnEnterBattle += Enable;
        _bassics.OnExitBattle += Disable;
        _bassics.OnDamage += delegate
        { 
            if (_bassics.esm.IsOnState<Idle>() && _bassics.psm.IsOnState<Center>())
            {
                _bassics.psm.Transition<Distant>();
            }
        };
    }
    // Perform Random Battle Action --> This is not the way this should be done
    protected override void OnFullBeat()
    {
        // (Ryan) Should't need to check for death here, just disable the conducatable conductor connection 
        //if (Conductor.Instance.Beat < _decisionTime || (_enemyActions != null && _enemyActions[_actionIdx].IsActive) || IsDead) return;
        if (Conductor.Instance.Beat < _decisionTime 
            || _director.state == PlayState.Playing 
            || _bassics.IsDead || _bassics.IsStaggered) return;
        if (_currentStage+1 < _enemyStages.Length && 
            _enemyStages[_currentStage+1].HealthThreshold > (float)_bassics.HP/_bassics.MaxHP) {
                _currentStage++;
                OnEnemyStageTransition?.Invoke();
            }
            
        TimelineAsset[] actions = _enemyStages[_currentStage].EnemyActionSequences;
        
        int idx = Random.Range(0, (actions != null ? actions.Length : 0) + 4) - 4;
        // int idx = Random.Range(0, actions != null ? actions.Length : 0);
        if (idx == _lastAction)
            idx = (idx + 1) % actions.Length;
        _lastAction = idx;
        //int idx = UnityEngine.Random.Range(0, 4);
        if (idx < 0)
        {
            _bassics.esm.Transition<Idle>();
        }
        else
        { 
            _bassics.esm.Transition<Attacking>();
            _director.playableAsset = actions[idx];
            _director.Play();
            _director.playableGraph.GetRootPlayable(0).SetSpeed(1 / _bassics.EnemyData.SPB);
        }
        _decisionTime = Conductor.Instance.Beat + _beatsPerDecision;
    }
}
