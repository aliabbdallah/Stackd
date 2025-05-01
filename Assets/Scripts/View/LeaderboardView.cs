using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardView : MonoBehaviour
{
    public GameObject leaderboardPanel;
    public Transform leaderboardContent;
    public GameObject leaderboardEntryPrefab;
    public int maxEntriesToShow = 10;
    public TextMeshProUGUI statusText;

    public void ShowStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    public void ClearLeaderboard()
    {
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayLeaderboard(List<LeaderboardEntry> entries)
    {
        ClearLeaderboard();
        int count = Mathf.Min(entries.Count, maxEntriesToShow);
        if (count == 0)
        {
            ShowStatus("No leaderboard data available");
            return;
        }
        string currentUsername = PlayerPrefs.GetString("Username", "");
        for (int i = 0; i < count; i++)
        {
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();
            if (entryUI != null)
            {
                entryUI.SetEntry(entries[i].rank, entries[i].username, entries[i].score);
                if (!string.IsNullOrEmpty(currentUsername) && entries[i].username == currentUsername)
                {
                    Image background = entryObj.GetComponent<Image>();
                    if (background != null)
                    {
                        background.color = new Color(1f, 0.92f, 0.016f, 0.5f);
                    }
                }
            }
        }
        ShowStatus("");
    }
}


[System.Serializable]
public class LeaderboardEntry
{
    public string username;
    public int score;
    public int rank;
} 