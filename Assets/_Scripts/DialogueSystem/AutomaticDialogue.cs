using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn;
using Yarn.Unity;

public class AutomaticDialogue : MonoBehaviour
{
    [SerializeField] private string nodeName;
    [SerializeField] private UnityEvent onDialogueComplete;
    [SerializeField] private GameObject targetLocation;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<PlayerTraversalPawn>()) {
            StartCoroutine(DialogueStart(other.GetComponent<PlayerTraversalPawn>()));
        }
    }

    private IEnumerator DialogueStart(PlayerTraversalPawn player) {
        Vector3 targetPos = targetLocation.transform.position;
        GameManager.Instance.GSM.Transition<GameStateMachine.Dialogue>();
        player.MoveToDestination(targetPos);
        yield return new WaitUntil(() => !player.movingToDestination);
        DialogueManager.Instance.RunDialogueNode(nodeName);
        StartCoroutine(OnDialogueComplete());
    }

    private IEnumerator OnDialogueComplete()
    {
        _collider.enabled = false;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueRunning);
        onDialogueComplete.Invoke();
        // _collider.enabled = true;
    }
}
