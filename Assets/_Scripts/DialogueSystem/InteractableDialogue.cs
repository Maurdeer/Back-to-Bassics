using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class InteractableDialogue : MonoBehaviour, Interactable
{
    [SerializeField] private string nodeName;
    public void Interact()
    {
        DialogueManager.Instance.RunDialogueNode(nodeName);
    }
}
