using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSpawner : MonoBehaviour
{
    public float maxLaserLength = 100f;
    public int maxNumReflections = 100;

    public LayerMask layerMask;

    public Transform laserFirePoint;
    public LineRenderer laserLineRenderer;
    public List<LineRenderer> laserLineRendererList;

    // private RaycastHit2D hit;
    // private Ray2D ray;

    private Vector2 laserDirection;

    private GameObject mostRecentHitObject = null;

    private void Start()
    {
        laserDirection = transform.up;
    }

    public void FixedUpdate()
    {
        ShootLaser();
    }

    public void ShootLaser()
    {
        // Debug.Log("Shooting laser");

        ResetMostRecentlyHitObject();

        Vector3 raycastOrigin = laserFirePoint.position;
        Vector3 raycastDirection = laserDirection;
        // ray = new Ray2D(transform.position, laserDirection);

        foreach (LineRenderer lineRenderer in laserLineRendererList)
        {
            lineRenderer.positionCount = 1;
            lineRenderer.SetPosition(0, laserFirePoint.position);
        }

        float remainingLength = maxLaserLength;

        for (int i = 0; i < maxNumReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, remainingLength, layerMask);
            if (hit)
            {
                Debug.Log("HIT " + hit.collider.name);

                Target target = hit.collider.gameObject.GetComponent<Target>();
                if (target != null)
                {
                    target.Hit();
                    break;
                }

                ResetMostRecentlyHitObject();
                
                int mostRecentHitLayer = LayerMask.NameToLayer("MostRecentMirror");
                mostRecentHitObject = hit.collider.gameObject;
                mostRecentHitObject.layer = mostRecentHitLayer;

                foreach (LineRenderer lineRenderer in laserLineRendererList)
                {
                    lineRenderer.positionCount += 1;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                }

                remainingLength -= Vector3.Distance(raycastOrigin, hit.point);

                // Debug.Log("Last direction : " + raycastDirection +  " to New direction : " + Vector2.Reflect(raycastDirection, hit.normal) + " with hit normal of " + hit.normal);

                // raycastOrigin = hit.collider.ClosestPoint(raycastDirection);
                raycastOrigin = hit.point;
                raycastDirection = Vector2.Reflect(raycastDirection, hit.normal);
            }
            else
            {
                // Debug.Log("No hit");

                foreach (LineRenderer lineRenderer in laserLineRendererList)
                {
                    lineRenderer.positionCount += 1;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, raycastOrigin + (raycastDirection * remainingLength));
                }

                break;
            }
        }
    }

    private void ResetMostRecentlyHitObject()
    {
        if (mostRecentHitObject != null)
        {
            int defaultLayer = LayerMask.NameToLayer("Mirrors");
            mostRecentHitObject.layer = defaultLayer;
        }
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }
}
