using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public MainMenuView mainMenuView;

    void Awake()
    {
        if (mainMenuView != null)
        {
            mainMenuView.OnSummerClicked += () => SceneManager.LoadScene(4);
            mainMenuView.OnWinterClicked += () => SceneManager.LoadScene(5);
            mainMenuView.OnAutumnClicked += () => SceneManager.LoadScene(6);
            mainMenuView.OnMainClicked += () => SceneManager.LoadScene(3);
            mainMenuView.OnLoginClicked += () => SceneManager.LoadScene(1);
        }
    }
} 