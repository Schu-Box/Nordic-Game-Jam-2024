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
    public MMF_Player feedback_fadeInStartUI;
    public MMF_Player feedback_fadeOutStartUI;

    [Header("End UI")]
    public CanvasGroup endUI;
    public MMF_Player feedback_fadeInEndUI;
    public MMF_Player feedback_fadeOutEndUI;

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

    [Header("Variables")]
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
    
    // [Header("Music")]
    // public FMODUnity.StudioEventEmitter musicEmitter;
    
    private void Awake()
    {
        Instance = this;

        endUI.alpha = 0f;
        endUI.interactable = false;
        endUI.blocksRaycasts = false;
        
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

            startUI.alpha = 0f;
            feedback_fadeInStartUI.PlayFeedbacks();
        }
        
        PlayerPrefs.SetString("savedName", "");
    }

    private void Start()
    {
        LineManager.Instance.CreateLineBetweenDragPoints(starterGateLeft, starterGateRight, true);

        // ShowGame();
    }

    public void ShowGameFromMainMenu()
    {
        InputNewName();
        ShowGame();
    }

    private void InputNewName()
    {
        currentName = nameInputField.text;
    }

    public void ShowGame()
    {
        // startUI.SetActive(false);
        playerNameText.text = currentName;

        startUI.interactable = false;
        startUI.blocksRaycasts = false;
        feedback_fadeOutStartUI.PlayFeedbacks();
        
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
            SceneManager.LoadScene(0);
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
        
        feedback_fadeInEndUI.PlayFeedbacks();
        
        endUI.interactable = true;
        endUI.blocksRaycasts = true;
    }

    public void Retry()
    {
        PlayerPrefs.SetString("savedName", currentName);
        
        RestartScene();
    }

    public void RestartNewPlayer()
    {
        RestartScene();
    }

    private void RestartScene()
    {
        // Debug.Log("RESTART!");
        
        endUI.blocksRaycasts = false;
        
        feedback_fadeOutEndUI.PlayFeedbacks(); //Actual restart is called by the end of this feedback
    }
}
