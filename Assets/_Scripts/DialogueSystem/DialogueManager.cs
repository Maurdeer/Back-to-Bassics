using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Cinemachine;
public class DialogueManager : Singleton<DialogueManager>
{
    // Reference to the existing Dialogue Runner in the scene
    public DialogueRunner customDialogueRunner; // Renamed from dialogueRunner to avoid ambiguity
    public List<DialogueViewBase> availableDialogueViews; // Renamed from dialogueViews to avoid ambiguity
    private DialogueViewBase activeDialogueView; // Current active dialogue view

    private void Awake()
    {
        Debug.Log("1");
        bool wasInitialized = InitializeSingleton(this);
        if (!wasInitialized)
        {
            Debug.LogError("Failed");
        }
    }

    // Initialize event listeners and Yarn command handlers
    private void Start()
    {
        if (customDialogueRunner == null)
        {
            Debug.LogError("DialogueRunner is not assigned!");
            return;
        }

        // Ensure that dialogue views are populated
        if (availableDialogueViews == null || availableDialogueViews.Count == 0)
        {
            Debug.LogError("No dialogue views available!");
            return;
        }

        // Set up the view switching command handler
        customDialogueRunner.AddCommandHandler<string>("setView", SetDialogueView);
    }

    public void RunDialogueNode(string node)
    {
        customDialogueRunner.StartDialogue(node);
    }

    // This method handles the Yarn command to switch dialogue views
    public IEnumerator SetDialogueView(string viewType)
    {
        Debug.Log($"Switching to view: {viewType}");
        activeDialogueView = null;
        // Loop through dialogue views and find the matching one
        int i = 0;
        foreach (var view in availableDialogueViews)
        {
            if (view.name.Equals(viewType, System.StringComparison.OrdinalIgnoreCase))
            {
                // Sets the currently active view
                activeDialogueView = view;

                // Swaps the dialogue view 
                customDialogueRunner.SetDialogueViews(new DialogueViewBase[] { view });
                Debug.Log($"Dialogue View Set to: {viewType}");
                break;
            }
            i++;
        }

        // If no view matches, log a warning
        if (activeDialogueView == null)
        {
            Debug.LogWarning($"No dialogue view found for: {viewType}");
        }

        // Required for the IEnumerator return type, even if we aren't waiting for anything
        yield break;
    }

    // This method displays the actual dialogue line using the active view
    public void DisplayDialogue(string line)
    {
        if (activeDialogueView != null)
        {
            Debug.Log($"Displaying line: {line} in {activeDialogueView.name}");

            // Create a dummy LocalizedLine for now
            var localizedLine = new LocalizedLine
            {
                TextID = "dummy_text_id", // In actual cases, this would be set based on the dialogue node
                RawText = line, // The actual text to display
            };

            // Define the onDialogueLineFinished action to continue the dialogue flow
            System.Action onDialogueLineFinished = () => {
                Debug.Log("Dialogue line finished displaying.");
                // Continue dialogue or handle post-dialogue logic here
            };

            // Pass the localized line and the onDialogueLineFinished callback to RunLine
            activeDialogueView.RunLine(localizedLine, onDialogueLineFinished);
        }
        else
        {
            Debug.LogWarning("No active dialogue view is set!");
        }
    }
}
