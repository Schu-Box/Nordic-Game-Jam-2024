using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
   public static LineManager Instance;

   public float lineWidth = 0.1f;
   public float edgeColliderRadius = 0.1f;

   public Material lineMaterial;

   public bool IsDraggingPoint = false;

   private List<Line> lineList = new List<Line>();
   
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

   public bool LineCanBeCreated(DragPoint origin, DragPoint destination)
   {
      foreach (Line line in lineList)
      {
         if(line.origin == origin && line.destination == destination || line.origin == destination && line.destination == origin)
         {
            return false;
         }
      }

      return true;
   }
   
   public void CreateLineBetweenDragPoints(DragPoint origin, DragPoint destination)
   {
      if(!LineCanBeCreated(origin, destination))
      {
         Debug.Log("Can't create line between " + origin.name + " and " + destination.name + " because it already exists.");
         return;
      }
      
      Debug.Log("Creating line between " + origin.name + " and " + destination.name);

      
      Line newLine = new GameObject("Line").AddComponent<Line>();
      newLine.origin = origin;
      newLine.destination = destination;
      
      LineRenderer lineRenderer = newLine.gameObject.AddComponent<LineRenderer>();
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

      lineRenderer.gameObject.layer = LayerMask.NameToLayer("Mirrors");

      lineList.Add(newLine);
   }

   public void DestroyLine(Line line)
   {
      lineList.Remove(line);
      
      Destroy(line.gameObject);
   }
}
