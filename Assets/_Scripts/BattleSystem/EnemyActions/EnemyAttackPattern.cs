


using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(menuName = "Data/EnemyAttackPattern"), System.Serializable]
public class EnemyAttackPattern : ScriptableObject {
    [SerializeField] private bool _interruptable = false;
    [SerializeField] private TimelineAsset _actionSequence;

    public bool Interruptable => _interruptable;
    public TimelineAsset ActionSequence => _actionSequence;
}
