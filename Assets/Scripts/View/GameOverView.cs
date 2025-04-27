using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GameOverView : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public Button restartButton;
    public Button mainMenuButton;

    public event Action OnRestartClicked;
    public event Action OnMainMenuClicked;

    public void Show(int finalScore)
    {
        if (finalScoreText != null)
            finalScoreText.text = "Final Score: " + finalScore;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void Hide()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Start()
    {
        if (restartButton != null) restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(() => OnMainMenuClicked?.Invoke());
    }
} 