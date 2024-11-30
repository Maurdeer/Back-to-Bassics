using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Cinemachine;
using FMODUnity;
using FMOD.Studio;

public class DialogueManager : Singleton<DialogueManager>
{
    // Reference to the existing Dialogue Runner in the scene
    public DialogueRunner customDialogueRunner; // Renamed from dialogueRunner to avoid ambiguity
    public List<DialogueViewBase> availableDialogueViews; // Renamed from dialogueViews to avoid ambiguity
    private DialogueViewBase activeDialogueView; // Current active dialogue view
    public bool IsDialogueRunning => customDialogueRunner.IsDialogueRunning;
    private EventInstance voiceByteInstance;
    private CinemachineVirtualCamera initialVirtualCam;

    private void Awake()
    {
        bool wasInitialized = InitializeSingleton(this);
        if (!wasInitialized)
        {
            Debug.LogError("Dialogue System Already Initialized!");
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
        customDialogueRunner.AddCommandHandler<string>("setCamera", SetCamera);
        customDialogueRunner.AddCommandHandler<GameObject, GameObject>("move", MoveToLocation);
        customDialogueRunner.AddCommandHandler<string, int>("zoom", CameraAdjustment);
        customDialogueRunner.AddCommandHandler<GameObject, string>("play", PlayAnimation);
        voiceByteInstance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.bassicsBlub);
    }

    public void RunDialogueNode(string node)
    {
        // Add soon when you can interface dialogue
        if (GameManager.Instance.GSM.IsOnState<GameStateMachine.WorldTraversal>())
        {
            GameManager.Instance.GSM.Transition<GameStateMachine.Dialogue>();
        }
        if (customDialogueRunner.IsDialogueRunning)
        {
            customDialogueRunner.Stop();
        }
        customDialogueRunner.StartDialogue(node);
        StartCoroutine(OnDialogueComplete());
    }

    private IEnumerator OnDialogueComplete()
    {
        yield return new WaitUntil(() => !customDialogueRunner.IsDialogueRunning);
        if (GameManager.Instance.GSM.IsOnState<GameStateMachine.Dialogue>())
        {
            GameManager.Instance.GSM.Transition<GameStateMachine.WorldTraversal>();
        }
        if (initialVirtualCam != null)
        {
            CameraConfigure.Instance.SwitchToCamera(initialVirtualCam);
        }
    }

    public void VoiceByte()
    {
        PLAYBACK_STATE playbackState;
        voiceByteInstance.getPlaybackState(out playbackState);
        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            voiceByteInstance.start();
        }
        //voiceByteInstance.start();
    }

    // This method handles the Yarn command to switch dialogue views
    public IEnumerator SetDialogueView(string viewType)
    {
        //Debug.Log($"Switching to view: {viewType}");
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
                //Debug.Log($"Dialogue View Set to: {viewType}");
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

    public IEnumerator PlayAnimation(GameObject pawn, string animation)
    {
        // TODO: Import Unity Animation, add animators to necessary targets, and play animations from Yarn Spinner at appropriate times.
        yield break;
    }

    public IEnumerator SetCamera(string cameraName)
    {
        if (initialVirtualCam != null) initialVirtualCam = CameraConfigure.Instance.CurrentVirtualCamera;
        CinemachineVirtualCamera camera = GameObject.Find(cameraName).GetComponent<CinemachineVirtualCamera>();
        if (camera == null)
        {
            Debug.LogWarning($"No camera found for: {cameraName}");
            yield break;
        }
        CameraConfigure.Instance.SwitchToCamera(camera);
        yield break;
    }

    // Function that can be called in yarn files to move TraversalPawns around to set locations.
    public IEnumerator MoveToLocation(GameObject pawn, GameObject targetLocation)
    {
        TraversalPawn traversal = pawn.GetComponent<TraversalPawn>();
        if (traversal == null) {
            Debug.LogWarning($"{pawn.name} is not a Traversal Pawn and cannot move");
        }
        traversal.MoveToDestination(targetLocation.transform.position);
        yield break;
    }

    // Function that allows for Camera Zom in or out
    // THis does not actually work as is right now; it will accurately change the current FOV but that has no impact on the actual zoom of the camera.
    public IEnumerator CameraAdjustment(string cameraName, int zoomFactor) {
        CinemachineVirtualCamera camera = GameObject.Find(cameraName).GetComponent<CinemachineVirtualCamera>();
        if (camera == null)
        {
            Debug.LogWarning($"No camera found for: {cameraName}");
            yield break;
        }
        camera.m_Lens.FieldOfView += zoomFactor;
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
