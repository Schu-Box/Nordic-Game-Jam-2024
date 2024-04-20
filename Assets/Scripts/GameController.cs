using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public DragPoint starterGateLeft;
    public DragPoint starterGateRight;
    
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    public GameObject GameOverUI;

    public float timeLimit = 60f;

    public float timer;
    public int score;

    public bool gameStarted = false;
    public bool gameOver = false;
    
    private void Awake()
    {
        Instance = this;
        
        GameOverUI.SetActive(false);
        
        timer = timeLimit;
    }

    private void Start()
    {
        LineManager.Instance.CreateLineBetweenDragPoints(starterGateLeft, starterGateRight);
    }

    public void StartGame()
    {
        gameStarted = true;
        
        //TODO: Lock playerName
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        
        if (!gameStarted || gameOver)
            return;
        
        timer -= Time.deltaTime;
        timerText.text = timer.ToString("F1");
        
        if(timer <= 0)
        {
            timer = 0;
            GameOver();
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
        gameOver = true;

        Leaderboard.Instance.FinalizeScore("placeholder");
    }
}
