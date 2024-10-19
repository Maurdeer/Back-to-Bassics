using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/Whoosh"), System.Serializable]
public class Whoosh : Combo
{
    [SerializeField] private int damage;
    [SerializeField] private int playerDamage;
    public int Damage => damage;
    public override void InBattle()
    {
        BattleManager.Instance.Enemy.Damage(damage);
        BattleManager.Instance.Player.Damage(playerDamage);
    }
    public override void InTraversal()
    {
        //get whatever obj they just tried to burn
        Debug.Log("Whoosh (Traversal)");
        BurnableButNotAbstract.PlayerCombo(StrId);
        BurnableButNotAbstract.PlayerSlashDone();
    }
}