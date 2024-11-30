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
        parentPawnSprite.Animator.Play($"TurboTopEnterSpinAction");
        yield return new WaitForSeconds(1f);
        spin.enabled = true;
        spin.speed = spin.minSpeed;
        parentPawnSprite.Animator.Play($"rise");
        yield return new WaitForSeconds(2f);
        parentPawnSprite.Animator.Play("TurboTopRevealSword");
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(4f);
        parentPawnSprite.Animator.Play("TurboTopHideSword");

        // TODO: Make it where the spinning ends on turbo top facing right up again
        // 0 degrees rot basically then we will lower him and end his action!
        parentPawnSprite.Animator.Play($"lower");
        yield return new WaitForSeconds(1f);
        StopAction();
    }

    protected override void OnStopAction()
    {
        parentPawnSprite.FaceDirection(Vector3.zero);
        parentPawnSprite.Animator.Play("TurboTopExitSpinAction");
        spin.Reset();
        spin.enabled = false;
    }
}
