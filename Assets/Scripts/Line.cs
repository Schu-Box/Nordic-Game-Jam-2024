using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    
    public DragPoint dragPointOrigin = null;
    public DragPoint dragPointDestination = null;

    public bool isActiveLine = false;
    public bool isStartingGate = false;
    
    public void GenerateLineRenderer(Vector2 origin, Vector2 destination)
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, destination);
        lineRenderer.startWidth = LineManager.Instance.lineWidth;
        lineRenderer.endWidth = LineManager.Instance.lineWidth;
        lineRenderer.material = LineManager.Instance.lineMaterial;
        
        lineRenderer.gameObject.layer = LayerMask.NameToLayer("Default");
    }
    
    public void GenerateMirror(DragPoint origin, DragPoint destination, bool startingGate = false)
    {
        GenerateLineRenderer(origin.transform.position, destination.transform.position);
        
        isStartingGate = startingGate;
        
        isActiveLine = true;
        
        dragPointOrigin = origin;
        dragPointDestination = destination;
        
        lineRenderer.gameObject.layer = LayerMask.NameToLayer("Mirrors");
        
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = new Vector2[] { dragPointOrigin.transform.position, dragPointDestination.transform.position };
        edgeCollider.edgeRadius = LineManager.Instance.edgeColliderRadius;
    }

    private void Update()
    {
        if (dragPointOrigin != null && dragPointDestination != null)
        {
            Vector2 origin = dragPointOrigin.transform.position;
            Vector2 destination = dragPointDestination.transform.position;
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, destination);
        
            edgeCollider.points = new Vector2[] { origin, destination };
        }
    }
}