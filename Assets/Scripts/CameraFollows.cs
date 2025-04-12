using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public float smoothSpeed = 10.0f;  // Higher value for faster camera movement
    public float heightOffset = 3.0f;  // How much higher the camera should be from the highest block
    public float minY = 0f;  // Minimum height for the camera (starting position)
    public bool centerLastPlacedBlock = true;  // Center on the last placed block instead of the current one
    public int blocksBeforeFollowing = 3;  // Number of blocks to place before camera starts following
    public float verticalOffset = -4.0f;  // Offset from center (negative = lower on screen)

    private Vector3 initialPosition;
    private float targetY;
    private GameManager gameManager;

    void Start()
    {
        initialPosition = transform.position;
        targetY = initialPosition.y;
        gameManager = FindObjectOfType<GameManager>();
    }

    void LateUpdate()
    {
        // Only start following after specified number of blocks have been placed
        if (gameManager != null && gameManager.score >= blocksBeforeFollowing)
        {
            // Update the target position based on the highest block
            UpdateTargetPosition();
            
            // Create the target position with current x and z values but updated y value
            Vector3 desiredPosition = new Vector3(
                transform.position.x,
                Mathf.Max(targetY, minY),  // Never go below the minimum height
                transform.position.z
            );
            
            // More responsive camera movement with lerp
            transform.position = Vector3.Lerp(
                transform.position, 
                desiredPosition, 
                smoothSpeed * Time.deltaTime
            );
            
            // If the camera is very close to the target, snap to it
            if (Vector3.Distance(transform.position, desiredPosition) < 0.05f)
            {
                transform.position = desiredPosition;
            }
        }
    }

    void UpdateTargetPosition()
    {
        if (gameManager != null)
        {
            // Get the height of the highest block (last placed block)
            float highestBlockY = gameManager.GetHighestBlockY();
            
            // If we're centering on the last placed block
            if (centerLastPlacedBlock && gameManager.GetLastPlacedBlock() != null)
            {
                // Get the last placed block's position
                float lastPlacedBlockY = gameManager.GetLastPlacedBlock().transform.position.y;
                
                // Set target Y position to center the camera on the last placed block
                // Add the vertical offset to position the tower below center
                targetY = lastPlacedBlockY + verticalOffset;
            }
            else
            {
                // Otherwise, follow the highest block with offset
                targetY = highestBlockY + heightOffset + verticalOffset;
            }
        }
    }
}