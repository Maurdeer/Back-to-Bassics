


// holds health thresholds for stages, dialogue between stages, indices of attacks, perhaps?


using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "Data/EnemyStageData"), System.Serializable]
public class EnemyStageData : ScriptableObject {
    [SerializeField] private float _healthThreshold;
    // indices of attacks in enemyActionSequences
    [SerializeField] private TimelineAsset[] _enemyActionSequences;
    [SerializeField] private int _beatsPerDecision;

    // future: add dialogue, bpm changes?

    public float HealthThreshold => _healthThreshold;
    public TimelineAsset[] EnemyActionSequences => _enemyActionSequences;
    public int BeatsPerDecision => _beatsPerDecision;
}
