using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class LeaderboardController : MonoBehaviour
{
    public LeaderboardView leaderboardView;
    public string baseUrl = "http://localhost:3000";
    public int maxEntriesToShow = 10;

    private List<LeaderboardEntry> cachedLeaderboard = new List<LeaderboardEntry>();

    public void LoadLeaderboard(int playerScore)
    {
        Debug.Log("[LeaderboardController] Loading leaderboard with score: " + playerScore);
        if (leaderboardView != null)
            leaderboardView.ShowStatus("Loading leaderboard...");
        StartCoroutine(FetchLeaderboard(playerScore));
    }

    IEnumerator FetchLeaderboard(int playerScore)
    {
        
        yield return StartCoroutine(SubmitScore(playerScore));

        
        UnityWebRequest request = UnityWebRequest.Get(baseUrl + "/leaderboard");
        yield return request.SendWebRequest();

        Debug.Log("[LeaderboardController] Raw leaderboard API response: " + request.downloadHandler.text);

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Leaderboard data received: " + request.downloadHandler.text);
            try {
                List<LeaderboardEntry> entries = ParseLeaderboardResponse(request.downloadHandler.text);
                leaderboardView.DisplayLeaderboard(entries);
                cachedLeaderboard = entries;
            }
            catch (System.Exception e) {
                Debug.LogError("Error parsing leaderboard data: " + e.Message);
                leaderboardView.ShowStatus("Could not load leaderboard data");
                if (cachedLeaderboard.Count > 0)
                    leaderboardView.DisplayLeaderboard(cachedLeaderboard);
            }
        }
        else
        {
            leaderboardView.ShowStatus("Failed to load leaderboard");
        }
    }

    IEnumerator SubmitScore(int score)
    {
        string username = PlayerPrefs.GetString("Username", "Player");
        string sessionToken = PlayerPrefs.GetString("SessionToken", "");
        string json = "{\"username\":\"" + username + "\",\"score\":" + score + "}";
        UnityWebRequest request = new UnityWebRequest(baseUrl + "/scores", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        if (!string.IsNullOrEmpty(sessionToken))
            request.SetRequestHeader("Authorization", "Bearer " + sessionToken);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error submitting score: " + request.error);
        }
    }

    private List<LeaderboardEntry> ParseLeaderboardResponse(string json)
    {
        Debug.Log("[LeaderboardController] Parsing leaderboard response: " + json);
        
        if (json.StartsWith("["))
        {
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>("{\"entries\":" + json + "}");
            Debug.Log("[LeaderboardController] Parsed " + (data.entries != null ? data.entries.Count : 0) + " entries from array response.");
            return data.entries;
        }
        else
        {
            
            int dataStartIndex = json.IndexOf("\"data\":");
            if (dataStartIndex == -1) throw new System.Exception("No data field in response");
            dataStartIndex += 7;
            int dataEndIndex = json.LastIndexOf("]") + 1;
            string dataArrayJson = json.Substring(dataStartIndex, dataEndIndex - dataStartIndex);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>("{\"entries\":" + dataArrayJson + "}");
            Debug.Log("[LeaderboardController] Parsed " + (data.entries != null ? data.entries.Count : 0) + " entries from object response.");
            return data.entries;
        }
    }

    [System.Serializable]
    private class LeaderboardData
    {
        public List<LeaderboardEntry> entries;
    }
} 