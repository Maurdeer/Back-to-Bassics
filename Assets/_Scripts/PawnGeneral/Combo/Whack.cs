using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/Whack"), System.Serializable]
public class Whack : Combo
{
    [SerializeField] private int damage;
    public int Damage => damage;
    public override void InBattle()
    {
        BattleManager.Instance.Enemy.Damage(damage);
        BattleManager.Instance.AddPlayerScore(20);
        //Debug.Log("Whack (Combat)");
    }
    public override void InTraversal()
    {
        //Debug.Log("Whack (Traversal)");
        DestructibleObject.PlayerCombo(StrId);
        DestructibleObject.PlayerSlashDone();
    }
}