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
            var vertical = Mathf.Sin(radians);
            var horizontal = Mathf.Cos(radians);
            var spawnDir = new Vector2(horizontal, vertical);
            var spawnPos = centerPoint + spawnDir * spawnRadius;

            var target = Instantiate(targetPrefab, spawnPos, Quaternion.identity, targetParent);
            var dir = centerPoint - spawnPos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            target.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
