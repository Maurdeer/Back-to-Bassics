using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using FMOD.Studio;

[DisallowMultipleComponent]
public class TraversalPawn : MonoBehaviour
{
    [Header("Traversal Pawn Specs")]
    [SerializeField] private float speed;
    [SerializeField] protected Animator _spriteAnimator;
    [field: SerializeField] public EventReference footStepsReference { get; private set; }
    public Animator SpriteAnimator => _spriteAnimator;
    protected Animator _pawnAnimator;
    protected PawnSprite _pawnSprite;
    protected CharacterController _characterController;
    public bool movingToDestination { get; private set; }
    protected Vector3 destinationTarget;
    private Rigidbody _rb;
    private Quaternion pawnFaceRotation;
    private bool isMoving;

    // Audio
    private EventInstance footStepsInstance;
    protected virtual void Awake()
    {
        pawnFaceRotation = transform.rotation;
        _characterController = GetComponent<CharacterController>();
        _rb = GetComponent<Rigidbody>();
        _pawnAnimator = GetComponent<Animator>();
        _pawnSprite = GetComponentInChildren<PawnSprite>();
    }
    protected virtual void Start()
    {
        footStepsInstance = AudioManager.Instance.CreateInstance(footStepsReference);
    }
    protected virtual void FixedUpdate()
    {
        UpdateSound();
        // Rotation and Movement Correction
        transform.rotation = Quaternion.Slerp(transform.rotation, pawnFaceRotation, Time.fixedDeltaTime * 10f);

        // Movement operation under fixedUpdate
        // *Could be removed to be its own corutine*
        if (movingToDestination)
        {
            // TODO: This section right here is what is causing the floating of our character, see if you
            // Can change this where gravity is applied and the character isn't going to try to fly.
            // You might need to use the rigidbody component in order to manipulate their kinetic movement
            

            transform.position = Vector3.MoveTowards(transform.position, destinationTarget, speed * Time.fixedDeltaTime);
            // _rb.AddForce(new Vector3(0, -100, 0));
            if (transform.position == destinationTarget)
            {
                movingToDestination = false;
                _pawnSprite.Animator.SetBool("moving", false);
            }
            else
            {
                Vector3 direction = destinationTarget - transform.position;
                _pawnSprite.Animator.SetBool("moving", true);
                _pawnSprite.FaceDirection(direction);
            }
        }
    }

    // X is Right and Left, Z is Forward and Backward 
    public virtual void Move(Vector3 direction)
    {
        if (!movingToDestination) {
            direction.Normalize();
            Vector3 move = transform.rotation * direction * speed;
            _rb.velocity = new Vector3(move.x, _rb.velocity.y, move.z);
            //_rb.AddForce(move - _rb.velocity);
            //_rb.MovePosition(transform.position + move * Time.deltaTime);
            //_characterController.Move(transform.rotation * direction * speed * Time.deltaTime);
            isMoving = direction != Vector3.zero;
            _pawnSprite.Animator.SetBool("moving", isMoving);
            _pawnSprite.FaceDirection(direction);
        }
        
    }
    public void MoveToDestination(Vector3 destination)
    {
        movingToDestination = true;
        destinationTarget = new Vector3(destination.x, destination.y, destination.z);
    }
    public void RotateOnYAxis(float y)
    {
        pawnFaceRotation = Quaternion.Euler(0, y, 0);
    }
    public void AddToRotationOnYAxis(float y)
    {
        pawnFaceRotation.eulerAngles += new Vector3(0, y, 0);
    }
    private void UpdateSound()
    {
        // Footstep Sounds
        if (isMoving)
        {
            PLAYBACK_STATE playbackState;
            footStepsInstance.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                footStepsInstance.start();
            }
        }
        else
        {
            footStepsInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }
}
