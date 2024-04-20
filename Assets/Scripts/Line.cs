using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private EdgeCollider2D edgeCollider;
    
    public DragPoint origin;
    public DragPoint destination;

    public bool isStartingGate = false;

    public void Generate(DragPoint origin, DragPoint destination, bool startingGate = false)
    {
        this.origin = origin;
        this.destination = destination;
        
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, origin.transform.position);
        lineRenderer.SetPosition(1, destination.transform.position);
        lineRenderer.startWidth = LineManager.Instance.lineWidth;
        lineRenderer.endWidth = LineManager.Instance.lineWidth;
        lineRenderer.material = LineManager.Instance.lineMaterial;
        
        lineRenderer.gameObject.layer = LayerMask.NameToLayer("Mirrors");
        
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = new Vector2[] { origin.transform.position, destination.transform.position };
        edgeCollider.edgeRadius = LineManager.Instance.edgeColliderRadius;

        isStartingGate = startingGate;

    }

    private void Update()
    {
        lineRenderer.SetPosition(0, origin.transform.position);
        lineRenderer.SetPosition(1, destination.transform.position);
        
        edgeCollider.points = new Vector2[] { origin.transform.position, destination.transform.position };
    }
}