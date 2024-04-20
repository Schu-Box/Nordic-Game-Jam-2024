using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPoint : MonoBehaviour
{
    public void OnMouseDown()
    {
        Debug.Log("Start drag at " + transform.position);
        LineManager.Instance.IsDraggingPoint = true;
    }

    public void OnMouseDrag()
    {
        // Debug.Log("Dragging");

        InputManager.Instance.ShowLineCreation(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    
    public void OnMouseUp()
    {
        LineManager.Instance.IsDraggingPoint = false;
        
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
