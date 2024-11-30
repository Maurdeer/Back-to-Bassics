using System.Collections;
using System.Collections.Generic;
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
    public float timelineDurationInBeats;
    private void Awake()
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
        if (!IsActive) return null;
        IsActive = false;
        Disable();
        return OnStopAction();
    }
    protected virtual void OnStartAction() { }
    protected virtual Coroutine OnStopAction() { return null; }
}
