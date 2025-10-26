using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnSprite : MonoBehaviour
{
    public Animator Animator => _animator;
    public Vector3 FacingDirection => _facingDirection;
    protected Animator _animator;
    protected Vector2 _facingDirection;
    private Coroutine _currFlippingCoroutine;
    private Queue<IEnumerator> _flippingEnumeratorQueue;
    private Vector3 _prevToChangeFaceDir;
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _flippingEnumeratorQueue = new Queue<IEnumerator>();
        StartCoroutine(FlipQueuer());
    }
    protected virtual void Start()
    {
        _facingDirection = new Vector2(_animator.GetFloat("x_faceDir"), _animator.GetFloat("z_faceDir"));
    }
    
    public void FaceDirection(Vector3 direction, bool dont_trigger_flip = false)
    {
        if (direction == Vector3.zero)
        {
            // No Significant changes need to be done
            return;
        }

        if (direction != _prevToChangeFaceDir)
        {
            _prevToChangeFaceDir = direction;
            _flippingEnumeratorQueue.Enqueue(FlipThread(direction, dont_trigger_flip));
        }  
    }

    public IEnumerator FlipThread(Vector3 direction, bool dont_trigger_flip = false)
    {
        Vector2 change = new Vector2(direction.x != 0 ? Mathf.Sign(direction.x) : _animator.GetFloat("x_faceDir"),
         direction.z != 0 ? Mathf.Sign(direction.z) : _animator.GetFloat("z_faceDir"));

        if (!dont_trigger_flip && change != _facingDirection)
        {
            float angle = Vector2.SignedAngle(_facingDirection, change);
            _facingDirection = change;
            if (angle > 0)
            {
                _animator.SetTrigger("flip_ccw");
                yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(_animator.GetLayerIndex("Flip Layer")).IsName("Part2"));
            }
            if (angle < 0)
            {
                _animator.SetTrigger("flip_cw");
                yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(_animator.GetLayerIndex("Flip Layer")).IsName("Part2"));
            }
            
        }
        //Debug.Log("Flip Thread Updated");
        _animator.SetFloat("x_faceDir", change.x);
        _animator.SetFloat("z_faceDir", change.y);
        _currFlippingCoroutine = null;
    }

    public IEnumerator FlipQueuer()
    {
        while (true)
        {
            yield return new WaitUntil(() => _flippingEnumeratorQueue.Count > 0 && _currFlippingCoroutine == null);
            _currFlippingCoroutine = StartCoroutine(_flippingEnumeratorQueue.Dequeue());
        }
    }
}
