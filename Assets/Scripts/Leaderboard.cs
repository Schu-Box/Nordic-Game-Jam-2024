using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct HighScoreData
{
    public int rank;
    public string playerName;
    public int playerScore;
}

public class Leaderboard : MonoBehaviour
{
    public int numScoresOnLeaderboard = 10;

    public Transform leaderboardEntryParent;
    public LeaderboardEntry leaderboardEntryPrefab;


    private string highScoreNamePrefix = "highScore_PlayerName";
    
    private string highScorePointsPrefix = "highScore_PlayerScore";

    private string highScoreTimePrefix = "highScore_PlayerTime";
    
    private string highScoreRankPrefix = "highScore_PlayerRank";
    

    private void Awake()
    {
        ShowLeaderboard(LoadTopXScores(numScoresOnLeaderboard));
    }

    [ContextMenu("Clear Scores")]
    public void ClearScores()
    {
        PlayerPrefs.DeleteAll();
    }
    
    public void FinalizeScore()
    {
        List<HighScoreData> topScores = LoadTopXScores(numScoresOnLeaderboard - 1);

        HighScoreData currentPlayer = new HighScoreData
        {
            playerName = GameController.Instance.currentName,
            playerScore = GameController.Instance.score,
            rank = 999,
        };
        
        topScores.Add(currentPlayer);
        
        // sort from highest to lowest
        topScores.Sort((data1, data2) => data1.playerScore.CompareTo(data2.playerScore));
        topScores.Reverse();

        for (int i = 0; i < numScoresOnLeaderboard; i++)
        {
            if (i >= topScores.Count)
            {
                break;
            }
            
            string playerNameKey = $"{highScoreNamePrefix}{i}";
            string rankKey = $"{highScoreRankPrefix}{i}";
            string pointsKey = $"{highScorePointsPrefix}{i}";

            HighScoreData currentData = topScores[i];
            
            PlayerPrefs.SetInt(rankKey, i + 1);
            PlayerPrefs.SetString(playerNameKey, currentData.playerName);
            PlayerPrefs.SetInt(pointsKey, currentData.playerScore);
        }
        
        ShowLeaderboard(topScores);
    }

    public List<HighScoreData> LoadTopXScores(int numScores)
    {
        List<HighScoreData> topScoreData = new();
        
        for (int i = 0; i < numScores; i++)
        {
            string playerNameKey = $"{highScoreNamePrefix}{i}";
            string rankKey = $"{highScoreRankPrefix}{i}";
            string pointsKey = $"{highScorePointsPrefix}{i}";

            if (!PlayerPrefs.HasKey(playerNameKey))
            {
                continue;
            }

            HighScoreData highScoreData = new();

            highScoreData.playerName = PlayerPrefs.GetString(playerNameKey);
            highScoreData.playerScore = PlayerPrefs.GetInt(pointsKey);
            highScoreData.rank = PlayerPrefs.GetInt(rankKey);
            
            topScoreData.Add(highScoreData);
        }

        return topScoreData;
    }

    public void ShowLeaderboard(List<HighScoreData> topScores)
    {
        //destroy all children of leaderboardEntryParent
        foreach (Transform child in leaderboardEntryParent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (HighScoreData highScore in topScores)
        {
            LeaderboardEntry newEntry = Instantiate(leaderboardEntryPrefab, leaderboardEntryParent);
            newEntry.ShowScore(highScore);   
        }
    }
}
