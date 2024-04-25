using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public Arrow lineCreationArrow;
    public Arrow lineCancellationArrow;
    
    public LayerMask cancellationLayerMask;

    public bool IsDraggingCancellation = false;
    private Vector3 startDragCancellationPosition;
    
    public DragPoint lastDragPoint = null;
    public bool IsDraggingPoint = false;

    
    private void Awake()
    {
        Instance = this;
    }
    
    private void LateUpdate()
    {
        // if (Input.GetMouseButtonUp(0) && LineManager.Instance.IsDraggingPoint) //Cancel drag
        // {
        //     Debug.Log("Cancelling drag");
        //     
        //     LineManager.Instance.lastDragPoint.deselectFeedback.PlayFeedbacks();
        //     
        //     LineManager.Instance.IsDraggingPoint = false;
        //     LineManager.Instance.lastDragPoint = null;
        //     lineCreationArrow.Show(false);
        // }
        
        if (!GameController.Instance.CanInteract())
            return;
        
        if (Input.GetMouseButtonDown(0) && !IsDraggingPoint)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            IsDraggingCancellation = true;
            startDragCancellationPosition = worldPoint;
        }
    }

    private void Update()
    {
        if (IsDraggingCancellation)
        {
            ShowLineCancellation(startDragCancellationPosition, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            
            if (Input.GetMouseButtonUp(0))
            {
                AttemptCancellation();
                
                IsDraggingCancellation = false;
                lineCancellationArrow.Show(false);
            }
        }
    }

    private void AttemptCancellation()
    {
        // Debug.Log("Attempting cancel");
        
        Vector2 start = startDragCancellationPosition;
        Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        bool attemptingRaycast = true;
        while (attemptingRaycast)
        {
            RaycastHit2D hit = Physics2D.Raycast(start, end - start, Vector2.Distance(start, end), cancellationLayerMask);
            if (hit.collider != null)
            {
                // Debug.Log("Hit " + hit.collider.name);
                Line line = hit.collider.GetComponent<Line>();
                if (line != null && line.isActiveLine)
                {
                    LineManager.Instance.BreakLine(hit.collider.GetComponent<Line>(), hit.point);
                }
            }
            else
            {
                attemptingRaycast = false; //No more hits, stop attempting to destroy lines
            }
        }
    }

    public void ShowLineCreation(Vector3 start, Vector3 end)
    {
        lineCreationArrow.SetArrow(start, end);
        lineCreationArrow.Show(true);
    }
    
    public void ShowLineCancellation(Vector3 start, Vector3 end)
    {
        lineCancellationArrow.SetArrow(start, end);
        lineCancellationArrow.Show(true);
    }

    public void CancelDrag(DragPoint dragPoint)
    {
        // Debug.Log("cancel dat");
        
        IsDraggingPoint = false;
        lastDragPoint = null;
        lineCreationArrow.Show(false);

        AudioManager.Instance.PlayEvent("event:/anchor_drop");
            
        dragPoint.deselectFeedback.PlayFeedbacks();
    }
    
    public void CompleteDrag(DragPoint origin, DragPoint destination)
    {
        IsDraggingPoint = false;
        lastDragPoint = null;
        lineCreationArrow.Show(false);
        
        LineManager.Instance.CreateLineBetweenDragPoints(origin, destination);

        destination.deselectFeedback.PlayFeedbacks();
            
        AudioManager.Instance.PlayEvent("event:/anchor_snap");
    }
}
