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
    [SerializeField, Tooltip("Seconds to pass before decaying multiplier")] private uint _clockDelayTH;
    [SerializeField, Tooltip("Seconds to pass to reach decay to 1x")] private uint _clockDecayTH;
    [SerializeField] private ulong _sRankMax;
    public int StaggerHealth => _staggerHealth; // Can be overwritten by EnemyStageData
    public uint ClockDelayTH => _clockDelayTH;
    public uint ClockDecayTH => _clockDecayTH;
    public ulong SRankMax => _sRankMax;
    public Sprite Icon => icon;
    public FMODUnity.EventReference fmodEvent;
    public FMODUnity.EventReference voiceByte;
}
