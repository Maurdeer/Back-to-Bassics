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

    private void OnTriggerEnter(Collider other)
    {
        transform.localPosition -= new Vector3(0, pressurePlateDepth, 0);
        onPressEvent.Invoke();
    }

    private void OnTriggerStay(Collider other)
    {
        onStayEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        transform.localPosition += new Vector3(0, pressurePlateDepth, 0);
        onReleaseEvent.Invoke();
    }

}
