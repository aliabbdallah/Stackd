using UnityEngine;
using TMPro;

public class LeaderboardEntryUI : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI scoreText;
    
    public void SetEntry(int rank, string username, int score)
    {
        if (rankText != null)
            rankText.text = "#" + rank.ToString();
        
        if (usernameText != null)
            usernameText.text = username;
        
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
}