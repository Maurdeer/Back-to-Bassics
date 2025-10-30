using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This Will be the inherited template for most battleactions!
/// </summary>
public abstract class EnemyAction : Conductable
{
    protected EnemyBattlePawn parentPawn;
    protected PawnSprite parentPawnSprite;
    public bool IsActive { get; protected set; }

    // Very Hacky But is very useful
    [HideInInspector] public float timelineDurationInBeats;
    protected virtual void Awake()
    {
        IsActive = false;
        parentPawn = GetComponentInParent<EnemyBattlePawn>();
        parentPawnSprite = parentPawn.GetComponentInChildren<PawnSprite>();
        if (parentPawn == null) 
        {
            Debug.LogError($"Enemy Action \"{gameObject.name}\" could not find Enemy Pawn Parent");
            return;
        }
        parentPawn.AddEnemyAction(this);
        //Debug.Log($"Enemy Action \"{gameObject.name}\" is type: {GetType()}");
    }
    public void StartAction()
    {
        if (IsActive) return;
        IsActive = true;
        Enable();
        OnStartAction();
    }
    public Coroutine StopAction()
    {
        // Debug.Log("Stop action is being called!");
        if (!IsActive) return null;
        // Debug.Log("Stopped an action! Yipee!");
        IsActive = false;
        Disable();
        return OnStopAction();
    }
    protected virtual void OnStartAction() { }
    protected virtual Coroutine OnStopAction() { return null; }
}
