using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class InteractableDialogue : MonoBehaviour, Interactable
{
    [SerializeField] private string nodeName;
    DialogueRunner dialogueRunner;
    // Start is called before the first frame update
    void Start()
    {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    public void Interact()
    {
        dialogueRunner.StartDialogue(nodeName);
    }
}
