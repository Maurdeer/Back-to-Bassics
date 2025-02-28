using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/UltCombo"), System.Serializable]
public class UltCombo : Combo
{
    public override void InBattle()
    {
        BattleManager.Instance.AddPlayerScore(1000); // Add first to avoid post damage issues
        BattleManager.Instance.Player.PawnSprite.Animator.Play("greatslash");
        BattleManager.Instance.Enemy.Damage(100);
        BattleManager.Instance.Enemy.UnStagger();
    }
    public override void InTraversal()
    {
        // Does Nothing
    }
}
