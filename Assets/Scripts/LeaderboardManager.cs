using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Text;

public class LeaderboardManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject leaderboardPanel;
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    public int maxEntriesToShow = 10;
    public TextMeshProUGUI statusText; // For showing loading/error messages
    
    [Header("API Settings")]
    public string baseUrl = "http://localhost:3000";
    
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string username;
        public int score;
        public int rank;
    }
    
    [System.Serializable]
    public class LeaderboardData
    {
        public List<LeaderboardEntry> entries;
    }
    
    private List<LeaderboardEntry> cachedLeaderboard = new List<LeaderboardEntry>();
    
    public void LoadLeaderboard(int playerScore)
    {
        if (statusText != null)
            statusText.text = "Loading leaderboard...";
            
        StartCoroutine(FetchLeaderboard(playerScore));
    }
    
IEnumerator FetchLeaderboard(int playerScore)
{
    // Submit the current score
    yield return StartCoroutine(SubmitScore(playerScore));
    
    // Fetch the updated leaderboard
    UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/leaderboard");
    yield return request.SendWebRequest();
    
    if (request.result == UnityWebRequest.Result.Success)
    {
        Debug.Log("Leaderboard data received: " + request.downloadHandler.text);
        
        try {
            // Check if the response is in the expected format (your API returns rows directly)
            if (request.downloadHandler.text.StartsWith("["))
            {
                // Parse as array directly
                LeaderboardData data = JsonUtility.FromJson<LeaderboardData>("{\"entries\":" + request.downloadHandler.text + "}");
                DisplayLeaderboard(data.entries);
                cachedLeaderboard = data.entries;
            }
            else 
            {
                // Handle your API format which returns {success: true, data: [...]}
                string jsonResponse = request.downloadHandler.text;
                // Extract just the data array
                int dataStartIndex = jsonResponse.IndexOf("\"data\":[") + 7;
                int dataEndIndex = jsonResponse.LastIndexOf("]") + 1;
                string dataArrayJson = jsonResponse.Substring(dataStartIndex, dataEndIndex - dataStartIndex);
                
                LeaderboardData data = JsonUtility.FromJson<LeaderboardData>("{\"entries\":" + dataArrayJson + "}");
                DisplayLeaderboard(data.entries);
                cachedLeaderboard = data.entries;
            }
            
            if (statusText != null)
                statusText.text = "";
        }
        catch (System.Exception e) {
            Debug.LogError("Error parsing leaderboard data: " + e.Message);
            if (statusText != null)
                statusText.text = "Could not load leaderboard data";
            
            // Fall back to cached leaderboard
            if (cachedLeaderboard.Count > 0)
                DisplayLeaderboard(cachedLeaderboard);
        }
    }
}    IEnumerator SubmitScore(int score)
    {
        // Get current username (stored during login)
        string username = PlayerPrefs.GetString("Username", "Player");
        string sessionToken = PlayerPrefs.GetString("SessionToken", "");
        
        // Create the JSON payload
        string json = "{\"username\":\"" + username + "\",\"score\":" + score + "}";
        
        UnityWebRequest request = new UnityWebRequest(baseUrl + "/scores", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        // Add auth token if available
        if (!string.IsNullOrEmpty(sessionToken))
            request.SetRequestHeader("Authorization", "Bearer " + sessionToken);
        
        yield return request.SendWebRequest();
        
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error submitting score: " + request.error);
        }
    }
    
    void DisplayLeaderboard(List<LeaderboardEntry> entries)
    {
        // Clear existing entries
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
        
        // Limit entries to display
        int count = Mathf.Min(entries.Count, maxEntriesToShow);
        
        if (count == 0)
        {
            if (statusText != null)
                statusText.text = "No leaderboard data available";
            return;
        }
        
        // Get current username for highlighting
        string currentUsername = PlayerPrefs.GetString("Username", "");
        
        // Create entry for each score
        for (int i = 0; i < count; i++)
        {
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();
            
            if (entryUI != null)
            {
                entryUI.SetEntry(entries[i].rank, entries[i].username, entries[i].score);
                
                // Highlight player's own score
                if (!string.IsNullOrEmpty(currentUsername) && entries[i].username == currentUsername)
                {
                    // Get the background image of the entry
                    Image background = entryObj.GetComponent<Image>();
                    if (background != null)
                    {
                        background.color = new Color(1f, 0.92f, 0.016f, 0.5f); // Gold highlight
                    }
                }
            }
        }
    }
}