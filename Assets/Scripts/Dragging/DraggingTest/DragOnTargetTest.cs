using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragOnTargetTest : DraggingActionsTest
{
    //public TargetingOptions Targets = TargetingOptions.AllCharacters;
    private SpriteRenderer spriteRenderer;
    private LineRenderer lineRenderer;
    private SpriteRenderer triangleSpriteRenderer;

    public string draggingSortingLayer = "AboveEverything";
    public Transform triangle;
    public float distanceForShowArrow = 2.3f;
    public float triangleRenderingGap = 1f;
    public float lineRenderingGap = 2.3f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        lineRenderer.sortingLayerName = draggingSortingLayer;
        triangleSpriteRenderer = triangle.GetComponent<SpriteRenderer>();
    }

    public override void OnStartDrag()
    {
        HoverPreview.PreviewsAllowed = false;
        spriteRenderer.enabled = true;
        lineRenderer.enabled = true;
    }

    public override void OnDraggingInUpdate(Vector3 mousePos)
    {
        Vector3 notNormalized = transform.position - transform.parent.position;
        Vector3 direction = notNormalized.normalized;
        float distanceToTarget = (direction* distanceForShowArrow).magnitude;


        if (notNormalized.magnitude > distanceToTarget)
        {
            // draw a line between the creature and the target
            lineRenderer.SetPositions(new Vector3[]{ transform.parent.position, transform.position - direction * lineRenderingGap });
            lineRenderer.enabled = true;

            // position the end of the arrow between near the target.
            triangleSpriteRenderer.enabled = true;
            triangleSpriteRenderer.transform.position = transform.position - triangleRenderingGap * direction;

            // proper rotarion of arrow end
            float zRotation = Mathf.Atan2(notNormalized.y, notNormalized.x) * Mathf.Rad2Deg;
            triangleSpriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, zRotation - 90);
        }
        else
        {
            lineRenderer.enabled = false;
            triangleSpriteRenderer.enabled = false;
        }

    }

    public override void OnEndDrag()
    {
        transform.localPosition = Vector3.zero;
        spriteRenderer.enabled = false;
        lineRenderer.enabled = false;
        triangleSpriteRenderer.enabled = false;
        HoverPreview.PreviewsAllowed = true;
    }

    protected override bool DragSuccessful()
    {
        return true;
    }
}
