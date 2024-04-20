using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
   public LineRenderer lineRenderer;
   
   public void SetArrow(Vector3 origin, Vector3 destination)
   {
      lineRenderer.positionCount = 2;
      lineRenderer.SetPosition(0, origin);
      lineRenderer.SetPosition(1, destination);
   }
   
   public void ShowArrow(bool show)
   {
      lineRenderer.enabled = show;
   }
}
