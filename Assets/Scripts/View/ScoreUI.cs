using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    [SerializeField] private GameObject gameOverPanel;
    
    private void OnEnable()
    {
        // Subscribe to events
        GameManager.OnScoreChanged += UpdateScore;
        GameManager.OnGameOver += ShowGameOverScreen;
        GameManager.OnGameRestart += HideGameOverScreen;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from events
        GameManager.OnScoreChanged -= UpdateScore;
        GameManager.OnGameOver -= ShowGameOverScreen;
        GameManager.OnGameRestart -= HideGameOverScreen;
    }
    
    private void UpdateScore(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score:{newScore}";
        }
    }
    
    private void ShowGameOverScreen(int finalScore)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = $"Final Score: {finalScore}";
        }
    }
    
    private void HideGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }
} 