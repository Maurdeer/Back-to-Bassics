using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combo/SpinSlash"), System.Serializable]
public class SpinSlash : Combo
{
    [SerializeField] private int damage;
    [SerializeField] private double damageMultiplier;
    [SerializeField] private float speedBoostDuration; // Boost duration in seconds
    [SerializeField] private float speedMultiplier; // Speed multiplier during boost
    [SerializeField] private float directionChangeInterval;
    public int Damage => damage;
    private PlayerController player;
    private Rigidbody playerRigidbody;
    private PlayerTraversalPawn traversalPawn;
    private PawnSprite pawnSprite;
    private bool isSpeedBoostActive = false;
    private Coroutine boostCoroutine;


    public override void InBattle()
    {
        BattleManager.Instance.Enemy.Damage(damage);
    }

    private void InitializePlayerComponents()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogWarning("PlayerController not found!");
                return;
            }
        }

        if (playerRigidbody == null)
        {
            playerRigidbody = player.GetComponent<Rigidbody>();
            if (playerRigidbody == null)
            {
                Debug.LogWarning("Rigidbody not found on player!");
                return;
            }
        }

        if (traversalPawn == null)
        {
            traversalPawn = player.GetComponent<PlayerTraversalPawn>();
            if (traversalPawn == null)
            {
                Debug.LogWarning("traversalPawn  not found on player!");
                return;
            }
        }

        if (pawnSprite == null) 
        {
            pawnSprite = player.GetComponentInChildren<PawnSprite>();
            if (pawnSprite == null)
            {
                Debug.LogWarning("PawnSprite not found on player!");
                return;
            }

        }
    }
    public override void InTraversal()
    {
        InitializePlayerComponents();
        if (player != null && playerRigidbody != null && traversalPawn != null)
        {
            StartSpeedBoost();
        }
    }

    private void StartSpeedBoost()
    {
        Debug.Log("Speed Boosted");
        if (playerRigidbody.velocity.magnitude > 0.1f) // Check if the player is moving
        {
            Vector3 currentDirection = playerRigidbody.velocity.normalized; // Get movement direction
            float originalSpeed = playerRigidbody.velocity.magnitude; // Save current speed

            isSpeedBoostActive = true;
            traversalPawn.spinSlashing = true;

            // Start coroutine to maintain boosted speed
            boostCoroutine = player.StartCoroutine(ApplyBoost(currentDirection, originalSpeed));
        }
        else
        {
            Debug.Log("Player is stationary; no boost applied.");
        }
    }

    private IEnumerator ApplyBoost(Vector3 direction, float originalSpeed)
    {
        float boostedSpeed = originalSpeed * speedMultiplier;
        float elapsedTime = 0f;
        float directionTimer = 0f;
        // Define directions: North, East, South, West
        Vector3[] directions = {
        new Vector3(0, 0, 1),   // North
        new Vector3(1, 0, 0),   // East
        new Vector3(0, 0, -1),  // South
        new Vector3(-1, 0, 0)   // West
        };

        int currentDirectionIndex = 0;

        while (elapsedTime < speedBoostDuration)
        {
            // Apply boosted speed in the current direction
            playerRigidbody.velocity = direction * boostedSpeed;

            // Accumulate time for direction change
            directionTimer += Time.deltaTime;

            // Change direction every 0.2 seconds
            if (directionTimer >= directionChangeInterval)
            {
                // Update sprite facing direction and reset timer
                pawnSprite.FaceDirection(directions[currentDirectionIndex]);
                currentDirectionIndex = (currentDirectionIndex + 1) % directions.Length;
                directionTimer -= directionChangeInterval; // Prevent drift by subtracting instead of resetting to zero
            }

            // Break the loop if spin is canceled
            if (!traversalPawn.spinSlashing) { break; }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Reset speed after the boost duration
        playerRigidbody.velocity = direction * originalSpeed;
        traversalPawn.spinSlashing = false;
        isSpeedBoostActive = false;
    }
}
