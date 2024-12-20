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
        parentPawn.OnEnemyStaggerEvent += StopAction;
        //Debug.Log($"Enemy Action \"{gameObject.name}\" is type: {GetType()}");
    }
    public void StartAction()
    {
        if (IsActive) return;
        IsActive = true;
        Enable();
        OnStartAction();
    }
    public void StopAction()
    {
        if (!IsActive) return;
        IsActive = false;
        Disable();
        OnStopAction();
    }
    protected virtual void OnStartAction() { }
    protected virtual void OnStopAction() { }
}
