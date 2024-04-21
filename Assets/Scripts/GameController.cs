using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [Header("Start UI")]
    public CanvasGroup startUI;
    public MMF_Player feedback_fadeStartUI;

    [Header("End UI")]
    public CanvasGroup endUI;
    public MMF_Player feedback_fadeEndUI;

    public Leaderboard endLeaderboard;

    [Header("Errythang else")]
    public MMF_Player feedback_breakStartingGate;
    
    public TextMeshProUGUI playerNameText;
    
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
    
    [Header("Name Entry")]
    public TMP_InputField nameInputField;
    
    private string savedName = "";
    
    public string currentName = "";
    
    [Header("Music")]
    public FMODUnity.StudioEventEmitter musicEmitter;
    
    private void Awake()
    {
        Instance = this;
        
        GameOverUI.SetActive(false);

        endUI.alpha = 0f;
        
        timer = timeLimit;
        timerText.text = timer.ToString("F1");
        
        savedName = PlayerPrefs.GetString("savedName");
        if (savedName != "")
        {
            Debug.Log("Coming in from save");
            
            currentName = savedName;

            startUI.gameObject.SetActive(false);
            
            ShowGame();
        }
        else
        {
            Debug.Log("was no save");
            
            // startUI.gameObject.SetActive(true);
        }
        
        PlayerPrefs.SetString("savedName", "");

        // nameInputField.interactable = true;
        // savedName = PlayerPrefs.GetString("savedName");
        // nameInputField.text = savedName;
    }
    
    public void SetName()
    {
        currentName = nameInputField.text;
        // PlayerPrefs.SetString("savedName", nameInputField.text);
        // savedName = PlayerPrefs.GetString("savedName");

        // nameInputField.interactable = false;
        playerNameText.text = currentName;
    }

    private void Start()
    {
        LineManager.Instance.CreateLineBetweenDragPoints(starterGateLeft, starterGateRight, true);

        // ShowGame();
    }

    public void ShowGame()
    {
        // startUI.SetActive(false);
        
        SetName();

        startUI.interactable = false;
        startUI.blocksRaycasts = false;
        feedback_fadeStartUI.PlayFeedbacks();
        
        Debug.Log("hiding all");
        
        fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/start_transition");
        fmodStudioEvent.start();
        fmodStudioEvent.release();

        screenFillSpawner.Spawn();
        
        screenFillSpawner.HideAllScreenFillers();
    }

    public void HideGame()
    {
        StartCoroutine(PlaySoundAfterDelay(1.5f));
        
        screenFillSpawner.ShowAllScreenFillers();
    }
    
    private IEnumerator PlaySoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/end_transition");
        fmodStudioEvent.start();
        fmodStudioEvent.release();
    }

    public void StartGame()
    {
        gameStarted = true;
        
        feedback_breakStartingGate.PlayFeedbacks();
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
        // GameOverUI.SetActive(true);
        gameOver = true;

        endLeaderboard.FinalizeScore();

        laserSpawner.TurnOff();
        
        HideGame();
        
        feedback_fadeEndUI.PlayFeedbacks();
    }

    public void Retry()
    {
        Debug.Log("Called this!");
        
        PlayerPrefs.SetString("savedName", currentName);
        
        SceneManager.LoadScene(0);
    }

    public void RestartNewPlayer()
    {
        SceneManager.LoadScene(0);
    }
}
