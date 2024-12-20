using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeflectableHitBox : MonoBehaviour, IAttackRequester
{
    [SerializeField] private int _damage;
    [SerializeField] private float _deflectionCoyoteTime = 0.25f;

    // Subscribable Events
    public event Action<IAttackReceiver> OnHit;
    public event Action<IAttackReceiver> OnDeflect;
    public event Action<IAttackReceiver> OnDodge;
    public event Action<IAttackReceiver> OnBlock;
    public event Action OnTriggered;

    // Bool Checks
    public delegate bool BoolDelegate(IAttackReceiver r);
    public event BoolDelegate DeflectCheck;
    public event BoolDelegate DodgeCheck;
    private void Start()
    {
        if (DeflectCheck == null || DodgeCheck == null) {
            Debug.LogWarning("Not all hitbox bool checks are referenced");
        }
    }
    public void OnAttackMaterialize(IAttackReceiver receiver)
    {
        if (receiver is not PlayerBattlePawn pawn)
        {
            return;
        }
        
        pawn.Damage(_damage);
        OnHit?.Invoke(receiver);
    }
    public float GetDeflectionCoyoteTime()
    {
        return _deflectionCoyoteTime;
    }
    public bool OnRequestDeflect(IAttackReceiver receiver)
    {
        if (!DeflectCheck.Invoke(receiver)) return false;

        // (TEMP) Manual DEBUG UI Tracker -------
        UIManager.Instance.IncrementParryTracker();
        //--------------------------------------- 
        OnDeflect?.Invoke(receiver);
        
        return true;
    }
    public bool OnRequestBlock(IAttackReceiver receiver)
    {
        OnBlock?.Invoke(receiver);
        return true;
    }
    public bool OnRequestDodge(IAttackReceiver receiver)
    {
        if (!DodgeCheck.Invoke(receiver)) return false;
        OnDodge?.Invoke(receiver);
        return true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerBattlePawn player))
        {
            player.ReceiveAttackRequest(this);
            OnTriggered?.Invoke();
        }
    }
}
