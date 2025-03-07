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

        if (rightInstance == null && leftInstance == null) {
            if (Random.Range(0, 2) == 0) { // Summon to the right side
                rightInstance = InstantiateSubordinate(rightSummonLocation, Direction.West);
            } else {
                leftInstance = InstantiateSubordinate(leftSummonLocation, Direction.East);
            }
        }
        else if (rightInstance == null) {
            rightInstance = InstantiateSubordinate(rightSummonLocation, Direction.West);
        } else if (leftInstance == null) {
            leftInstance = InstantiateSubordinate(leftSummonLocation, Direction.East);
        }
        StopAction();
    }

    private Subortinate InstantiateSubordinate(Vector3 summonLocation, Direction direction) {
        Subortinate summonedSubordinate = Instantiate(_subortinatePrefabRef).GetComponent<Subortinate>();
        if (empoweredSubordinates) rightInstance.UpgradeStats();
        summonedSubordinateCount++;
        summonedSubordinate.Summon(summonLocation, direction);
        return summonedSubordinate;
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
