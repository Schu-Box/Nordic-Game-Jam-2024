using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public LaserSpawner laserSpawner;
    
    public DragPoint starterGateLeft;
    public DragPoint starterGateRight;
    
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;

    public MMF_Player feedback_lowTime;

    public GameObject GameOverUI;

    public float timeLimit = 60f;

    public float timer;
    public int score;

    public bool gameStarted = false;
    public bool gameOver = false;

    public float timeRemainingWhenBeepsStart = 10f;
    public float timeBetweenBeeps = 1f;
    private float timeUntilBeepTimer = 0f;

    public ScreenFillSpawner screenFillSpawner;
    
    private void Awake()
    {
        Instance = this;
        
        GameOverUI.SetActive(false);
        
        timer = timeLimit;
    }

    private void Start()
    {
        LineManager.Instance.CreateLineBetweenDragPoints(starterGateLeft, starterGateRight, true);

        ShowGame();
    }

    public void ShowGame()
    {
        Debug.Log("hiding all");

        screenFillSpawner.Spawn();
        
        screenFillSpawner.HideAllScreenFillers();
    }

    public void HideGame()
    {
        screenFillSpawner.ShowAllScreenFillers();
    }

    public void StartGame()
    {
        gameStarted = true;
        
        Leaderboard.Instance.SetName();
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

            return;
        }
        
        if(timer <= timeRemainingWhenBeepsStart)
        {
            timeUntilBeepTimer -= Time.deltaTime;
            if(timeUntilBeepTimer <= 0)
            {
                feedback_lowTime.PlayFeedbacks();
                
                timeUntilBeepTimer = timeBetweenBeeps;
            }
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

        Leaderboard.Instance.FinalizeScore();

        laserSpawner.TurnOff();
        
        HideGame();
    }
}
