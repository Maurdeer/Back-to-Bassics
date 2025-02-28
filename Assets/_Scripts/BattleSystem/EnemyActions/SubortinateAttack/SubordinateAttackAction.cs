using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubordinateAttackAction : EnemyAction
{

    private enum Direction {
        LEFT,
        RIGHT,
        UP
    }

    [Header("Subordinate Attack Action")]
    [SerializeField] private Direction attackDirection;
    [SerializeField] private SummonSubortinate summonSubordinates;
    // Start is called before the first frame update
    protected override void OnStartAction() {
        Debug.Log("Is this how it works?");
        if(attackDirection == Direction.LEFT) {
            summonSubordinates.GetLeftSubordinate()?.Attack();
        } else if (attackDirection == Direction.RIGHT) {
            summonSubordinates.GetRightSubordinate()?.Attack();
        }
        StopAction();
    } 
}
