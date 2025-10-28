


// holds health thresholds for stages, dialogue between stages, attacks


using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "Data/EnemyStageData"), System.Serializable]
public class EnemyStageData : ScriptableObject {
    [SerializeField] private float _healthThreshold;
    // indices of attacks in enemyActionSequences
    [SerializeField] private EnemyAttackPattern[] _enemyAttackPatterns;
    [SerializeField] private int _beatsPerDecision;
    [SerializeField] private int _staggerHealth;
    [SerializeField] private string _dialogueNode;
    [SerializeField] private EnemyAttackPattern _phaseTransitionMove;
    [SerializeField] private bool _resetStaggerHealth;
    [SerializeField] private bool _skipPointInSong = false;
    [SerializeField, Tooltip("Its in Miliseconds")] private int _pointToSkip;
    

    // future: add dialogue, bpm changes?
    public string DialogueNode => _dialogueNode;
    public float HealthThreshold => _healthThreshold;
    public EnemyAttackPattern[] EnemyAttackPatterns => _enemyAttackPatterns;
    public int BeatsPerDecision => _beatsPerDecision;
    public int StaggerHealth => _staggerHealth;
    public EnemyAttackPattern PhaseTransitionMove => _phaseTransitionMove;
    public bool ResetStaggerHealth => _resetStaggerHealth;
    public bool SkipPointInSong => _skipPointInSong;
    public int PointToSkip => _pointToSkip;
}
