using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static PositionStateMachine;

public class PlayerTraversalPawn : TraversalPawn
{
    [SerializeField] private VisualEffect _slashEffect;
    [field: SerializeField] public EventReference slashSoundReference { get; private set; }
    private PlayerBattlePawn _battlePawn;
    private ComboManager _comboManager;
    private bool attacking;
    private Interactable currInteractable;
    protected override void Awake()
    {
        base.Awake();
        _battlePawn = GetComponent<PlayerBattlePawn>();
        _comboManager = GetComponent<ComboManager>();
    }
    public override void Move(Vector3 direction)
    {
        if (attacking) return;
        base.Move(direction);
    }
    public void Slash(Vector2 slashDirection)
    {
        if (attacking) return;
        _pawnSprite.FaceDirection(new Vector3(slashDirection.x, 0, 0));
        _pawnAnimator.Play($"Slash{DirectionHelper.GetVectorDirection(slashDirection)}");
        _slashEffect.Play();
        AudioManager.Instance.PlayOnShotSound(slashSoundReference, transform.position);
        // Set the Slash Direction
        //SlashDirection = direction;
        //SlashDirection.Normalize();
        updateCombo(slashDirection);
        DestructibleObject.PlayerSlash(slashDirection);
        StartCoroutine(Attacking());
    }
    public void Interact()
    {
        currInteractable?.Interact();
    }
    private IEnumerator Attacking()
    {
        attacking = true;
        yield return new WaitForSeconds(_battlePawn.WeaponData.AttackDuration /* * Conductor.quarter * Conductor.Instance.spb*/);
        DestructibleObject.PlayerSlashDone();
        attacking = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
        if (other.TryGetComponent(out Interactable interactable))
        {
            UIManager.Instance.ShowInteractableUI();
            currInteractable = interactable;
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.TryGetComponent(out Interactable interactable))
    //    {
    //        if (interactable != currInteractable)
    //        {
    //            UIManager.Instance.ShowInteractableUI();
    //            currInteractable = interactable;
    //        }
    //    } 
    //    else
    //    {
    //        currInteractable = null;
    //        UIManager.Instance.HideInteractableUI();
    //    }
    //}
    private void OnTriggerExit(Collider other)
    {
        ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);
        if (other.TryGetComponent(out Interactable interactable) && currInteractable == interactable)
        {
            UIManager.Instance.HideInteractableUI();
            currInteractable = null;
        }
    }
    private void updateCombo(Vector2 slashDirection)
    {
        //if (slash)
        //{
            if (slashDirection == Vector2.left)
            {
                _comboManager.AppendToCombo('W');
            }
            else if (slashDirection == Vector2.right)
            {
                _comboManager.AppendToCombo('E');
            }
            else if (slashDirection == Vector2.up)
            {
                _comboManager.AppendToCombo('N');
            }
            else if (slashDirection == Vector2.down)
            {
                _comboManager.AppendToCombo('S');
            }
        //}
        //else
        //{
        //    switch (DodgeDirection)
        //    {
        //        case Direction.North:
        //            _comboManager.AppendToCombo('n');
        //            UIManager.Instance.ComboDisplay.AddCombo("n");
        //            break;
        //        case Direction.South:
        //            _comboManager.AppendToCombo('s');
        //            UIManager.Instance.ComboDisplay.AddCombo("s");
        //            break;
        //        case Direction.West:
        //            _comboManager.AppendToCombo('w');
        //            UIManager.Instance.ComboDisplay.AddCombo("w");
        //            break;
        //        case Direction.East:
        //            _comboManager.AppendToCombo('e');
        //            UIManager.Instance.ComboDisplay.AddCombo("e");
        //            break;
        //    }
        //}
    }
}
