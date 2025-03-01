using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using static GameStateMachine;

public class Subortinate : Conductable
{
    [SerializeField] private int decisionTimeInBeats = 4;
    [SerializeField] private int fasterDecisionTimeInBeats;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float fasterSpeed;
    [SerializeField] private int health = 3;
    [Header("References")]
    [SerializeField] private DeflectableHitBox hitBox;
    [SerializeField] private Animator _spriteAnimator;
    private SubortinateState state;
    private Coroutine activeThread;
    private int currDecisionTime;
    private Vector3 startingPosition;
    private bool facingWest;
    private void Awake()
    {
        _spriteAnimator = GetComponentInChildren<Animator>();
        currDecisionTime = decisionTimeInBeats;
        hitBox.OnHit += OnHit;
        hitBox.OnDeflect += OnDeflect;
        hitBox.DeflectCheck += DeflectCheckEvent;
        hitBox.DodgeCheck += DodgeCheckEvent;
        hitBox.OnTriggered += () => StopCoroutine(activeThread);
    }
    public void Summon(Vector3 startingPosition, Direction direction)
    {
        Enable();
        state = SubortinateState.idle;
        _spriteAnimator.Play("idle");
        this.startingPosition = startingPosition;
        
        if (direction == Direction.West)
        {
            transform.rotation = Quaternion.Euler(0, 180f, 0);
            transform.position = new Vector3(10, 0, 0) + startingPosition;
            facingWest = true;
        }
        else
        {
            transform.position = new Vector3(-10, 0, 0) + startingPosition;
        }
        StartCoroutine(MoveIntoPosition());
    }

    public void UpgradeStats() {
        speed = fasterSpeed;
        decisionTimeInBeats = fasterDecisionTimeInBeats;
    }

    public void Stagger() {
        if (this == null) return;
        state = SubortinateState.idle;

        if (activeThread != null) StopCoroutine(activeThread);
        transform.position = startingPosition;
        _spriteAnimator?.Play("staggered");
        // _spriteAnimator.Play("idle");
    }

    public void Unstagger() {
        // Debug.Log("I'm unstaggering now!");
        currDecisionTime = decisionTimeInBeats;
        if (_spriteAnimator != null) _spriteAnimator.Play("idle");
    }
    protected override void OnFullBeat() 
    {
        if (state == SubortinateState.idle || state == SubortinateState.attack || state == SubortinateState.dead) return;
        if (currDecisionTime > 0)
        {
            currDecisionTime--;
            return;
        }

        

        switch (state)
        {
            // case SubortinateState.idle:
            //     if (Random.Range(0, 2) == 1)
            //     {
            //         state = SubortinateState.broadcast;
            //         _spriteAnimator.Play("engard");
            //     }
            //     break;
            case SubortinateState.broadcast:
                state = SubortinateState.attack;
                _spriteAnimator.Play("charge");
                activeThread = StartCoroutine(ChargeThread());
                break;
            default:
                Debug.LogError("Should only decide during idle and broadcast!");
                return;
        }

        currDecisionTime = decisionTimeInBeats;
    }

    // About 510 in Timeline, 240 For Sal Slash
    public void Attack() {
        Debug.Log("Attacking, playing animation Engarde and setting state to broadcast");
        state = SubortinateState.broadcast;
        _spriteAnimator?.Play("engard");
    }

    private IEnumerator ChargeThread()
    { 
        Vector3 targetPosition = BattleManager.Instance.Player.transform.position;
        while (Vector3.Distance(transform.position, targetPosition) > 0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.fixedDeltaTime * speed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = targetPosition;     
    }
    private IEnumerator GoBack()
    {
        while (Vector3.Distance(transform.position, startingPosition) > 0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startingPosition, Time.fixedDeltaTime * speed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = startingPosition;
        state = SubortinateState.idle;
        _spriteAnimator.Play("idle");
    }
    private IEnumerator MoveIntoPosition()
    {
        while (Vector3.Distance(transform.position, startingPosition) > 0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startingPosition, Time.fixedDeltaTime * speed);
            yield return new WaitForFixedUpdate();
        }
        transform.position = startingPosition;
    }

    public void Kill()
    {
        state = SubortinateState.dead;
        Disable();
        StartCoroutine(FlyOutAndDestroy(0.5f, 15f));
    }

    private IEnumerator FlyOutAndDestroy(float duration, float speed)
    {
        _spriteAnimator.Play("dead");
        while (duration > 0f)
        {
            Vector3 newPosition = transform.position + (facingWest ? new Vector3(200, 200, 0) : new Vector3(-200, 200, 0));
            transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.fixedDeltaTime * speed);
            yield return new WaitForFixedUpdate();
            duration -= Time.fixedDeltaTime;
        }
        Destroy(gameObject);
    }
    

    #region HitBox Methods
    private void OnDeflect(IAttackReceiver receiver)
    {
        _spriteAnimator.Play("deflected");
        if (--health <= 0)
        {
            Kill();
        }
        else
        {
            StartCoroutine(GoBack());
        }     
    }
    private void OnHit(IAttackReceiver receiver)
    {
        StartCoroutine(GoBack());
    }
    private bool DeflectCheckEvent(IAttackReceiver receiver)
    {
        return (facingWest && BattleManager.Instance.Player.SlashDirection == Vector2.right)
            || BattleManager.Instance.Player.SlashDirection == Vector2.left;
    }
    private bool DodgeCheckEvent(IAttackReceiver receiver)
    {
        StartCoroutine(GoBack());
        return (!facingWest || BattleManager.Instance.Player.DodgeDirection != Direction.East)
            && BattleManager.Instance.Player.DodgeDirection != Direction.West;
    }


    #endregion
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.TryGetComponent(out PlayerBattlePawn pawn))
    //    {
    //        pawn.ReceiveAttackRequest(this);
    //        StopCoroutine(activeThread);
    //    }
    //}
}

public enum SubortinateState
{
    idle,
    broadcast,
    attack,
    dead,
}
