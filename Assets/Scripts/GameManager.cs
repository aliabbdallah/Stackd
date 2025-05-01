using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using GameStatePattern;
using System;

public class GameManager : MonoBehaviour
{
    // Events for the Observer Pattern
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnGameOver;
    public static event Action OnGameRestart;

    [Header("Block Settings")]
    public GameObject blockPrefab;
    public float SpawnDistanceAfterThirdBlock = 10f;  // Constant distance above camera position
    public float spawnY = 10f;  // Adjust this value as needed

    [Header("UI References")]
    public GameOverView gameOverView;
    public GameObject pausePanel; // Assign this in the Inspector
    
    [Header("Ground Reference")]
    public GameObject Ground; // Reference to the ground GameObject
    
    // Changed to public so the camera can access it
    public GameObject currentBlock;
    private List<GameObject> placedBlocks = new List<GameObject>();
    private GameObject lastPlacedBlock;
    private float highestBlockY = 0f;
    public int score = 0;
    public bool isGameActive = true;
    
    private GameStateContext gameStateContext;
    
    void Start()
    {
        gameStateContext = new GameStateContext(this);
        gameStateContext.SetState(new PlayingState());
        SpawnNewBlock();
    }
    
    void Update()
    {
        if (gameStateContext != null)
        {
            gameStateContext.Update();
        }
    }
    
    public void SpawnNewBlock()
    {
        if (!isGameActive) return;
        
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;
        
        CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
        float verticalOffset = 0f;
        if (cameraFollow != null)
        {
            verticalOffset = cameraFollow.verticalOffset;
        }
        
        if (score < 3)
        {
            // Fixed spawn position for the first few blocks
            Vector3 spawnPosition = new Vector3(0f, spawnY, 0f);
            currentBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            float spawnY = mainCamera.transform.position.y + SpawnDistanceAfterThirdBlock; 
            Vector3 spawnPosition = new Vector3(0f, spawnY, 0f);
            currentBlock = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        }
    }
    
    public void BlockSettled()
    {
        // Add the current block to our list of placed blocks
        if (currentBlock != null)
        {
            placedBlocks.Add(currentBlock);
            lastPlacedBlock = currentBlock;
            
            // Update highest block position
            UpdateHighestBlockPosition();

            // Deactivate ground after first block is placed
            if (score == 0 && Ground != null)
            {
                Ground.SetActive(false);
            }
        }
        
        // Increment score and notify observers
        score++;
        OnScoreChanged?.Invoke(score);        
        // Spawn next block with a delay to let the camera adjust
        StartCoroutine(SpawnBlockWithDelay());
    } 
    
    private IEnumerator SpawnBlockWithDelay()
    {
        // Wait for the camera to adjust
        yield return new WaitForSeconds(0.5f);
        
        // Now spawn the next block
        SpawnNewBlock();
    }
    
    private void UpdateHighestBlockPosition()
    {
        // Find the highest Y position among all placed blocks
        foreach (GameObject block in placedBlocks)
        {
            if (block != null)
            {
                // Use the top of the block
                Renderer renderer = block.GetComponent<Renderer>();
                if (renderer != null)
                {
                    float blockTopY = block.transform.position.y + (renderer.bounds.size.y / 2);
                    highestBlockY = Mathf.Max(highestBlockY, blockTopY);
                }
            }
        }
    }
    
    public float GetHighestBlockY()
    {
        return highestBlockY;
    }
    
    public GameObject GetLastPlacedBlock()
    {
        return lastPlacedBlock;
    }
    
    public void GameOver()
    {
        Debug.Log("Game Over! Final Score: " + score);
        gameStateContext.SetState(new GameOverState());
        
        // Notify observers about game over
        OnGameOver?.Invoke(score);

        // Update the leaderboard
        LeaderboardController leaderboardController = GetComponent<LeaderboardController>();
        if (leaderboardController == null)
        {
            leaderboardController = FindObjectOfType<LeaderboardController>();
        }
        if (leaderboardController != null)
        {
            Debug.Log("[GameManager] Calling LoadLeaderboard with score: " + score);
            leaderboardController.LoadLeaderboard(score);
        }
        else
        {
            Debug.LogWarning("[GameManager] No LeaderboardController found in the scene.");
        }
    }
    
    public void RestartGame()
    {
        // Stop any running coroutines that might spawn blocks
        StopAllCoroutines();
        
        // Destroy the current moving block
        if (currentBlock != null)
        {
            Destroy(currentBlock);
            currentBlock = null;
        }
        
        // Clear existing placed blocks
        foreach (GameObject block in placedBlocks)
        {
            if (block != null)
            {
                Destroy(block);
            }
        }
        
        // Clear lists and reset variables
        placedBlocks.Clear();
        lastPlacedBlock = null;
        highestBlockY = 0f;
        
        // Reset score
        score = 0;
        OnScoreChanged?.Invoke(score);
        
        // Notify observers about game restart
        OnGameRestart?.Invoke();
        
        // Reactivate the ground
        if (Ground != null)
        {
            Ground.SetActive(true);
        }
        
        // Reactivate the game state
        gameStateContext.SetState(new PlayingState());
        
        // Reset camera position
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                // Reset camera to initial position
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, cameraFollow.minY, mainCamera.transform.position.z);
            }
        }
        
        // Wait a frame before spawning new block to ensure all cleanup is complete
        StartCoroutine(SpawnFirstBlockNextFrame());
    }

    private IEnumerator SpawnFirstBlockNextFrame()
    {
        yield return null; // Wait one frame
        SpawnNewBlock();
    }

    public void TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            gameStateContext.SetState(new PlayingState());
        }
        else
        {
            gameStateContext.SetState(new PauseState());
        }
    }
}