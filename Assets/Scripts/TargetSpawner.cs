using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TargetSpawner : MonoBehaviour
{
    public Transform targetParent;
    public Target targetPrefab;

    public int numTargets = 100;
    public float spawnRadius = 4.5f;

    public bool uteslutaBottomTarget = true;
    public bool inverseRotation = false;
        
    public Vector2 scaleMinMaxOfTargets = new Vector2(1f, 1f);
    
    // public bool spawnNewTargets = false;

    private void Start()
    {
        SpawnTargets();
    }
    
    private void Update()
    {
        // if (spawnNewTargets)
        // {
        //     SpawnTargets();
        //     spawnNewTargets = false;
        // }
    }
    
    [Sirenix.OdinInspector.Button]
    public void SpawnTargets()
    {
        DestroyTargets();
        
        Vector2 centerPoint = Vector2.zero;
        
        for (int i = 0; i < numTargets; i++)
        {
            var radians = 2 * Mathf.PI / numTargets * i;
            var vertical = -Mathf.Cos(radians);
            var horizontal = Mathf.Sin(radians);
            var spawnDir = new Vector2(horizontal, vertical);
            var spawnPos = centerPoint + spawnDir * spawnRadius;
            
            Target target = Instantiate(targetPrefab, spawnPos, Quaternion.identity, targetParent);
            var dir = centerPoint - spawnPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            target.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
            if (inverseRotation)
            {
                target.transform.Rotate(Vector3.forward, 180f);
            }
            
            target.transform.localScale = Vector3.one * Random.Range(scaleMinMaxOfTargets.x, scaleMinMaxOfTargets.y);
        }
        
        if(uteslutaBottomTarget) //Destroys the target at the bottom of the circle (so the laser enters unobstructed)
            DestroyImmediate(targetParent.GetChild(0).gameObject);
    }
    
    [Sirenix.OdinInspector.Button, Tooltip("If main scene too large, delete all targets from each targetSpawner in the scene.")]
    public void DestroyTargets()
    {
        for (int i = targetParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(targetParent.GetChild(i).gameObject);
        }
    }
}
