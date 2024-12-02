using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using static GameStateMachine;

public class Subortinate : Conductable, IAttackRequester
{
    [SerializeField] private Animator _spriteAnimator;
    [SerializeField] private int decisionTimeInBeats = 4;
    [SerializeField] private float speed = 5f;
    [SerializeField] private int health = 3;
    [SerializeField] private int damage = 2;
    private SubortinateState state;
    private Coroutine activeThread;
    private int currDecisionTime;
    private Vector3 startingPosition;
    private bool facingWest;
    private void Awake()
    {
        _spriteAnimator = GetComponentInChildren<Animator>();
        currDecisionTime = decisionTimeInBeats;
    }
    public void Summon(Vector3 startingPosition, Direction direction)
    {
        Enable();
        state = SubortinateState.idle;
        _spriteAnimator.Play("idle");
        this.startingPosition = startingPosition;
        
        if (direction == Direction.West)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 180f, 0);
            transform.position = new Vector3(10, 0, 0) + startingPosition;
            facingWest = true;
        }
        else
        {
            transform.position = new Vector3(-10, 0, 0) + startingPosition;
        }
        StartCoroutine(MoveIntoPosition());
    }
    protected override void OnFullBeat() 
    {
        if (state == SubortinateState.attack) return;
        if (currDecisionTime > 0)
        {
            currDecisionTime--;
            return;
        }

        switch (state)
        {
            case SubortinateState.idle:
                if (Random.Range(0, 2) == 1)
                {
                    state = SubortinateState.broadcast;
                    _spriteAnimator.Play("engard");
                }
                break;
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

    private IEnumerator GobackThread()
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

    void IAttackRequester.OnAttackMaterialize(IAttackReceiver receiver)
    {
        if (receiver is not PlayerBattlePawn pawn)
        {
            return;
        }
        StartCoroutine(GobackThread());
        pawn.Damage(damage);
    }

    float IAttackRequester.GetDeflectionCoyoteTime()
    {
        return 0.2f;
    }

    bool IAttackRequester.OnRequestDeflect(IAttackReceiver receiver)
    {
        if ((facingWest && BattleManager.Instance.Player.SlashDirection == Vector2.right)
            || BattleManager.Instance.Player.SlashDirection == Vector2.left)
        {
            _spriteAnimator.Play("deflected");
            if (--health <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(GobackThread());
            }
            
            return true;
        }
        return false;
    }

    bool IAttackRequester.OnRequestBlock(IAttackReceiver receiver)
    {
        StartCoroutine(GobackThread());
        return true;
    }

    bool IAttackRequester.OnRequestDodge(IAttackReceiver receiver)
    {
        StartCoroutine(GobackThread());
        if ((facingWest && BattleManager.Instance.Player.DodgeDirection == Direction.East)
            || BattleManager.Instance.Player.DodgeDirection == Direction.West)
        {
            return false;
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerBattlePawn pawn))
        {
            pawn.ReceiveAttackRequest(this);
            StopCoroutine(activeThread);
        }
    }
}

public enum SubortinateState
{
    idle,
    broadcast,
    attack
}
