using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn;
using Yarn.Unity;

public class InteractableDialogue : MonoBehaviour, Interactable
{
    [SerializeField] private string nodeName;
    [SerializeField] private UnityEvent onDialogueComplete;
    private Collider _collider;
    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public void Interact()
    {
        DialogueManager.Instance.RunDialogueNode(nodeName);
        StartCoroutine(OnDialogueComplete());
    }

    private IEnumerator OnDialogueComplete()
    {
        _collider.enabled = false;
        yield return new WaitUntil(() => !DialogueManager.Instance.IsDialogueRunning);
        onDialogueComplete.Invoke();
        _collider.enabled = true;
    }
}
