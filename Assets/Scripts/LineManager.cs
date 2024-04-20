using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
   public static LineManager Instance;

   public float lineWidth = 0.1f;
   public float edgeColliderRadius = 0.1f;

   public Material lineMaterial;

   public DragPoint lastDragPoint = null;
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
      if(origin == destination)
      {
         return false;
      }
      
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
      newLine.Generate(origin, destination);

      lineList.Add(newLine);
   }

   public void DestroyLine(Line line)
   {
      if (line.isStartingGate)
      {
         GameController.Instance.StartGame();
      }
      
      lineList.Remove(line);
      
      Destroy(line.gameObject);
   }
}
