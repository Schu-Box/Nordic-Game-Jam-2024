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

    
    private void Awake()
    {
        Instance = this;
    }
    
    private void LateUpdate()
    {
        if (GameController.Instance.gameOver)
            return;
        
        if (Input.GetMouseButtonDown(0) && !LineManager.Instance.IsDraggingPoint)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            IsDraggingCancellation = true;
            startDragCancellationPosition = worldPoint;
        }

        if (Input.GetMouseButtonUp(0) && LineManager.Instance.IsDraggingPoint)
        {
            LineManager.Instance.IsDraggingPoint = false;
            lineCreationArrow.Show(false);
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
        Debug.Log("Attempting cancel");
        
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
                if (line != null)
                {
                    LineManager.Instance.DestroyLine(hit.collider.GetComponent<Line>());
                }
                else
                {
                    Debug.LogWarning(hit.collider.name + " should not be on this layer! Was hit by cancellation arrow.");
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
}
