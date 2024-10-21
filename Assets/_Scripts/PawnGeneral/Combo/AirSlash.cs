using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/AirSlash"), System.Serializable]
public class AirSlash : Combo
{
    int i = 0;

    [SerializeField] private int damage;
    public int Damage => damage;
    public override void InBattle()
    {
        BattleManager.Instance.Enemy.Damage(damage);
        Debug.Log("This combo hit " + i);
        i++;
    }
    public override void InTraversal()
    {
        // Nothing Yet
    }
}
