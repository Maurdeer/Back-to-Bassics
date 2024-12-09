using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SummonSubortinate : EnemyAction
{
    [SerializeField] private GameObject _subortinatePrefabRef;
    private Subortinate leftInstance;
    private Subortinate rightInstance;
    private Vector3 leftSummonLocation;
    private Vector3 rightSummonLocation;    
    private void Start()
    {
        leftSummonLocation = parentPawn.targetFightingLocation.position + new Vector3(-9, 0, 0);
        rightSummonLocation = parentPawn.targetFightingLocation.position + new Vector3(9, 0, 0);
    }
    protected override void OnStartAction()
    {
        if (rightInstance == null)
        {
            rightInstance = Instantiate(_subortinatePrefabRef).GetComponent<Subortinate>();
            rightInstance.Summon(rightSummonLocation, Direction.West);
        }
        else if (leftInstance == null)
        {
            leftInstance = Instantiate(_subortinatePrefabRef).GetComponent<Subortinate>();
            leftInstance.Summon(leftSummonLocation, Direction.East);
        }
        StopAction();
    }

}
