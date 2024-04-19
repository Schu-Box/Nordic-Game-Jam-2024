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

    private RaycastHit2D hit;
    private Ray2D ray;
    // private Vector3 direction;

    public void Update()
    {
        // ShootLaser();
    }

    // public void ShootLaser()
    // {
    //     ray = new Ray(transform.position, transform.forward);
    //
    //     laserLineRenderer.positionCount = 1;
    //     laserLineRenderer.SetPosition(0, laserFirePoint.position);
    //
    //     float remainingLength = maxLaserLength;
    //
    //     for (int i = 0; i < maxNumReflections; i++)
    //     {
    //         hit = Physics2D.Raycast(ray.origin, ray.direction, remainingLength, layerMask);
    //         if (hit)
    //         {
    //             
    //         }
    //         else
    //         {
    //             laserLineRenderer.positionCount += 1;
    //             laserLineRenderer.SetPosition(laserLineRenderer.positionCount - 1, ray.origin + (transform.forward * remainingLength));
    //         }
    //     }
    //     
    //     if (Physics2D.Raycast(transform.position, transform.up))
    //     {
    //         RaycastHit2D raycastHit2D = Physics2D.Raycast(laserFirePoint.position, transform.up);
    //         Draw2DRay(laserFirePoint.position, raycastHit2D.point);
    //     }
    //     else
    //     {
    //         Draw2DRay(transform.position, laserFirePoint.position + transform.up * maxLaserDistance);
    //     }
    // }

    public void Draw2DRay(Vector3 startPosition, Vector3 endPosition)
    {
        laserLineRenderer.SetPosition(0, startPosition);
        laserLineRenderer.SetPosition(1, endPosition);        
    }
}
