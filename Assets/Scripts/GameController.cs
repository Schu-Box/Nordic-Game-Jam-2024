using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    public GameObject GameOverUI;

    public float timeLimit = 60f;

    private float timer;
    private int score;
    
    private void Awake()
    {
        Instance = this;
        
        GameOverUI.SetActive(false);
        
        timer = timeLimit;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        timerText.text = timer.ToString("F1");
        
        if(timer <= 0)
        {
            timer = 0;
            GameOver();
        }
        
        if(Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
        scoreText.text = score + "%";
    }

    public void GameOver()
    {
        GameOverUI.SetActive(true);
    }
}
