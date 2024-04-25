using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Collider2D collie;
    
    [Header("Feedbacks")]
    public MMF_Player selectFeedback;
    public MMF_Player deselectFeedback;
    public MMF_Player hoverFeedback;
    public MMF_Player unhoverFeedback;
    
    public MMF_Player unstableFeedback;
    public MMF_Player destroyFeedback;

    public MMWiggle wiggle;

    private float timerBeforeMovement;
    private Coroutine movementCoroutine;
    
    private bool isUnstable = false;
    public bool IsUnstable => isUnstable;

    private void Start()
    {
        // wiggle.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (GameController.Instance.gameOver)
        //     return;

        // Debug.Log("Hovering");
        
        hoverFeedback.PlayFeedbacks();

        if (InputManager.Instance.IsDraggingPoint)
        {
            AudioManager.Instance.PlayEvent("event:/anchor_hover_with_line");
        }
        else
        {
            AudioManager.Instance.PlayEvent("event:/hover_cursor");
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

        // Debug.Log("Start drag at " + transform.position);
        
        InputManager.Instance.IsDraggingPoint = true;
        InputManager.Instance.lastDragPoint = this;
        
        selectFeedback.PlayFeedbacks();
        
        AudioManager.Instance.PlayEvent("event:/anchor_select");
    }

    public void OnMouseDrag()
    {
        if (!GameController.Instance.CanInteract())
            return;
        
        if(InputManager.Instance.lastDragPoint != this)
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
        if(InputManager.Instance.lastDragPoint != this)
            return;
        
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DragPoint overlappingDragPoint = OverlappingDragPoint(worldPoint);
        
        if (OverlappingDragPoint(worldPoint) != null && GameController.Instance.CanInteract())
        {
            InputManager.Instance.CompleteDrag(this, overlappingDragPoint);
        }
        else if (GameController.Instance.GameShown) //missed, cancel line
        {
            InputManager.Instance.CancelDrag(this);
        }
    }

    public DragPoint OverlappingDragPoint(Vector3 position)
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

    [Header("Unstability Variables")]
    public Vector2 unstabilityDurationRange = new Vector2(2f, 4f);
    public Vector2 unstabilityWiggleStartRange = new Vector2(0.5f, 0.8f);
    public Vector2 unstabilityWiggleEndRange = new Vector2(1.2f, 1.5f);

    public Vector2 unstabilityParticleEmissionRateRange = new Vector2(5f, 30f);

    // public LeanTweenType easeType;

    private ParticleSystem unstabilityParticles;
    public void TriggerUnstability()
    {
        isUnstable = true;
        
        wiggle.enabled = true;
        unstableFeedback.PlayFeedbacks();

        unstabilityParticles = unstableFeedback.transform.GetComponentInChildren<ParticleSystem>();

        StartCoroutine(UnstabilityCoroutine());
    }
    private IEnumerator UnstabilityCoroutine()
    {
        Vector3 startAmplitudeMin = new Vector3(unstabilityWiggleStartRange.x, unstabilityWiggleStartRange.x, 0f);
        Vector3 startAmplitudeMax = new Vector3(unstabilityWiggleStartRange.y, unstabilityWiggleStartRange.y, 0f);
        Vector3 endAmplitudeMin = new Vector3(unstabilityWiggleEndRange.x, unstabilityWiggleEndRange.x, 0f);
        Vector3 endAmplitudeMax = new Vector3(unstabilityWiggleEndRange.y, unstabilityWiggleEndRange.y, 0f);

        float duration = Random.Range(unstabilityDurationRange.x, unstabilityDurationRange.y);
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;

            float step = timer / duration;
            step = LeanTween.easeInSine(0f, 1f, step);

            // Debug.Log(step);
            
            wiggle.PositionWiggleProperties.AmplitudeMin = Vector3.Lerp(startAmplitudeMin, endAmplitudeMin, step);
            wiggle.PositionWiggleProperties.AmplitudeMax = Vector3.Lerp(startAmplitudeMax, endAmplitudeMax, step);
            
            ParticleSystem.EmissionModule emissionModule = unstabilityParticles.emission;
            emissionModule.rateOverTime = Mathf.Lerp(unstabilityParticleEmissionRateRange.x, unstabilityParticleEmissionRateRange.y, step);
            
            yield return null;
        }
        
        destroyFeedback.PlayFeedbacks();
        GameController.Instance.RemoveDragPoint(this);
    }
}
