using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinnowsSwarmAction : EnemyAction
{
    [SerializeField] private NoEnemySlashAction slashAction;
    [SerializeField] private NotEnemyFireProjectileAction fireProjectileAction;
    private Animator _minnowsAnimator;
    private PawnSprite _minnowSprite;
    private Coroutine _idleTrickThread;

    protected override void Awake()
    {
        base.Awake();
        _minnowsAnimator = GetComponentInChildren<Animator>();
        _minnowSprite = GetComponentInChildren<PawnSprite>();
    }
    public void IdleSelf(Direction direction)
    {
        if (direction == Direction.East && _minnowsAnimator.GetFloat("x_faceDir") < 0)
        {
            StartCoroutine(IdleTrick(direction));
            
        }
        else if (direction == Direction.West && _minnowsAnimator.GetFloat("x_faceDir") > 0)
        {
            StartCoroutine(IdleTrick(direction));
        }
        else
        {
            _minnowsAnimator.SetFloat("speed", 1);
        }
    }
    public void ShootPlayer(FireProjectileNode node)
    {
        if (_idleTrickThread != null)
        {
            StopCoroutine(_idleTrickThread);
            _idleTrickThread = null;
        }
        fireProjectileAction.timelineDurationInBeats = timelineDurationInBeats;
        _minnowSprite.FaceDirection(new Vector3(node.relativeSpawnPosition.x, 0, -1), true);
        fireProjectileAction.FireProjectileAtPlayer(node);
    }
    public void SlashPlayer(SlashNode node)
    {
        if (_idleTrickThread != null)
        {
            StopCoroutine(_idleTrickThread);
            _idleTrickThread = null;
        }
        slashAction.Slash(node);
    }
    public void CoverPlayerScreen(float duration)
    {
        
    }
    public void EnterBattle(Direction direction)
    {
        _minnowsAnimator.SetFloat("x_faceDir", direction == Direction.East ? 1 : -1);
        _minnowsAnimator.SetFloat("speed", 1 / (Conductor.Instance.spb * timelineDurationInBeats));
        _minnowsAnimator.Play("enter_battle");
    }
    private IEnumerator ShootThread(FireProjectileNode node)
    {
        _minnowSprite.FaceDirection(new Vector3(node.relativeSpawnPosition.x, 0, -1), true);
        float tenth_of_duration = timelineDurationInBeats * 0.1f;
        fireProjectileAction.timelineDurationInBeats = timelineDurationInBeats - tenth_of_duration;
        _minnowsAnimator.SetFloat("speed", 1 / (Conductor.Instance.spb * tenth_of_duration));
        _minnowsAnimator.SetTrigger("bow");
        yield return new WaitUntil(() => !_minnowsAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle_battle") 
        && !_minnowsAnimator.IsInTransition(0));
        fireProjectileAction.FireProjectileAtPlayer(node);
        _minnowsAnimator.ResetTrigger("bow");
    }
    private IEnumerator IdleTrick(Direction direction)
    {
        string dir = direction.ToString().ToLower();
        float transitionTime = Conductor.Instance.spb * timelineDurationInBeats;
        _minnowsAnimator.SetFloat("speed", 1 / transitionTime);
        _minnowsAnimator.SetTrigger($"idle_{dir}"); 
        yield return new WaitUntil(() => _minnowsAnimator.GetCurrentAnimatorStateInfo(0).IsName($"idle_battle_trick_{dir}"));
        _minnowsAnimator.SetFloat("x_faceDir", direction == Direction.East ? 1 : -1);
        _minnowsAnimator.Play("idle_battle");

        _idleTrickThread = null;
    }
}

public enum MinnowsSwarmActionChoice
{
    Idle,
    TurnIntoSword,
    TurnIntoBow,
    Fire,
    Slash,
    Cover,
    EnterBattle,
}
