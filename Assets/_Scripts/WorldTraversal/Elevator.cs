using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private Transform newPosition;
    [SerializeField] private float speed;
    private Vector3 startingPosition;
    private Vector3 targetPosition;
    private Vector3 destinationPosition;
    private void Awake()
    {
        startingPosition = transform.position;
        targetPosition = transform.position;
        destinationPosition = newPosition.position;
    }
    public void FixedUpdate()
    {
        if (transform.position != targetPosition)
        {
            transform.position = Vector3.Slerp(transform.position, targetPosition, speed * Time.fixedDeltaTime);
        }
    }
    public void GoToNextPosition()
    {
        targetPosition = destinationPosition;
    }
    public void ReturnToPriorPosition()
    {
        targetPosition = startingPosition;
    }
}
