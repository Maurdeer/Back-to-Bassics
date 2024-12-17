using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyBattlePawnData"), System.Serializable]
public class EnemyBattlePawnData : BattlePawnData
{
    [Header("Enemy Data")]
    [SerializeField] private int _staggerHealth;
    public int StaggerHealth => _staggerHealth; // Can be overwritten by EnemyStageData
    public FMODUnity.EventReference fmodEvent;
    public FMODUnity.EventReference voiceByte;
}
