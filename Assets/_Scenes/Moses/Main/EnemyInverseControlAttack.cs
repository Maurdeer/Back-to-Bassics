using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// You will create a script deriving from EnemyAction to inflict the player
public class EnemyInverseControlAttack : EnemyAction
{
    public float effectDuration = 5.0f; // Ryan Can Change this value

    // This is where the attack logic is defined

    protected override void OnStartAction()
    {
        base.OnStartAction();

        // Apply the InverseControls status to the player
        PlayerBattlePawn playerPawn = FindObjectOfType<PlayerBattlePawn>();

        if (playerPawn != null )
        {
            InverseControlStatusAilment reverseControls = playerPawn.gameObject.AddComponent<InverseControlStatusAilment>();
            reverseControls.reverse(effectDuration);
        } else
        {
            Debug.LogError("There is no Player that was attacked");
        }
    }
}
