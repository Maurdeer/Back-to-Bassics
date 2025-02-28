using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SummonSubortinate : EnemyAction
{
    [SerializeField] private GameObject _subortinatePrefabRef;
    [SerializeField] private EnemyBattlePawn kingSal;
    private bool empoweredSubordinates;
    private Subortinate leftInstance;
    private Subortinate rightInstance;
    private Vector3 leftSummonLocation;
    private Vector3 rightSummonLocation;    
    private void Start()
    {
        leftSummonLocation = parentPawn.targetFightingLocation.position + new Vector3(-9, 0, 0);
        rightSummonLocation = parentPawn.targetFightingLocation.position + new Vector3(9, 0, 0);
        kingSal.OnEnemyStaggerEvent += StaggerMinions;
        kingSal.OnEnemyUnstaggerEvent += UnstaggerMinions;
    }
    protected override void OnStartAction()
    {
        float duration = (timelineDurationInBeats - 1) * Conductor.Instance.spb;
        parentPawnSprite.Animator.SetFloat("speed", 1 / duration);
        parentPawnSprite.Animator.Play("kingsal_summon");
        if (rightInstance == null)
        {
            rightInstance = Instantiate(_subortinatePrefabRef).GetComponent<Subortinate>();
            if (empoweredSubordinates) rightInstance.UpgradeStats();
            rightInstance.Summon(rightSummonLocation, Direction.West);
        }
        else if (leftInstance == null)
        {
            leftInstance = Instantiate(_subortinatePrefabRef).GetComponent<Subortinate>();
            if (empoweredSubordinates) leftInstance.UpgradeStats();
            leftInstance.Summon(leftSummonLocation, Direction.East);
        }
        StopAction();
    }

    public Subortinate GetLeftSubordinate() {
        return leftInstance;
    }

    public Subortinate GetRightSubordinate() {
        return rightInstance;
    }


    // protected override void OnDestroy()
    // {
    //     leftInstance?.Kill();
    //     rightInstance?.Kill();
    // }


    // This works, just modify the code that applies to them.
    public void UpgradeMinions() {
        Debug.Log("TIME FOR A LITTLE PICK ME UP!");
        if (leftInstance != null) leftInstance.UpgradeStats();
        if (rightInstance != null) rightInstance.UpgradeStats();
        empoweredSubordinates = true;
    }

    private void StaggerMinions() {
        leftInstance?.Stagger();
        rightInstance?.Stagger();
    }

    private void UnstaggerMinions() {
        leftInstance?.Unstagger();
        rightInstance?.Unstagger();
    }
}
