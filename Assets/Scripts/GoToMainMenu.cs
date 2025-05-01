using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMainMenu : MonoBehaviour
{
    public void GoToMain()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null && Time.timeScale == 0f)
        {
            gameManager.TogglePause();
        }
        else
        {
            Time.timeScale = 1f;
        }

        SceneManager.LoadScene(3);
    }
} 