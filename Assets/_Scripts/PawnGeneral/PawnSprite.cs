using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSprite : MonoBehaviour
{
    public Animator Animator => _animator;
    public Vector3 FacingDirection => _facingDirection;
    protected Animator _animator;
    protected Vector3 _facingDirection;
    protected SpriteRenderer _spriteRenderer;
    private Coroutine _flipRoutine;
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        _facingDirection = new Vector3(_animator.GetFloat("x_faceDir"), 0, _animator.GetFloat("z_faceDir"));
    }
    protected virtual void Start()
    {
        //_spriteRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
    public void FaceDirection(Vector3 direction)
    {
        if (direction.x != 0)
        {

            _animator.SetFloat("x_faceDir", Mathf.Sign(direction.x));
        }
        if (direction.z != 0)
        {
            _animator.SetFloat("z_faceDir", Mathf.Sign(direction.z));
        }
        Vector2 change = new Vector2(_animator.GetFloat("x_faceDir"), _animator.GetFloat("z_faceDir"));
        //Vector2 change = new Vector2(direction.x != 0 ? Mathf.Sign(direction.x) : _facingDirection.x,
        //    direction.z != 0 ? Mathf.Sign(direction.z) : _facingDirection.z);
        float angle = Vector2.SignedAngle(_facingDirection, change);
        _facingDirection = change;
        if (angle > 0)
        {
            _animator.SetTrigger("flip_ccw");
            //if (_flipRoutine != null) StopCoroutine(_flipRoutine);
            //_flipRoutine = StartCoroutine(Part1Flip(change));
        }
        else if (angle < 0)
        {
            _animator.SetTrigger("flip_cw");
            //if (_flipRoutine != null) StopCoroutine(_flipRoutine);
            //_flipRoutine = StartCoroutine(Part1Flip(change));
        }   
    }
    private IEnumerator Part1Flip(Vector2 change)
    {
        yield return new WaitForSeconds(0.1f);
        _animator.SetFloat("x_faceDir", change.x);
        _animator.SetFloat("z_faceDir", change.y);
    }
}
