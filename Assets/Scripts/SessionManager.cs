using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Game;

public class SessionManager : MonoBehaviour
{
    public string loginScene = "Log In";
    public string mainMenuScene = "Main Menu";
    public float timeoutSeconds = 5.0f;
    
    void Start()
    {
        StartCoroutine(CheckSession());
    }
    
    IEnumerator CheckSession()
    {
        
        string token = PlayerPrefs.GetString("SessionToken", "");
        
        if (string.IsNullOrEmpty(token))
        {
            
            Debug.Log("No saved session, redirecting to login");
            SceneManager.LoadScene(loginScene);
            yield break;
        }
        
        
        APIManager apiManager = FindObjectOfType<APIManager>();
        if (apiManager == null)
        {
            Debug.LogError("APIManager not found! Redirecting to login");
            SceneManager.LoadScene(loginScene);
            yield break;
        }
        
        
        bool isValid = false;
        float timer = 0;
        
        
        apiManager.StartCoroutine(apiManager.ValidateSession((valid) => {
            isValid = valid;
        }));
        
        
        while (!isValid && timer < timeoutSeconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (isValid)
        {
            
            Debug.Log("Session valid, going to main menu");
            SceneManager.LoadScene(mainMenuScene);
        }
        else
        {
            
            Debug.Log("Session invalid or expired, redirecting to login");
            SceneManager.LoadScene(loginScene);
        }
    }
}