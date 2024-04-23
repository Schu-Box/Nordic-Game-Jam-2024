using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MMF_Player selectFeedback;
    public MMF_Player deselectFeedback;
    public MMF_Player hoverFeedback;
    public MMF_Player unhoverFeedback;

    private float timerBeforeMovement;
    private Coroutine movementCoroutine;
    
    private FMOD.Studio.EventInstance fmodStudioEvent;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (GameController.Instance.gameOver)
        //     return;

        // Debug.Log("Hovering");
        
        hoverFeedback.PlayFeedbacks();

        if (LineManager.Instance.IsDraggingPoint)
        {
            fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/anchor_hover_with_line");
            fmodStudioEvent.start();
            fmodStudioEvent.release();
        }
        else
        {
            fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/hover_cursor");
            fmodStudioEvent.start();
            fmodStudioEvent.release();  
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        // if (GameController.Instance.gameOver)
        //     return;

        // Debug.Log("Unhovering");
        
        unhoverFeedback.PlayFeedbacks();
    }
    
    public void OnMouseDown()
    {
        if (!GameController.Instance.CanInteract())
            return;

        Debug.Log("Start drag at " + transform.position);
        
        LineManager.Instance.IsDraggingPoint = true;
        LineManager.Instance.lastDragPoint = this;
        
        selectFeedback.PlayFeedbacks();
        
        fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/anchor_select");
        fmodStudioEvent.start();
        fmodStudioEvent.release();
    }

    public void OnMouseDrag()
    {
        if (!GameController.Instance.CanInteract())
            return;
        
        Vector2 destinationPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DragPoint overlappingDragPoint = OverlappingDragPoint(destinationPosition);
        if (overlappingDragPoint != null)
        {
            destinationPosition = overlappingDragPoint.transform.position;
        }

        // Debug.Log(transform.position + " to " + destinationPosition);
        InputManager.Instance.ShowLineCreation(transform.position, destinationPosition);
    }
    
    public void OnMouseUp()
    {
        LineManager.Instance.IsDraggingPoint = false;
        LineManager.Instance.lastDragPoint = null;
        InputManager.Instance.lineCreationArrow.Show(false);

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Debug.Log("End drag at " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        DragPoint overlappingDragPoint = OverlappingDragPoint(worldPoint);
        if (OverlappingDragPoint(worldPoint) != null && GameController.Instance.CanInteract())
        {
            LineManager.Instance.CreateLineBetweenDragPoints(this, overlappingDragPoint);

            overlappingDragPoint.deselectFeedback.PlayFeedbacks();
            
            fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/anchor_snap");
            fmodStudioEvent.start();
            fmodStudioEvent.release();
        }
        else if (GameController.Instance.GameShown) //missed, cancel line
        {
            // Debug.Log("CANCEL!");

            fmodStudioEvent = FMODUnity.RuntimeManager.CreateInstance("event:/anchor_drop");
            fmodStudioEvent.start();
            fmodStudioEvent.release();
            
            deselectFeedback.PlayFeedbacks();
            
            // if (LineManager.Instance.lastDragPoint != null)
            // {
            //     LineManager.Instance.lastDragPoint.deselectFeedback.PlayFeedbacks();
            // }
        }
    }

    private DragPoint OverlappingDragPoint(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.zero);
        if (hit.collider != null)
        {
            DragPoint dragPoint = hit.collider.GetComponent<DragPoint>();
            if (dragPoint != null)
            {
                return dragPoint;
            }
        }

        return null;
    }
}
