using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlateTrigger : MonoBehaviour
{

    private float pressurePlateDepth = 0.05f;

    [SerializeField] private UnityEvent onPressEvent;
    [SerializeField] private UnityEvent onStayEvent;
    [SerializeField] private UnityEvent onReleaseEvent;
    [SerializeField] private GameObject targetObject;
    private bool pressed; // (Joseph 1 / 13 / 25) Done to resolve an issue where the pressure plate would float
    private Vector3 priorlocation;

    private void OnTriggerEnter(Collider other)
    {
        // If there is a targetObject and the thing that entered the trigger is the targetObject
        if (pressed || (targetObject != null && other.transform.gameObject != targetObject)) return;
        // (Joseph 1 / 13 / 25) Modified this function to more appropriately check if a certain game object is required and if so, only depreses upon that gameObject
        priorlocation = transform.localPosition;
        transform.localPosition -= new Vector3(0, pressurePlateDepth, 0);
        onPressEvent.Invoke();
        pressed = true;

    }

    private void OnTriggerStay(Collider other)
    {
        onStayEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!pressed) return;
        transform.localPosition = priorlocation;
        onReleaseEvent.Invoke();
        pressed = false;
    }

}
