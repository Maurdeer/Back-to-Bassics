using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BassicsAI : BossAI
{
    [SerializeField] private GameObject comboList;
    // Start is called before the first frame update
    protected override void PhaseChange() {
        base.PhaseChange();
        DisplayCombos();
    }

    private void DisplayCombos() {
        // comboList.SetActive(true);
    }

    private void HideCombos() {
        comboList.SetActive(false);
    }
}
