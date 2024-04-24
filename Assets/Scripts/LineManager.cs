using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class LineManager : MonoBehaviour
{
   public static LineManager Instance;

   public float lineWidth = 0.1f;
   public float edgeColliderRadius = 0.1f;

   public Material lineMaterial;

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

      LeanTween.reset();
   }

   public bool LineCanBeCreated(DragPoint origin, DragPoint destination)
   {
      if(origin == destination)
      {
         return false;
      }
      
      foreach (Line line in lineList)
      {
         if(line.dragPointOrigin == origin && line.dragPointDestination == destination || line.dragPointOrigin == destination && line.dragPointDestination == origin)
         {
            return false;
         }
      }

      return true;
   }
   
   public void CreateLineBetweenDragPoints(DragPoint origin, DragPoint destination, bool isStartingGate = false)
   {
      if(!LineCanBeCreated(origin, destination))
      {
         // Debug.Log("Can't create line between " + origin.name + " and " + destination.name + " because it already exists.");
         return;
      }
      
      // Debug.Log("Creating line between " + origin.name + " and " + destination.name);

      
      Line newLine = new GameObject("Line").AddComponent<Line>();
      newLine.transform.parent = transform;
      newLine.GenerateMirror(origin, destination, isStartingGate);

      lineList.Add(newLine);
   }

   // private float durationBreak = 0.2f;
   
   public void BreakLine(Line line, Vector2 breakPoint)
   {
      Debug.Log("Breaking line : " + line.gameObject.name);
      
      if (line.isStartingGate)
      {
         GameController.Instance.StartGame();
      }
      
      lineList.Remove(line);
      
      CreateSnapLine(breakPoint, line.dragPointOrigin);
      CreateSnapLine(breakPoint, line.dragPointDestination);
      
      
      AudioManager.Instance.PlayEvent("event:/line_snip");
      
      Destroy(line.gameObject);
   }

   private void CreateSnapLine(Vector2 breakPoint, DragPoint snapToPoint)
   {
      Line newSnapLine = new GameObject("Line").AddComponent<Line>();
      newSnapLine.GenerateLineRenderer(snapToPoint.transform.position, breakPoint);
      newSnapLine.transform.parent = transform;

      // snapToPoint.deselectFeedback.PlayFeedbacks();

      LeanTween.value(newSnapLine.gameObject, breakPoint, (Vector2)snapToPoint.transform.position, 0.2f).setEaseOutCubic().setOnUpdate((Vector3 val) =>
      {
         newSnapLine.lineRenderer.SetPosition(1, val);
      }).setOnComplete(() =>
      {
         snapToPoint.deselectFeedback.PlayFeedbacks();
         Destroy(newSnapLine.gameObject);
      });
   }

   public void BreakAllLinesConnectedToDragPoint(DragPoint dragPoint)
   {
      for(int i = lineList.Count - 1; i >= 0; i--)
      {
         Line line = lineList[i];
         if (line.dragPointOrigin == dragPoint || line.dragPointDestination == dragPoint)
         {
            BreakLine(line, dragPoint.transform.position);
         }
      }
   }
}
