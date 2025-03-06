using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SummonSubortinate : EnemyAction
{
    [SerializeField] private GameObject _subortinatePrefabRef;
    [SerializeField] private EnemyBattlePawn kingSal;
    private bool empoweredSubordinates;
    public Subortinate leftInstance;
    public Subortinate rightInstance;
    private Vector3 leftSummonLocation;
    private Vector3 rightSummonLocation;
    private int summonedSubordinateCount = 0;
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
        parentPawnSprite.Animator?.SetFloat("speed", 1 / duration);
        parentPawnSprite.Animator?.Play("kingsal_summon");
        if (rightInstance == null && Random.Range(0, 2) == 0) {
            rightInstance = Instantiate(_subortinatePrefabRef).GetComponent<Subortinate>();
            if (empoweredSubordinates) rightInstance.UpgradeStats();
            rightInstance.Summon(rightSummonLocation, Direction.West);
            summonedSubordinateCount++;
        }
        // parentPawnSprite.Animator?.Set("")
        else if (leftInstance == null)
        {
            // Debug.Log("Summoned to the left");
            leftInstance = Instantiate(_subortinatePrefabRef).GetComponent<Subortinate>();
            if (empoweredSubordinates) leftInstance.UpgradeStats();
            leftInstance.Summon(leftSummonLocation, Direction.East);
            summonedSubordinateCount++;
        }
        StopAction();
    }

    public bool CompletedKillNoSubordinatesTask() {
        return summonedSubordinateCount < 3;
    }

    public Subortinate GetLeftSubordinate() {
        if (leftInstance != null) return leftInstance;
        else return null;
    }

    public Subortinate GetRightSubordinate() {
        if (rightInstance != null) return rightInstance;
        return null;
    }

    public bool GetIsFull() {
        return (leftInstance != null && rightInstance != null);
    }


    // protected override void OnDestroy()
    // {
    //     leftInstance?.Kill();
    //     rightInstance?.Kill();
    // }


    // This works, just modify the code that applies to them.
    public void UpgradeMinions() {
        // Debug.Log("TIME FOR A LITTLE PICK ME UP!");
        if (leftInstance != null) leftInstance.UpgradeStats();
        if (rightInstance != null) rightInstance.UpgradeStats();
        empoweredSubordinates = true;
    }

    private void StaggerMinions() {
        leftInstance?.Stagger();
        rightInstance?.Stagger();
    }

    private void UnstaggerMinions() {
        // Debug.Log("Unstaggering Left Side");
        leftInstance?.Unstagger();
        // Debug.Log("Unstagerg Right Side");
        rightInstance?.Unstagger();
    }
}
