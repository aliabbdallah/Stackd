using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using GameStatePattern;
using System;

public class GameManager : MonoBehaviour
{
    
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnGameOver;
    public static event Action OnGameRestart;

    [Header("Block Settings")]
    public GameObject blockPrefab;
    public float SpawnDistanceAfterThirdBlock = 10f;  
    public float spawnY = 10f;  

    [Header("UI References")]
    public GameOverView gameOverView;
    public GameObject pausePanel; 
    
    [Header("Ground Reference")]
    public GameObject Ground; 
    
    
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
        
        if (currentBlock != null)
        {
            placedBlocks.Add(currentBlock);
            lastPlacedBlock = currentBlock;
            
            
            UpdateHighestBlockPosition();

            
            if (score == 0 && Ground != null)
            {
                Ground.SetActive(false);
            }
        }
        
        
        score++;
        OnScoreChanged?.Invoke(score);        
        
        StartCoroutine(SpawnBlockWithDelay());
    } 
    
    private IEnumerator SpawnBlockWithDelay()
    {
        
        yield return new WaitForSeconds(0.5f);
        
        
        SpawnNewBlock();
    }
    
    private void UpdateHighestBlockPosition()
    {
        
        foreach (GameObject block in placedBlocks)
        {
            if (block != null)
            {
                
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
        
        
        OnGameOver?.Invoke(score);

        
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
        
        StopAllCoroutines();
        
        
        if (currentBlock != null)
        {
            Destroy(currentBlock);
            currentBlock = null;
        }
        
        
        foreach (GameObject block in placedBlocks)
        {
            if (block != null)
            {
                Destroy(block);
            }
        }
        
        
        placedBlocks.Clear();
        lastPlacedBlock = null;
        highestBlockY = 0f;
        
        
        score = 0;
        OnScoreChanged?.Invoke(score);
        
        
        OnGameRestart?.Invoke();
        
        
        if (Ground != null)
        {
            Ground.SetActive(true);
        }
        
        
        gameStateContext.SetState(new PlayingState());
        
        
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                
                mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, cameraFollow.minY, mainCamera.transform.position.z);
            }
        }
        
        
        StartCoroutine(SpawnFirstBlockNextFrame());
    }

    private IEnumerator SpawnFirstBlockNextFrame()
    {
        yield return null; 
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