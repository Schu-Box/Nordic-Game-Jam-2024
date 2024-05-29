using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : SerializedMonoBehaviour
{
    public static GameController Instance;

    [Header("Start UI")]
    public GameObject blackBackground;

    public CanvasGroup startUI;
    public MMF_Player feedback_fadeInStartUI;
    public MMF_Player feedback_fadeOutStartUI;

    [Header("Map Select UI")]
    public CanvasGroup mapSelectUI;

    public MMF_Player feedback_fadeInMapSelectUI;
    public MMF_Player feedback_fadeOutMapSelectUI;

    public List<ModeSelectButton> modeSelectButtonList;
    public List<MapSelectButton> mapSelectButtonList;

    [Header("Gameplay UI")]
    public Leaderboard gameplayLeaderboard;

    public TextMeshProUGUI playerNameText;

    public TextMeshProUGUI modeText;
    public TextMeshProUGUI mapText;
    
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public MMF_Player feedback_lowTime;
    
    public MMProgressBar chargeBar;

    [Header("Gameplay")]
    [SerializeField] public Dictionary<MapType, GameObject> mapDictionary = new Dictionary<MapType, GameObject>();

    public ScreenFillSpawner screenFillSpawner;

    public LaserSpawner laserSpawner;
    public DragPoint starterGateLeft;
    public DragPoint starterGateRight;
    public MMF_Player feedback_breakStartingGate;

    public DragPoint startingExcitedDragPoint;
    public MMF_Player feedback_excitedDragPointDestroyed;

    public Transform dragPointParent;
    public DragPoint dragPointPrefab;
    public List<DragPoint> dragPointList = new List<DragPoint>();

    public float dragPointSpawnRadius;
    public Vector2Int dragPointCountRange;
    public float minDistanceBetweenDragPoints = 1f;

    public List<Target> targetList = new List<Target>();

    [Header("End UI")]
    public CanvasGroup endUI;

    public MMF_Player feedback_fadeInEndUI;
    public MMF_Player feedback_fadeOutEndUI;

    public Leaderboard endLeaderboard;

    [Header("Variables")]
    public float timedModeTimeLimit = 45f;
    
    public float endlessModeStartingTimeLimit = 45f;

    public int numUnstableDragPoints = 2;

    public float timeRemainingWhenBeepsStart = 10f;
    public float timeBetweenBeeps = 1f;
    private float timeUntilBeepTimer = 0f;

    [Header("Should be private")]
    private float maxTimeLimit;
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
    public MapType currentMap = MapType.Circle;
    public ModeType currentMode = ModeType.Timed;

    private void Awake()
    {
        laserSpawner.SetActive(false);

        blackBackground.SetActive(true);

        Instance = this;

        endUI.alpha = 0f;
        endUI.interactable = false;
        endUI.blocksRaycasts = false;

        timer = endlessModeStartingTimeLimit;
        timerText.text = timer.ToString("F1");
        // chargeBar.BarProgress = 1f;

        mapSelectUI.alpha = 0f;
        mapSelectUI.interactable = false;
        mapSelectUI.blocksRaycasts = false;

        savedName = PlayerPrefs.GetString("savedName");

        if (savedName != "")
        {
            // Debug.Log("Coming in from save");

            currentName = savedName;
            currentMap = (MapType)Enum.Parse(typeof(MapType), PlayerPrefs.GetString("savedMap"));
            currentMode = (ModeType)Enum.Parse(typeof(ModeType), PlayerPrefs.GetString("savedMode"));

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

        scoreText.text = score.ToString();
    }

    private void Start()
    {
        //For Debug
        // SelectMode(ModeType.Timed);
        // SkipToGame();
    }

    private void SkipToGame()
    { 
        feedback_fadeOutStartUI.PlayFeedbacks();
        startUI.interactable = false;
        startUI.blocksRaycasts = false;
        
        ShowGame();
    }

    public void ShowGameFromMainMenu()
    {
        InputNewName();
        // ShowGame();
        ShowMapSelect();
        
        AudioManager.Instance.PlayEvent("event:/start_button_press");
    }

    private void InputNewName()
    {
        currentName = nameInputField.text;
    }
    
    public void ShowMapSelect()
    {
        feedback_fadeInMapSelectUI.PlayFeedbacks();
        mapSelectUI.interactable = true;
        mapSelectUI.blocksRaycasts = true;
        
        feedback_fadeOutStartUI.PlayFeedbacks();
        startUI.interactable = false;
        startUI.blocksRaycasts = false;

        foreach(MapSelectButton mapSelectButton in mapSelectButtonList)
        {
            mapSelectButton.DeselectMap();
        }
        
        modeSelectButtonList[0].SelectMode();
        mapSelectButtonList[0].SelectMap();
    }

    public void SelectMode(ModeType modeType)
    {
        currentMode = modeType;
        
        switch (modeType)
        {
            case ModeType.Timed:
                timer = timedModeTimeLimit;
                break;
            case ModeType.Endless:
                timer = endlessModeStartingTimeLimit;
                break;
        }

        maxTimeLimit = timer;
    }

    private MapSelectButton lastSelectedMapButton = null;
    public void SelectMap(MapSelectButton mapSelectButton)
    {
        Debug.Log("Selected map: " + mapSelectButton.mapType.ToString());
        
        currentMap = mapSelectButton.mapType;

        if (lastSelectedMapButton != null && lastSelectedMapButton != mapSelectButton)
        {
            lastSelectedMapButton.DeselectMap();
        }
        lastSelectedMapButton = mapSelectButton;
    }

    public void ConfirmMapAndMode()
    {
        feedback_fadeOutMapSelectUI.PlayFeedbacks();
        mapSelectUI.interactable = false;
        mapSelectUI.blocksRaycasts = false;

        ShowGame();
    }

    public void ShowGame()
    {
        foreach(GameObject map in mapDictionary.Values)
        {
            map.SetActive(false);
        }
        mapDictionary[currentMap].SetActive(true);
        
        gameShown = true;
        playerNameText.text = currentName;

        mapText.text = currentMap.ToString();
        modeText.text = currentMode.ToString();
        
        blackBackground.SetActive(false);
        
        // startUI.interactable = false;
        // startUI.blocksRaycasts = false;
        // feedback_fadeOutStartUI.PlayFeedbacks();

        screenFillSpawner.Spawn();
        
        screenFillSpawner.HideAllScreenFillers();
        
        AudioManager.Instance.PlayEvent("event:/start_transition");
        
        LineManager.Instance.CreateLineBetweenDragPoints(starterGateLeft, starterGateRight, true);
        
        laserSpawner.SetActive(true);

        gameplayLeaderboard.SetMapAndMode(currentMap, currentMode);
        endLeaderboard.SetMapAndMode(currentMap, currentMode);

        ClearNonStartingDragPoints();
        SpawnNewDragPoints();
        
        TriggerNewUnstableDragPoint(startingExcitedDragPoint);
        for(int i = 1; i < numUnstableDragPoints; i++)
        {
            TriggerNewUnstableDragPoint();
        }
    }
    
    private void ClearNonStartingDragPoints()
    {
        // Debug.Log("Generating new drag points");
        
        //destroy all dragPoints
        for(int i = dragPointList.Count - 1; i >= 0; i--)
        {
            DragPoint dragPoint = dragPointList[i];

            if (dragPoint == starterGateLeft || dragPoint == starterGateRight || dragPoint == startingExcitedDragPoint)
                continue;
            
            RemoveDragPoint(dragPoint);
        }
    }
    
#region Arcade Gameplay

    public void TriggerNewUnstableDragPoint(DragPoint forcedDragPoint = null)
    {
        // Debug.Log("New unstable");
        
        if(forcedDragPoint != null)
        {
            forcedDragPoint.TriggerUnstability();
            return;
        }
        
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

                foreach (Target target in targetList)
                {
                    if (Vector3.Distance(target.transform.position, newPosition) < minDistanceBetweenDragPoints)
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
        
        if (timer > maxTimeLimit) //Time can't go above max time
        {
            timer = maxTimeLimit;
        }

        UpdateTimerVisuals();
    }

    private void UpdateTimerVisuals()
    {
        timerText.text = timer.ToString("F1");
        
        chargeBar.UpdateBar(timer / maxTimeLimit, 0f, maxTimeLimit);
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
        UpdateTimerVisuals();
        
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

    public void CheckIfAllTargetsHit()
    {
        foreach(Target target in targetList)
        {
            if (!target.destroyed)
                return;
        }
        
        GameOver();
    }

    public void GameOver()
    {
        // GameOverUI.SetActive(true);
        gameOver = true;

        
        
        endLeaderboard.FinalizeScore();
        
        //TODO: If timed mode, show time as score

        laserSpawner.SetActive(false);
        
        HideGame();
        
        feedback_fadeInEndUI.PlayFeedbacks();
        
        endUI.interactable = true;
        endUI.blocksRaycasts = true;
    }

    public void Retry()
    {
        PlayerPrefs.SetString("savedName", currentName);
        PlayerPrefs.SetString("savedMap", currentMap.ToString());
        PlayerPrefs.SetString("savedMode", currentMode.ToString());
        
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
