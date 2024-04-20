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

    [Header("Target")]
    public Transform targetParent;
    public Target targetPrefab;

    private float timer;
    private int score;

    public bool gameOver = false;
    
    private void Awake()
    {
        Instance = this;
        
        GameOverUI.SetActive(false);
        
        timer = timeLimit;

        // SpawnTargets();
    }

    private void SpawnTargets()
    {
        Vector2 centerPoint = Vector2.zero;
        int numTargets = 100;
        float radius = 4.5f;

        for (int i = 0; i < numTargets; i++)
        {
            var radians = 2 * Mathf.PI / numTargets * i;
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);
            var spawnDir = new Vector2(horizontal, vertical);
            var spawnPos = centerPoint + spawnDir * radius;

            var target = Instantiate(targetPrefab, spawnPos, Quaternion.identity, targetParent);
            var dir = centerPoint - spawnPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            target.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
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
        gameOver = true;
    }
}
