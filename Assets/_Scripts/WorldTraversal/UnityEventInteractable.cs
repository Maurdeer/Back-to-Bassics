using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class UnityEventInteractable : MonoBehaviour, Interactable
{
    public bool interactOnce;
    public UnityEvent OnInteract;
    private bool interacted;
    public void Interact()
    {
        if (interactOnce && interacted) return;
        interacted = true;
        OnInteract.Invoke();
    }
}
