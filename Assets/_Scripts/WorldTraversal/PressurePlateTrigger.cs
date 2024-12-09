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
    [SerializeField] private bool smallFryStone; // Done for making sure the stone is complete 


    private void OnTriggerEnter(Collider other)
    {
        // Done quickly to check for the stone
        // TODO: DON'T DO THIS WTF WHY WOULD YOU
        // crunch...
        print(other.transform.gameObject.name);
        if (smallFryStone) {
            if (other.transform.gameObject.name.Equals("DAROCK")) {
                transform.localPosition -= new Vector3(0, pressurePlateDepth, 0);
                onPressEvent.Invoke();
            }
        } else {
            transform.localPosition -= new Vector3(0, pressurePlateDepth, 0);
            onPressEvent.Invoke();
        }

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
