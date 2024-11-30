using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyStateMachine;

public class RotationAction : EnemyAction
{
    //[SerializeField] private GameObject knifeFab;
    // [SerializeField] private Spinning[] spinners;
    // protected override void OnStartAction()
    // {
    //     parentPawn.esm.Transition<Attacking>();
    //     foreach (Spinning spinner in spinners)
    //     {
    //         if (spinner != null) spinner.speed = spinner.minSpeed;
    //     }
    // }
    // protected override void OnStopAction()
    // {
    //     foreach (Spinning spinner in spinners)
    //     {
    //         if (spinner != null) spinner.speed = 0f;
    //     }
    // }
    [SerializeField] private Spinning spin;


    protected override void OnStartAction() {  
        StartCoroutine(DoTheThing());
    }

    private IEnumerator DoTheThing() {
        parentPawnSprite.Animator.Play($"TurboTopEnterSpinEast");
        yield return new WaitForSeconds(1f);
        
        parentPawnSprite.Animator.Play("TurboTopEnterSpinAttack");
        yield return new WaitForSeconds(1f);
        spin.enabled = true;
        spin.speed = spin.minSpeed;
        yield return new WaitForSeconds(4f);
        parentPawnSprite.Animator.Play("TurboTopExitSpinAttack");
        yield return new WaitForSeconds(1f);
        StopAction();
    }

    protected override void OnStopAction()
    {
        parentPawnSprite.FaceDirection(Vector3.zero);
        parentPawnSprite.Animator.Play("TurboTopExitSpinWest");
        spin.Reset();
        spin.enabled = false;
    }
}
