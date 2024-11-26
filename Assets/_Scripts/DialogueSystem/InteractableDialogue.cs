using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn;
using Yarn.Unity;

public class InteractableDialogue : MonoBehaviour, Interactable
{
    [SerializeField] private string nodeName;
    //[SerializeField] private event UnityEvent onDialogueComplete;
    private Collider _collider;
    
    public void Interact()
    {
        DialogueManager.Instance.RunDialogueNode(nodeName);
    }
}
