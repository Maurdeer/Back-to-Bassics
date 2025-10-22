using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PressurePlateTrigger : MonoBehaviour
{

    private float pressurePlateDepth = 0.05f;

    [SerializeField] private UnityEvent onPressEvent;
    [SerializeField] private UnityEvent onStayEvent;
    [SerializeField] private UnityEvent onReleaseEvent;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float unpressedDelay = 0f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool onlyPlayer = false;
    private bool pressed = false;
    private Vector3 unpressedlocation;
    private Vector3 targetPosition;
    private Vector3 pressedPosition;

    private void Awake()
    {
        unpressedlocation = transform.localPosition;
        pressedPosition = unpressedlocation - new Vector3(0, pressurePlateDepth, 0);
    }

    private void FixedUpdate()
    {
        //if (transform.localPosition != targetPosition)
        //{
        //    transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, speed * Time.fixedDeltaTime);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onlyPlayer && other.GetComponent<PlayerTraversalPawn>() == null) return;
        // If there is a targetObject and the thing that entered the trigger is the targetObject
        if (pressed || (targetObject != null && other.transform.gameObject != targetObject)) return;
        // (Joseph 1 / 13 / 25) Modified this function to more appropriately check if a certain game object is required and if so, only depreses upon that gameObject
        transform.localPosition = pressedPosition;
        onPressEvent.Invoke();
        pressed = true;

    }

    private void OnTriggerStay(Collider other)
    {
        if (onlyPlayer && other.GetComponent<PlayerTraversalPawn>() == null) return;
        onStayEvent.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (onlyPlayer && other.GetComponent<PlayerTraversalPawn>() == null) return;
        if (!pressed) return;
        transform.localPosition = unpressedlocation;
        onReleaseEvent.Invoke();

        if (unpressedDelay <= 0)
        {
            pressed = false;
        }
        else
        {
            StartCoroutine(DelayUnpressing());
        }
        
    }

    private IEnumerator DelayUnpressing()
    {
        yield return new WaitForSeconds(unpressedDelay);
        pressed = false;
    }

}
