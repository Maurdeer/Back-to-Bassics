using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/BattlePawnData"), System.Serializable]
public class BattlePawnData : ScriptableObject
{
    [Header("Battle Pawn Data")]
    [SerializeField, Tooltip("Name of pawn")] private string _name;
    [SerializeField, Tooltip("Hit points")] private int _hp;
    [SerializeField] private float staggerDuration = 4f;
    [SerializeField, TextArea] private string _lore;

    //[Header("Resistances")]
    //[SerializeField] private float _blockingReduction;
    //[SerializeField] private float _confuseResistance;
    //[SerializeField] private float _poisonResistance;

    // Data Properties
    public string Name => _name;
    public int HP => _hp;
    public float StaggerDuration => staggerDuration;
    public string Lore => _lore;

    // Resistances
    //public float BlockingReduction => _blockingReduction;
    //public float ConfuseResistance => _confuseResistance;
    //public float PoisonResistance => _poisonResistance;
}
