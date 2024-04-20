using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
   public static LineManager Instance;

   public float lineWidth = 0.1f;
   public float edgeColliderRadius = 0.1f;
   
   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
      }
      else
      {
         Destroy(gameObject);
      }
   }
   
   public void CreateLineBetweenDragPoints(DragPoint origin, DragPoint destination)
   {
      Debug.Log("Creating line between " + origin.name + " and " + destination.name);

      LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
      lineRenderer.positionCount = 2;
      lineRenderer.SetPosition(0, origin.transform.position);
      lineRenderer.SetPosition(1, destination.transform.position);
      lineRenderer.startWidth = lineWidth;
      
      //add collider to lineRenderer
      EdgeCollider2D edgeCollider = lineRenderer.gameObject.AddComponent<EdgeCollider2D>();
      edgeCollider.points = new Vector2[] { origin.transform.position, destination.transform.position };
      edgeCollider.edgeRadius = edgeColliderRadius;
      
      //add collider to lineRenderer
      // BoxCollider2D boxCollider = lineRenderer.gameObject.AddComponent<BoxCollider2D>();
      // boxCollider.size = new Vector2(Vector2.Distance(origin.transform.position, destination.transform.position), lineWidth);
      // boxCollider.offset = (origin.transform.position + destination.transform.position) / 2;
      // boxCollider.transform.up = origin.transform.position - destination.transform.position;
   }
}
