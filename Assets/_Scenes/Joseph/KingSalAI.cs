using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSalAI : BossAI
{
    [Header("King Sal Specific Parameters")]
    [SerializeField] SummonSubortinate regularSubordinateSummon;
    [SerializeField] SummonSubortinate fastSubordinateSummon;

    protected override void PhaseChange() {
        base.PhaseChange();
        if (_currentStage == 2) {
            Destroy(regularSubordinateSummon);
        }
    }

    

}
