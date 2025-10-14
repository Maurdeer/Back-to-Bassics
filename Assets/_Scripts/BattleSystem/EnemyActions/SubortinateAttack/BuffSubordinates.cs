using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffSubordinates : EnemyAction
{
    // This works, just modify the code that applies to them.
    // Probably shift this to keep track of in BossAI to reduce dependencies?
    [SerializeField] private SummonSubortinate summon;
    protected override void OnStartAction()
    {
        float duration = (timelineDurationInBeats - 1) * Conductor.Instance.spb;
        parentPawnSprite.Animator?.SetFloat("speed", 1 / duration);
        parentPawnSprite.Animator?.Play("Stage3");
        UpgradeMinions();
        StopAction();
    }

    public void UpgradeMinions() {
        Debug.Log("TIME FOR A LITTLE PICK ME UP!");
        if (summon.leftInstance != null) summon.leftInstance.UpgradeStats();
        if (summon.rightInstance != null) summon.rightInstance.UpgradeStats();
        summon.empoweredSubordinates = true;
    }
}
