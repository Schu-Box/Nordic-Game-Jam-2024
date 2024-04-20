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
        Debug.Log("Shooting laser");

        ResetMostRecentlyHitObject();

        Vector3 raycastOrigin = laserFirePoint.position;
        Vector3 raycastDirection = laserDirection;
        // ray = new Ray2D(transform.position, laserDirection);
        
        laserLineRenderer.positionCount = 1;
        laserLineRenderer.SetPosition(0, laserFirePoint.position);
        
        float remainingLength = maxLaserLength;
        
        for (int i = 0; i < maxNumReflections; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, raycastDirection, remainingLength, layerMask);
            if (hit)
            {
                // Debug.Log("HIT " + hit.collider.name);

                ResetMostRecentlyHitObject();
                
                int mostRecentHitLayer = LayerMask.NameToLayer("MostRecentHit");
                mostRecentHitObject = hit.collider.gameObject;
                mostRecentHitObject.layer = mostRecentHitLayer;
                
                laserLineRenderer.positionCount += 1;
                laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, hit.point);
        
                remainingLength -= Vector3.Distance(raycastOrigin, hit.point);
        
                Debug.Log("Last direction : " + raycastDirection +  " to New direction : " + Vector2.Reflect(raycastDirection, hit.normal) + " with hit normal of " + hit.normal);

                // raycastOrigin = hit.collider.ClosestPoint(raycastDirection);
                raycastOrigin = hit.point;
                raycastDirection = Vector2.Reflect(raycastDirection, hit.normal);
            }
            else
            {
                // Debug.Log("No hit");
                
                laserLineRenderer.positionCount += 1;
                laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, raycastOrigin + (raycastDirection * remainingLength));
                break;
            }
        }
    }

    private void ResetMostRecentlyHitObject()
    {
        if (mostRecentHitObject != null)
        {
            int defaultLayer = LayerMask.NameToLayer("Default");
            mostRecentHitObject.layer = defaultLayer;
        }
    }
    
    // public void ShootLaser3D()
    // {
    //     Debug.Log("Shooting laser");
    //
    //     ray = new Ray(transform.position, laserDirection);
    //     
    //     laserLineRenderer.positionCount = 1;
    //     laserLineRenderer.SetPosition(0, laserFirePoint.position);
    //     
    //     float remainingLength = maxLaserLength;
    //     
    //     for (int i = 0; i < maxNumReflections; i++)
    //     {
    //         if(Physics.Raycast(ray.origin, ray.direction, out hit, remainingLength, layerMask))
    //         {
    //             // Debug.Log("HIT " + hit.collider.name);
    //             
    //             laserLineRenderer.positionCount += 1;
    //             laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, hit.point);
    //     
    //             remainingLength -= Vector3.Distance(ray.origin, hit.point);
    //     
    //             Debug.Log("Last direction : " + ray.direction +  " to New direction : " + Vector3.Reflect(ray.direction, hit.normal) + " with hit normal of " + hit.normal);
    //             
    //             ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
    //         }
    //         else
    //         {
    //             // Debug.Log("No hit");
    //             
    //             laserLineRenderer.positionCount += 1;
    //             laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, ray.origin + (ray.direction * remainingLength));
    //             break;
    //         }
    //     }
    // }
}
