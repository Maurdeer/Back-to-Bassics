


// holds health thresholds for stages, dialogue between stages, attacks


using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "Data/EnemyStageData"), System.Serializable]
public class EnemyStageData : ScriptableObject {
    [SerializeField] private float _healthThreshold;
    // indices of attacks in enemyActionSequences
    [SerializeField] private TimelineAsset[] _enemyActionSequences;
    [SerializeField] private int _beatsPerDecision;
    [SerializeField] private int _staggerHealth;
    [SerializeField] private string _dialogueNode;

    // future: add dialogue, bpm changes?
    public string DialogueNode => _dialogueNode;
    public float HealthThreshold => _healthThreshold;
    public TimelineAsset[] EnemyActionSequences => _enemyActionSequences;
    public int BeatsPerDecision => _beatsPerDecision;
    public int StaggerHealth => _staggerHealth;
}
