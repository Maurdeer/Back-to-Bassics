using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingSalAI : BossAI
{
    [Header("King Sal Specific Parameters")]
    [SerializeField] private SummonSubortinate regularSubordinateSummon;
    private bool empoweredSubordinates = false;

    protected override void PhaseChange() {
        base.PhaseChange();
        if (!empoweredSubordinates && _currentStage == 2) {
            regularSubordinateSummon.UpgradeMinions();
            empoweredSubordinates = true;
        }
    }


}
