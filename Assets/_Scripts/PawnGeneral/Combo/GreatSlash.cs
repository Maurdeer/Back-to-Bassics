using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/GreatSlash"), System.Serializable]
public class GreatSlash : Combo
{
    [SerializeField] private int damage;
    public int Damage => damage;
    public override void InBattle()
    {
        if (StrId.Length == 2) BattleManager.Instance.AddPlayerScore(10);
        if (StrId.Length == 4) BattleManager.Instance.AddPlayerScore(30);
        BattleManager.Instance.Enemy.Damage(damage);    
    }
    public override void InTraversal()
    {
        // Nothing Yet
    }
}
