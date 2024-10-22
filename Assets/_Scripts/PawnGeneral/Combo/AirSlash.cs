using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/AirSlash"), System.Serializable]
public class AirSlash : Combo
{
    [SerializeField] private int damage;
    public int Damage => damage;
    public override void InBattle()
    {
        BattleManager.Instance.Enemy.Damage(damage);

        if(StrId.Equals("nN")) 
        {
            BattleManager.Instance.Enemy.GetComponentInChildren<PawnSprite>().Animator.Play("Jump");
        }
        //Read string to check for combo, depending, force or finish animation
    }
    public override void InTraversal()
    {
        // Nothing Yet
    }
}
