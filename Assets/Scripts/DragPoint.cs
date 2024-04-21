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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameController.Instance.gameOver)
            return;

        Debug.Log("Hovering");
        
        hoverFeedback.PlayFeedbacks();
        
        
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameController.Instance.gameOver)
            return;

        Debug.Log("Unhovering");
        
        unhoverFeedback.PlayFeedbacks();
    }
    
    public void OnMouseDown()
    {
        if (GameController.Instance.gameOver)
            return;
        
        Debug.Log("Start drag at " + transform.position);
        LineManager.Instance.IsDraggingPoint = true;
        LineManager.Instance.lastDragPoint = this;
        
        selectFeedback.PlayFeedbacks();
    }

    public void OnMouseDrag()
    {
        if (GameController.Instance.gameOver)
            return;
        
        Vector2 destinationPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DragPoint overlappingDragPoint = OverlappingDragPoint(destinationPosition);
        if (overlappingDragPoint != null)
        {
            destinationPosition = overlappingDragPoint.transform.position;
        }

        Debug.Log(transform.position + " to " + destinationPosition);
        InputManager.Instance.ShowLineCreation(transform.position, destinationPosition);
    }
    
    public void OnMouseUp()
    {
        LineManager.Instance.IsDraggingPoint = false;
        LineManager.Instance.lastDragPoint = null;
        InputManager.Instance.lineCreationArrow.Show(false);
        
        if (GameController.Instance.gameOver)
            return;
        
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("End drag at " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        DragPoint overlappingDragPoint = OverlappingDragPoint(worldPoint);
        if (OverlappingDragPoint(worldPoint) != null)
        {
            LineManager.Instance.CreateLineBetweenDragPoints(this, overlappingDragPoint);

            overlappingDragPoint.deselectFeedback.PlayFeedbacks();
        }
        else //missed, cancel line
        {
            if (LineManager.Instance.lastDragPoint != null)
            {
                LineManager.Instance.lastDragPoint.deselectFeedback.PlayFeedbacks();
            }
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
