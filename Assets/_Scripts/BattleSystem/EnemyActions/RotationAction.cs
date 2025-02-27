using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyStateMachine;
using Cinemachine;

public class RotationAction : EnemyAction
{
    [Header("Rotation Action Specifics")]
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float fakeOutChance = 0.2f;
    [SerializeField] private float spinSpeedIncreasePerHit = 0.2f;
    [SerializeField] private bool reduceSpeedOnHit = true;
    [Header("References")]
    [SerializeField] private Spinning spinner;
    [SerializeField] private DeflectableHitBox hitBox;
    [SerializeField] private CinemachineVirtualCamera spinCamera;

    #region Technical
    private Coroutine activeSpinThread;
    private float resetSpeed = 0f;
    #endregion

    private bool wasPlayerHit;

    protected override void Awake()
    {
        base.Awake();
        wasPlayerHit = true;
        if (spinner == null)
        {
            Debug.LogError($"Rotation Action must reference spinner");
            return;
        }
        if (hitBox == null)
        {
            Debug.LogError($"Rotation Action must reference hitBox");
            return;
        }
    }
    protected override void OnStartAction() {
        if (activeSpinThread != null)
        {
            Debug.LogError("Attempting to start spin action even though it is already active");
            return;
        }
        wasPlayerHit = false;
        spinner.minSpeed = minSpeed;
        spinner.maxSpeed = maxSpeed;

        // Subscribe to Events
        hitBox.OnHit += OnHit;
        hitBox.OnDeflect += OnDeflect;
        hitBox.DeflectCheck += DeflectCheckEvent;
        hitBox.DodgeCheck += DodgeCheckEvent;

        activeSpinThread = StartCoroutine(SpinThread());
    }
    protected override Coroutine OnStopAction()
    {
        hitBox.OnHit -= OnHit;
        hitBox.OnDeflect -= OnDeflect;
        hitBox.DeflectCheck -= DeflectCheckEvent;
        hitBox.DodgeCheck -= DodgeCheckEvent;
        resetSpeed = 0;

        /// [WRECKCON] TURBOTOP_SPECIAL
        if (!wasPlayerHit)
        {
            UIManager.Instance.WreckconQuests.MarkAchievement(9);
        }
        ///==============================
        return StartCoroutine(StopSpinThread());
    }
    private IEnumerator SpinThread() {
        float spinDuration = (timelineDurationInBeats * Conductor.Instance.spb) - 5f;
        parentPawnSprite.Animator.SetFloat("speed", 5f / 4f);
        parentPawnSprite.Animator.Play($"TurboTopEnterSpinAction");
        yield return new WaitForSeconds(0.8f);
        parentPawnSprite.Animator.Play($"rise");
        CameraConfigure.Instance.SwitchToCamera(spinCamera);
        yield return new WaitForSeconds(0.8f);
        parentPawnSprite.Animator.Play("TurboTopRevealSword");
        yield return new WaitForSeconds(0.8f);
        spinner.enabled = true;
        spinner.speed = spinner.minSpeed;
        yield return new WaitForSeconds(spinDuration);
        
        StopAction();
    }
    private IEnumerator StopSpinThread()
    {
        if (activeSpinThread != null)
        {
            StopCoroutine(activeSpinThread);
            activeSpinThread = null;
        }
        spinner.Finish();

        yield return new WaitUntil(() => spinner.gameObject.transform.rotation.eulerAngles == Vector3.zero);
        parentPawnSprite.Animator.Play("TurboTopHideSword");
        parentPawnSprite.Animator.Play($"lower");
        
        CameraConfigure.Instance.SwitchBackToPrev();
        yield return new WaitForSeconds(0.8f);
        parentPawnSprite.Animator.Play("TurboTopExitSpinAction");
        yield return new WaitForSeconds(0.8f);
        spinner.Reset();
        spinner.enabled = false;
    }
    // HitBox Related Methods
    private void OnHit(IAttackReceiver receiver)
    {
        wasPlayerHit = true;
        // Decrease spinner speed if player is hit
        if (reduceSpeedOnHit && spinner.speed > spinner.minSpeed)
        {
            spinner.speed /= 2;
            spinner.speed = Mathf.Max(spinner.speed, spinner.minSpeed);
            spinner.ReduceSpeed(spinner.speed / 2);
            resetSpeed = spinner.speed / 2;
        }
    }
    private void OnDeflect(IAttackReceiver receiver)
    {
        // Limit max speed of spinner
        if (spinner.speed < spinner.maxSpeed)
        {
            spinner.speed += resetSpeed;
            spinner.speed = Mathf.Min(spinner.speed, spinner.maxSpeed);
        }
        // Randomize fake out chance
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand <= fakeOutChance)
        {
            spinner.FakeOut(spinner.minSpeed + resetSpeed);
        }
        else
        {
            spinner.ChangeDirection(spinner.minSpeed + resetSpeed);
        }
        resetSpeed += spinSpeedIncreasePerHit;

        // (TEMP)----------- This is dumb IK---------------------
        BattleManager.Instance.Enemy.StaggerDamage(1);
        //-------------------------------------------------------
    }
    private bool DeflectCheckEvent(IAttackReceiver receiver)
    {
        PlayerBattlePawn player = receiver as PlayerBattlePawn;
        return player != null
        && DirectionHelper.MaxAngleBetweenVectors(spinner.ccw ? Vector2.right : Vector2.left, player.SlashDirection, 5f);
    }
    private bool DodgeCheckEvent(IAttackReceiver receiver)
    {
        return true;
    }
}
