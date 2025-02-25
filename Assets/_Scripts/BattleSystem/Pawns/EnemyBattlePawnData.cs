using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/EnemyBattlePawnData"), System.Serializable]
public class EnemyBattlePawnData : BattlePawnData
{
    [Header("Enemy Data")]
    [SerializeField] private Sprite icon;
    [SerializeField] private int _staggerHealth;
    [SerializeField, Tooltip("In Seconds to being decay under 1x")] private uint _clockDecayTH;
    public int StaggerHealth => _staggerHealth; // Can be overwritten by EnemyStageData
    public uint ClockDecayTH => _clockDecayTH;
    public FMODUnity.EventReference fmodEvent;
    public FMODUnity.EventReference voiceByte;
}
