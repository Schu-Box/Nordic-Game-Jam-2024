using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPoint : MonoBehaviour
{
    public void OnMouseDown()
    {
        if (GameController.Instance.gameOver)
            return;
        
        Debug.Log("Start drag at " + transform.position);
        LineManager.Instance.IsDraggingPoint = true;
    }

    public void OnMouseDrag()
    {
        if (GameController.Instance.gameOver)
            return;
        
        // Debug.Log("Dragging");

        InputManager.Instance.ShowLineCreation(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    
    public void OnMouseUp()
    {
        if (GameController.Instance.gameOver)
            return;
        
        LineManager.Instance.IsDraggingPoint = false;
        InputManager.Instance.lineCreationArrow.Show(false);
        
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("End drag at " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (hit.collider != null)
        {
            DragPoint dragPoint = hit.collider.GetComponent<DragPoint>();
            if (dragPoint != null)
            {
                LineManager.Instance.CreateLineBetweenDragPoints(this, dragPoint);
            }
        }
    }
}
