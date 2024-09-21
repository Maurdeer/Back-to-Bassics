using System;
using UnityEngine;
using Yarn.Unity;

public class ProximityDialogueTrigger : MonoBehaviour
{
    public DialogueRunner dialogueRunner; // Reference to the Dialogue Runner
    public string dialogueNodeName; // Name of the Yarn node to start

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player entered the trigger
        
        {
            Console.WriteLine("Player entered the trigger");
            if (!dialogueRunner.IsDialogueRunning) // Ensure no dialogue is currently running
            {
                dialogueRunner.StartDialogue(dialogueNodeName); // Start the Yarn dialogue
            }
        }
    }
}