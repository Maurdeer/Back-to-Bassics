using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyBattlePawnData"), System.Serializable]
public class EnemyBattlePawnData : BattlePawnData
{
    [Header("Enemy Data")]
    [SerializeField] private int _bpm;
    [SerializeField] private int _staggerHealth;
    public int BPM => _bpm;
    public float SPB => 60f / _bpm;
    public int StaggerHealth => _staggerHealth; // Can be overwritten by EnemyStageData
    public FMODUnity.EventReference fmodEvent;
}
