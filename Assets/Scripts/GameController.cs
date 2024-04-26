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
    public GameObject blackBackground;
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
    
    public ScreenFillSpawner screenFillSpawner;

    [Header("Gameplay")]
    public Transform dragPointParent;
    public DragPoint dragPointPrefab;
    public List<DragPoint> dragPointList = new List<DragPoint>();
    
    public float dragPointSpawnRadius;
    public Vector2Int dragPointCountRange;
    public float minDistanceBetweenDragPoints = 1f;

    [Header("Variables")]
    public float timeLimit = 60f;

    public int numUnstableDragPoints = 2;
    
    public float timeRemainingWhenBeepsStart = 10f;
    public float timeBetweenBeeps = 1f;
    private float timeUntilBeepTimer = 0f;

    [Header("Should be private")]
    public float timer;
    public int score;

    private bool gameShown = false;
    public bool GameShown => gameShown;
    
    private bool gameStarted = false;
    private bool gameOver = false;
    
    [Header("Name Entry")]
    public TMP_InputField nameInputField;
    
    private string savedName = "";
    
    public string currentName = "";

    private void Awake()
    {
        blackBackground.SetActive(true);
        
        Instance = this;

        endUI.alpha = 0f;
        endUI.interactable = false;
        endUI.blocksRaycasts = false;
        
        timer = timeLimit;
        timerText.text = timer.ToString("F1");
        
        savedName = PlayerPrefs.GetString("savedName");
        if (savedName != "")
        {
            // Debug.Log("Coming in from save");
            
            currentName = savedName;

            startUI.gameObject.SetActive(false);
            
            ShowGame();
        }
        else
        {
            // Debug.Log("was no save");

            startUI.alpha = 0f;
            feedback_fadeInStartUI.PlayFeedbacks();
        }
        
        PlayerPrefs.SetString("savedName", "");
    }

    private void Start()
    {
        
        
        // ShowGame();
    }

    private void ClearNonStartingDragPoints()
    {
        Debug.Log("Generating new drag points");
        //Debug purposes
        
        //destroy all dragPoints
        for(int i = dragPointList.Count - 1; i >= 0; i--)
        {
            DragPoint dragPoint = dragPointList[i];
            
            if(dragPoint == starterGateLeft || dragPoint == starterGateRight)
                continue;
            
            RemoveDragPoint(dragPoint);
        }
    }

    public void ShowGameFromMainMenu()
    {
        InputNewName();
        ShowGame();
        
        AudioManager.Instance.PlayEvent("event:/start_button_press");
    }

    private void InputNewName()
    {
        currentName = nameInputField.text;
    }

    public void ShowGame()
    {
        gameShown = true;
        // startUI.SetActive(false);
        playerNameText.text = currentName;
        
        blackBackground.SetActive(false);
        
        startUI.interactable = false;
        startUI.blocksRaycasts = false;
        feedback_fadeOutStartUI.PlayFeedbacks();

        screenFillSpawner.Spawn();
        
        screenFillSpawner.HideAllScreenFillers();
        
        AudioManager.Instance.PlayEvent("event:/start_transition");
        
        
        LineManager.Instance.CreateLineBetweenDragPoints(starterGateLeft, starterGateRight, true);
        
        ClearNonStartingDragPoints();
        SpawnNewDragPoints();
        
        Debug.Log("Check dis");
        for(int i = 0; i < numUnstableDragPoints; i++)
        {
            Debug.Log("check DAT");
            
            TriggerNewUnstableDragPoint();
        }
    }
    
#region Arcade Gameplay

    public void TriggerNewUnstableDragPoint()
    {
        Debug.Log("New unstable");
        
        List<DragPoint> stableDragPoints = new List<DragPoint>();
        foreach (DragPoint dragPoint in dragPointList)
        {
            if(dragPoint.IsUnstable || (!gameStarted && (dragPoint == starterGateLeft || dragPoint == starterGateRight)))
                continue;

            stableDragPoints.Add(dragPoint);
        }
        
        if (stableDragPoints.Count == 0)
        {
            Debug.LogWarning("No stable drag points to make unstable");
            return;
        }
        
        int randomIndex = Random.Range(0, stableDragPoints.Count);
        stableDragPoints[randomIndex].TriggerUnstability();
    }

    public void RemoveDragPoint(DragPoint dragPoint)
    {
        dragPoint.SelfDestruct();
        
        dragPointList.Remove(dragPoint);

        if (InputManager.Instance.IsDraggingPoint && InputManager.Instance.lastDragPoint == dragPoint)
        {
            // Debug.Log("Dragging dis but no longer");
            
            InputManager.Instance.CancelDrag(dragPoint);
        }
        
        LineManager.Instance.BreakAllLinesConnectedToDragPoint(dragPoint);
    }

    public void SpawnNewDragPoints()
    {
        int randomNumDragPoints = Random.Range(dragPointCountRange.x, dragPointCountRange.y);

        for (int i = dragPointList.Count; i < randomNumDragPoints; i++)
        {
            Vector3 newPosition = Random.insideUnitCircle * dragPointSpawnRadius;
            
            int safetyCounter = 0;
            int safetyCounterCount = 100;
            bool tooClose = true;

            while (tooClose && safetyCounter < safetyCounterCount)
            {
                safetyCounter++;
                tooClose = false;

                foreach (DragPoint dragPoint in dragPointList)
                {
                    if (Vector3.Distance(dragPoint.transform.position, newPosition) < minDistanceBetweenDragPoints)
                    {
                        newPosition = Random.insideUnitCircle * dragPointSpawnRadius;
                        tooClose = true;
                        break;
                    }
                }

                if (safetyCounter >= safetyCounterCount)
                {
                    Debug.LogWarning("Safety counter reached 100, breaking loop and not spawning new drag point");
                    return;
                }
            }
            
            DragPoint newDragPoint = Instantiate(dragPointPrefab, dragPointParent);
            newDragPoint.transform.position = newPosition;
            dragPointList.Add(newDragPoint);
        }
    }

    public void AddTime(float time)
    {
        timer += time;
        
        timerText.text = timer.ToString("F1");
    }

#endregion

    public void HideGame()
    {
        StartCoroutine(PlaySoundAfterDelay(1.5f));
        
        screenFillSpawner.ShowAllScreenFillers();
    }
    
    private IEnumerator PlaySoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        AudioManager.Instance.PlayEvent("event:/end_transition");
    }

    public bool CanInteract()
    {
        return gameShown && !gameOver;
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
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ClearNonStartingDragPoints();
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnNewDragPoints();
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriggerNewUnstableDragPoint();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CreateAllLines();
        }

        if (!gameStarted || gameOver)
            return;
        
        timer -= Time.deltaTime;
        timerText.text = timer.ToString("F1");
        
        if(timer <= 0)
        {
            timer = 0;
            GameOver();
            
            AudioManager.Instance.PlayEvent("event:/timer_final");

            return;
        }
        
        if(timer <= timeRemainingWhenBeepsStart)
        {
            timeUntilBeepTimer -= Time.deltaTime;
            if(timeUntilBeepTimer <= 0)
            {
                feedback_lowTime.PlayFeedbacks();
                
                timeUntilBeepTimer = timeBetweenBeeps;
                
                AudioManager.Instance.PlayEvent("event:/timer_beep");
            }
        }
    }
    
    public void AddScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
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
        
        AudioManager.Instance.PlayEvent("event:/start_button_press");
        
        endUI.blocksRaycasts = false;
        
        feedback_fadeOutEndUI.PlayFeedbacks(); //Actual restart is called by the end of this feedback
    }

    private void CreateAllLines()
    {
        foreach (DragPoint dragPoint in dragPointList)
        {
            foreach (DragPoint otherDragPoint in dragPointList)
            {
                if(dragPoint != otherDragPoint)
                    LineManager.Instance.CreateLineBetweenDragPoints(dragPoint, otherDragPoint);
            }
        }
    }
}
