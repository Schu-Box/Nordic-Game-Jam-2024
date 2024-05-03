using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct HighScoreData
{
    public MapType map;
    public ModeType mode;
    
    public int rank;
    public string playerName;
    public int playerScore;

    public bool isMostRecent;
}

public class Leaderboard : MonoBehaviour
{
    public int numScoresOnLeaderboard = 10;

    public MapType mapType = MapType.Circle;
    public ModeType modeType = ModeType.Timed;

    public Transform leaderboardEntryParent;
    public LeaderboardEntry leaderboardEntryPrefab;

    private string highScoreMapPrefix = "highScore_Map";
    private string highScoreModePrefix = "highScore_Mode";
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
    
    //TODO: Fix issue where circle and donut leaderboards aren't working properly
    
    public void FinalizeScore()
    {
        List<HighScoreData> topScores = LoadTopXScores(numScoresOnLeaderboard - 1);

        HighScoreData currentData = new HighScoreData
        {
            mode = GameController.Instance.currentMode,
            map = GameController.Instance.currentMap,
            playerName = GameController.Instance.currentName,
            playerScore = GameController.Instance.score,
            rank = 999,
            isMostRecent = true,
        };
        
        topScores.Add(currentData);
        
        // sort from highest to lowest
        topScores.Sort((data1, data2) => data1.playerScore.CompareTo(data2.playerScore));
        topScores.Reverse();

        //TODO: Reorder them I guess?
        
        for (int i = 0; i < numScoresOnLeaderboard; i++)
        {
            if (i >= topScores.Count)
            {
                break;
            }
            
            string mapKey = $"{highScoreMapPrefix}{i}";
            string modeKey = $"{highScoreModePrefix}{i}";
            string playerNameKey = $"{highScoreNamePrefix}{i}";
            string rankKey = $"{highScoreRankPrefix}{i}";
            string pointsKey = $"{highScorePointsPrefix}{i}";
        
            HighScoreData rankedData = topScores[i];
            
            PlayerPrefs.SetInt(rankKey, i + 1);
            PlayerPrefs.SetString(playerNameKey, rankedData.playerName);
            PlayerPrefs.SetInt(pointsKey, rankedData.playerScore);
            
            PlayerPrefs.SetString(mapKey, rankedData.map.ToString());
            PlayerPrefs.SetString(modeKey, rankedData.mode.ToString());
        }

        // int i = LoadTopXScores(9999).Count + 1;
        //
        // string mapKey = $"{highScoreMapPrefix}{i}";
        // string modeKey = $"{highScoreModePrefix}{i}";
        // string playerNameKey = $"{highScoreNamePrefix}{i}";
        // string rankKey = $"{highScoreRankPrefix}{i}";
        // string pointsKey = $"{highScorePointsPrefix}{i}";
        //
        // PlayerPrefs.SetInt(rankKey, i);
        // PlayerPrefs.SetString(playerNameKey, currentData.playerName);
        // PlayerPrefs.SetInt(pointsKey, currentData.playerScore);
        //     
        // PlayerPrefs.SetString(mapKey, currentData.map.ToString());
        // PlayerPrefs.SetString(modeKey, currentData.mode.ToString());
        
        ShowLeaderboard(topScores);
    }

    public List<HighScoreData> LoadTopXScores(int numScores)
    {
        List<HighScoreData> topScoreData = new();
        
        Debug.Log("Loading top " + numScores + " scores.");
        
        for (int i = 0; i < numScores; i++)
        {
            string mapKey = $"{highScoreMapPrefix}{i}";
            string modeKey = $"{highScoreModePrefix}{i}";
            string playerNameKey = $"{highScoreNamePrefix}{i}";
            string rankKey = $"{highScoreRankPrefix}{i}";
            string pointsKey = $"{highScorePointsPrefix}{i}";

            if (!PlayerPrefs.HasKey(playerNameKey))
            {
                Debug.Log("No key found for " + playerNameKey + ". Skipping.");
                continue;
            }

            HighScoreData highScoreData = new();
            
            highScoreData.map = (MapType)Enum.Parse(typeof(MapType), PlayerPrefs.GetString(mapKey));
            highScoreData.mode = (ModeType)Enum.Parse(typeof(ModeType), PlayerPrefs.GetString(modeKey));

            if (mapType != highScoreData.map || modeType != highScoreData.mode)
            {
                Debug.Log("Skipping score because it is for a different map or mode.");
                continue;
            }

            highScoreData.playerName = PlayerPrefs.GetString(playerNameKey);
            highScoreData.playerScore = PlayerPrefs.GetInt(pointsKey);
            highScoreData.rank = PlayerPrefs.GetInt(rankKey);
            highScoreData.isMostRecent = false;
            
            topScoreData.Add(highScoreData);
        }

        return topScoreData;
    }

    public void ShowLeaderboard(List<HighScoreData> topScores)
    {
        Debug.Log("Showing leaderboard with num scores: " + topScores.Count);
        
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
    
    public void SetMapAndMode(MapType map, ModeType mode)
    {
        mapType = map;
        modeType = mode;
        
        ShowLeaderboard(LoadTopXScores(numScoresOnLeaderboard));
    }
}
