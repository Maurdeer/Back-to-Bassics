using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyStateMachine;
using Cinemachine;

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
    [SerializeField] private CinemachineVirtualCamera spinCamera;


    protected override void OnStartAction() {  
        StartCoroutine(DoTheThing());
    }

    private IEnumerator DoTheThing() {
        float spinDuration = (timelineDurationInBeats * Conductor.Instance.spb) - 4.5f;
        parentPawnSprite.Animator.SetFloat("speed", 5f / 4f);
        parentPawnSprite.Animator.Play($"TurboTopEnterSpinAction");
        yield return new WaitForSeconds(0.8f);
        parentPawnSprite.Animator.Play($"rise");
        CameraConfigure.Instance.SwitchToCamera(spinCamera);
        yield return new WaitForSeconds(0.8f);
        parentPawnSprite.Animator.Play("TurboTopRevealSword");
        yield return new WaitForSeconds(0.8f);
        spin.enabled = true;
        spin.speed = spin.minSpeed;
        yield return new WaitForSeconds(spinDuration);
        // Segment
        spin.Finish();
        yield return new WaitUntil(() => spin.gameObject.transform.rotation.eulerAngles == Vector3.zero);
        parentPawnSprite.Animator.Play("TurboTopHideSword");
        parentPawnSprite.Animator.Play($"lower");

        CameraConfigure.Instance.SwitchBackToPrev();
        yield return new WaitForSeconds(0.8f);
        StopAction();
    }

    protected override void OnStopAction()
    {
        parentPawnSprite.FaceDirection(Vector3.zero);
        parentPawnSprite.Animator.Play("TurboTopExitSpinAction");
        spin.Reset();
        spin.enabled = false;
    }
    //private IEnumerator
}
