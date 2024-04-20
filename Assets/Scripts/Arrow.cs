using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
   public Transform tip;
   public LineRenderer lineRenderer;
   
   public void SetArrow(Vector3 origin, Vector3 destination)
   {
      lineRenderer.positionCount = 2;
      lineRenderer.SetPosition(0, origin);
      lineRenderer.SetPosition(1, destination);

      if (tip != null)
      {
         Vector3 tipPosition = destination;
         tipPosition.z = 0f;
         tip.position = tipPosition;
         
         //rotate the tip so it faces away from the origin and towards the destination
         Vector3 direction = destination - origin;
         float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
         tip.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
      }
   }
   
   public void Show(bool show)
   {
      lineRenderer.enabled = show;
      if (tip != null)
      {
         tip.gameObject.SetActive(show);
      }
   }
}
