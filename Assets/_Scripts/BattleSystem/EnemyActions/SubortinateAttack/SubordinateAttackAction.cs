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
        if(attackDirection == Direction.LEFT) {
            Debug.Log("Calling the attack from the left");
            summonSubordinates.GetLeftSubordinate()?.Attack();
        } else if (attackDirection == Direction.RIGHT) {
            Debug.Log("Calling the attack from the right");
            summonSubordinates.GetRightSubordinate()?.Attack();
        }
        StopAction();
    } 
}
