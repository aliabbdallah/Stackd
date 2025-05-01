using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public float smoothSpeed = 10.0f;  
    public float heightOffset = 3.0f;  
    public float minY = 0f;  
    public bool centerLastPlacedBlock = true;  
    public int blocksBeforeFollowing = 3;  
    public float verticalOffset = -4.0f;  

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
        
        if (gameManager != null && gameManager.score >= blocksBeforeFollowing)
        {
            
            UpdateTargetPosition();
            
            
            Vector3 desiredPosition = new Vector3(
                transform.position.x,
                Mathf.Max(targetY, minY),  
                transform.position.z
            );
            
            
            transform.position = Vector3.Lerp(
                transform.position, 
                desiredPosition, 
                smoothSpeed * Time.deltaTime
            );
            
            
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
            
            float highestBlockY = gameManager.GetHighestBlockY();
            
            
            if (centerLastPlacedBlock && gameManager.GetLastPlacedBlock() != null)
            {
                
                float lastPlacedBlockY = gameManager.GetLastPlacedBlock().transform.position.y;
                
                
                
                targetY = lastPlacedBlockY + verticalOffset;
            }
            else
            {
                
                targetY = highestBlockY + heightOffset + verticalOffset;
            }
        }
    }
}