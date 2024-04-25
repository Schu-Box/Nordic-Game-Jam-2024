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

    public Vector2 scaleMinMaxOfTargets = new Vector2(1f, 1f);

    public bool spawnNewTargets = false;

    private void Update()
    {
        if (spawnNewTargets)
        {
            SpawnTargets();
            spawnNewTargets = false;
        }
    }

    public void SpawnTargets()
    {
       for (int i = targetParent.childCount - 1; i >= 0; i--)
       {
           DestroyImmediate(targetParent.GetChild(i).gameObject);
       }
        
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
            
            target.transform.localScale = Vector3.one * Random.Range(scaleMinMaxOfTargets.x, scaleMinMaxOfTargets.y);
        }
        
        //Destroys the target at the bottom of the circle (so the laser enters unobstructed)
        DestroyImmediate(targetParent.GetChild(0).gameObject);
    }
}
