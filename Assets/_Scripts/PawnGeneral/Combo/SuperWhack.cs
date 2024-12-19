using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/SuperWhack"), System.Serializable]
public class SuperWhack : Combo
{
    [SerializeField] private int damage;
    public int Damage => damage;
    public override void InBattle()
    {
        BattleManager.Instance.Enemy.Damage(damage);
        //Debug.Log("Whack (Combat)");
    }
    public override void InTraversal()
    {
        //Debug.Log("Whack (Traversal)");
        DestructibleObject.PlayerCombo(StrId);
        DestructibleObject.PlayerSlashDone();
    }
}