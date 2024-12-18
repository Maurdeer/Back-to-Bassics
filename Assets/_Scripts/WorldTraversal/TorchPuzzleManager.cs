using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TorchPuzzleManager : MonoBehaviour
{
    [SerializeField] private List<TikiTorch> tikiTorches;
    [SerializeField] private GameObject obstacle;
    [SerializeField] private float durationMove;
    [SerializeField] private float amountMove;
    [SerializeField] private Vector3 direction;
    [SerializeField] private UnityEvent onComplete;
    private bool complete = false;

    void Awake()
    {
        foreach (TikiTorch torch in tikiTorches){
            torch.setPuzzleManager(this);
        }
    }


    public void CheckAllBurning() {
        // Check if all tiki torches are currently burning
        foreach (TikiTorch torch in tikiTorches) {
            if (!torch.getBurning()) {
                return; // Exit if any torch is not burning
            }
        }

        // Trigger the event if all torches are burning
        if (!complete)
        {
            complete = true;
            onComplete.Invoke();
            StartCoroutine(MoveObstacleDown()); // Start moving the obstacle
        }
    }

    private IEnumerator MoveObstacleDown()
    {
        Vector3 startPosition = obstacle.transform.position;
        Vector3 endPosition = startPosition + direction.normalized * amountMove; 
        float elapsedTime = 0;

        while (elapsedTime < durationMove)
        {
            obstacle.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / durationMove);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }
        // Ensure it reaches the final position
        obstacle.transform.position = endPosition;
        //TurnOffTorches();
        //complete = false; // Reset complete if you want to allow the puzzle to be re-triggered later
    }

    private void TurnOffTorches()
    {
        foreach (TikiTorch torch in tikiTorches)
        {
            torch.Extinguish(); // Call the method to extinguish the torch
        }
    }
}
