using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyBattlePawnData"), System.Serializable]
public class EnemyBattlePawnData : BattlePawnData
{
    [Header("Enemy Data")]
    [SerializeField] private int _bpm;
    public int BPM => _bpm;
    public float SPB => 60f / _bpm;
    
    public FMODUnity.EventReference fmodEvent;
}
