using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform newPosition;
    [SerializeField] private float speed;
    [SerializeField] private GameObject borders;
    private Vector3 startingPosition;
    private Vector3 targetPosition;
    private Vector3 destinationPosition;
    private bool isActive = false;
   
    private void Awake()
    {
        startingPosition = transform.position;
        targetPosition = transform.position;
        destinationPosition = newPosition.position;
        borders.SetActive(false);
    }
    public void FixedUpdate()
    {
        if (isActive)
        {
            if((transform.position - targetPosition).magnitude > 0.1f)
            {
                if (borders != null) borders.SetActive(true);
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);
            }
            else
            {
                if (borders != null) borders.SetActive(false);
                targetPosition = transform.position;
                isActive = false;
            }
            
        }
    }
    public void GoToNextPosition()
    {
        targetPosition = destinationPosition;
        isActive = true;
    }
    public void ReturnToPriorPosition()
    {
        targetPosition = startingPosition;
        isActive = true;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerTraversalPawn>() == null) return;
        other.transform.SetParent(transform, true);
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerTraversalPawn>() == null) return;
        other.transform.SetParent(null, true);
    }
}
