using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class IdiotSandwhich : EnemyAction, IAttackRequester
{
    [SerializeField] private string animationName;
    [SerializeField] private AnimationClip sandwhichAnimationClip;
    public float GetDeflectionCoyoteTime()
    {
        throw new System.NotImplementedException();
    }
    public void OnAttackMaterialize(IAttackReceiver receiver)
    {
        throw new System.NotImplementedException();
    }
    public bool OnRequestBlock(IAttackReceiver receiver)
    {
        throw new System.NotImplementedException();
    }
    public bool OnRequestDeflect(IAttackReceiver receiver)
    {
        throw new System.NotImplementedException();
    }
    public bool OnRequestDodge(IAttackReceiver receiver)
    {
        throw new System.NotImplementedException();
    }
    protected override void OnStartAction()
    {
        
    }
}
