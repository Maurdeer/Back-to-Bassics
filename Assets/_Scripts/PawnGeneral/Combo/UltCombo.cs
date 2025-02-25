using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/UltCombo"), System.Serializable]
public class UltCombo : Combo
{
    public override void InBattle()
    {
        BattleManager.Instance.Player.PawnSprite.Animator.Play("greatslash");
        BattleManager.Instance.Enemy.Damage(100);
        BattleManager.Instance.Enemy.UnStagger();
        BattleManager.Instance.AddPlayerScore(1000);
    }
    public override void InTraversal()
    {
        // Does Nothing
    }
}
