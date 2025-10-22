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
    private Vector2 _prevToChangeFaceDir;
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
        Vector2 change = new Vector2(direction.x != 0 ? direction.x : _animator.GetFloat("x_faceDir"),
            direction.z != 0 ? direction.z : _animator.GetFloat("z_faceDir"));
        
        if (change != _prevToChangeFaceDir)
        {
            _prevToChangeFaceDir = change;
            _flippingEnumeratorQueue.Enqueue(FlipThread(change, dont_trigger_flip));
            Debug.Log($"{_flippingEnumeratorQueue.Count}, {_currFlippingCoroutine}");
        }  
    }

    public IEnumerator FlipThread(Vector2 change, bool dont_trigger_flip = false)
    {
        if (!dont_trigger_flip)
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
        Debug.Log("Flip Thread Updated");
        _animator.SetFloat("x_faceDir", Mathf.Sign(change.x));
        _animator.SetFloat("z_faceDir", Mathf.Sign(change.y));
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
