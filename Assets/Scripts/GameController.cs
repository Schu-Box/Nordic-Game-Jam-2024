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
    
    private FMOD.Studio.EventInstance fmodStudioEvent;
    
    private void Awake()
    {
        Instance = this;
        
        GameOverUI.SetActive(false);
        
        timer = timeLimit;
        timerText.text = timer.ToString("F1");
    }

    private void Start()
    {
        LineManager.Instance.CreateLineBetweenDragPoints(starterGateLeft, starterGateRight, true);

        ShowGame();
    }

    public void ShowGame()
    {
        Debug.Log("hiding all");
        
        // fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/show_game");
        // fmodStudioEvent.start();
        // fmodStudioEvent.release();

        screenFillSpawner.Spawn();
        
        screenFillSpawner.HideAllScreenFillers();
    }

    public void HideGame()
    {
        // fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/show_game");
        // fmodStudioEvent.start();
        // fmodStudioEvent.release();
        
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
            
            fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/timer_final");
            fmodStudioEvent.start();
            fmodStudioEvent.release();

            return;
        }
        
        if(timer <= timeRemainingWhenBeepsStart)
        {
            timeUntilBeepTimer -= Time.deltaTime;
            if(timeUntilBeepTimer <= 0)
            {
                feedback_lowTime.PlayFeedbacks();
                
                timeUntilBeepTimer = timeBetweenBeeps;
                
                fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/timer_beep");
                fmodStudioEvent.start();
                fmodStudioEvent.release();
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
