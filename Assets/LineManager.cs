using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
   public static LineManager Instance;

   public float lineWidth = 0.1f;
   public float edgeColliderRadius = 0.1f;

   public Material lineMaterial;
   
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
      lineRenderer.endWidth = lineWidth;
      lineRenderer.material = lineMaterial;
      
      //add collider to lineRenderer
      EdgeCollider2D edgeCollider = lineRenderer.gameObject.AddComponent<EdgeCollider2D>();
      edgeCollider.points = new Vector2[] { origin.transform.position, destination.transform.position };
      edgeCollider.edgeRadius = edgeColliderRadius;
   }
}
