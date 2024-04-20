using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPoint : MonoBehaviour
{
    public MMF_Player selectFeedback;
    public MMF_Player deselectFeedback;
    
    public Vector2 durationRangeBeforeMovement = new Vector2(2f, 8f);
    public Vector2 durationRangeDuringMovement = new Vector2(1f, 4f);
    public Vector2 distanceRangeOfMovement = new Vector2(0f, 1f);

    private float timerBeforeMovement;
    private Coroutine movementCoroutine;

    private void Start()
    {
        // wiggle.Pl
    }

    // private void Start()
    // {
    //     timerBeforeMovement = Random.Range(durationRangeBeforeMovement.x, durationRangeBeforeMovement.y);
    // }
    //
    // private void Update()
    // {
    //     if (GameController.Instance.gameOver)
    //         return;
    //     
    //     timerBeforeMovement -= Time.deltaTime;
    //
    //     if (timerBeforeMovement <= 0f && movementCoroutine == null)
    //     {
    //         Vector2 randomDirection = Random.insideUnitCircle;
    //         float randomDistance = Random.Range(distanceRangeOfMovement.x, distanceRangeOfMovement.y);
    //         
    //         Vector3 newPosition = transform.position + new Vector3(randomDirection.x, randomDirection.y, 0f) * randomDistance;
    //         
    //         //TODO: Prevent it from picking a position outside the circle
    //
    //         movementCoroutine = StartCoroutine(Move(newPosition, Random.Range(durationRangeDuringMovement.x, durationRangeDuringMovement.y)));
    //     }
    // }
    //
    // private IEnumerator Move(Vector3 newPosition, float duration)
    // {
    //     //move the point to the new position over the duration
    //     float timeElapsed = 0f;
    //     Vector3 startingPosition = transform.parent.position;
    //     
    //     while (timeElapsed < duration && !GameController.Instance.gameOver)
    //     {
    //         transform.parent.position = Vector3.Lerp(startingPosition, newPosition, timeElapsed / duration);
    //         timeElapsed += Time.deltaTime;
    //         yield return null;
    //     }
    //
    //     timerBeforeMovement = Random.Range(durationRangeBeforeMovement.x, durationRangeBeforeMovement.y);
    //     
    //     movementCoroutine = null;
    // }
    
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
        
        // Debug.Log("Dragging");

        InputManager.Instance.ShowLineCreation(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    
    public void OnMouseUp()
    {
        if (LineManager.Instance.lastDragPoint != null)
        {
            LineManager.Instance.lastDragPoint.deselectFeedback.PlayFeedbacks();
        }
        
        LineManager.Instance.IsDraggingPoint = false;
        LineManager.Instance.lastDragPoint = null;
        InputManager.Instance.lineCreationArrow.Show(false);
        
        if (GameController.Instance.gameOver)
            return;
        
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log("End drag at " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        if (hit.collider != null)
        {
            DragPoint dragPoint = hit.collider.GetComponent<DragPoint>();
            if (dragPoint != null)
            {
                LineManager.Instance.CreateLineBetweenDragPoints(this, dragPoint);
                
                dragPoint.deselectFeedback.PlayFeedbacks();
            }
        }
    }
}
