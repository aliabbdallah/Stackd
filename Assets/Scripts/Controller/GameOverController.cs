using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public GameOverView gameOverView;
    public GameManager gameManager;

    void Awake()
    {
        if (gameOverView != null)
        {
            gameOverView.OnRestartClicked += () => {
                if (gameManager != null) gameManager.RestartGame();
                gameOverView.Hide();
            };
            gameOverView.OnMainMenuClicked += () => {
                SceneManager.LoadScene(3); 
            };
        }
    }
} 