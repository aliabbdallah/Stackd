using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMainMenu : MonoBehaviour
{
    public void GoToMain()
    {
        SceneManager.LoadScene(3); // Or use SceneManager.LoadScene("Main Menu"); if you prefer by name
    }
} 