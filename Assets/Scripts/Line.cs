using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    private LineRenderer lineRenderer;
    
    public DragPoint origin;
    public DragPoint destination;
    
    public void Generate(DragPoint origin, DragPoint destination)
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

    }

    private void Update()
    {
        lineRenderer.SetPosition(0, origin.transform.position);
        lineRenderer.SetPosition(1, destination.transform.position);
    }
}