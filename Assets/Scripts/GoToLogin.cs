using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToLogin : MonoBehaviour
{
    public void LoadLoginScene()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadSignUpScene()
    {
        SceneManager.LoadScene(2);
    }
    
}
