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
        // Check for saved session token
        string token = PlayerPrefs.GetString("SessionToken", "");
        
        if (string.IsNullOrEmpty(token))
        {
            // No session token, go to login
            Debug.Log("No saved session, redirecting to login");
            SceneManager.LoadScene(loginScene);
            yield break;
        }
        
        // Find API Manager
        APIManager apiManager = FindObjectOfType<APIManager>();
        if (apiManager == null)
        {
            Debug.LogError("APIManager not found! Redirecting to login");
            SceneManager.LoadScene(loginScene);
            yield break;
        }
        
        // Try to validate the session
        bool isValid = false;
        float timer = 0;
        
        // Start validation
        apiManager.StartCoroutine(apiManager.ValidateSession((valid) => {
            isValid = valid;
        }));
        
        // Wait for validation or timeout
        while (!isValid && timer < timeoutSeconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (isValid)
        {
            // Session is valid, go to main menu
            Debug.Log("Session valid, going to main menu");
            SceneManager.LoadScene(mainMenuScene);
        }
        else
        {
            // Invalid or expired session, go to login
            Debug.Log("Session invalid or expired, redirecting to login");
            SceneManager.LoadScene(loginScene);
        }
    }
}