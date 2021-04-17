using System;
using System.Linq;
using UnityEngine;

public abstract class DraggingTargetActions : DraggingActions
{
    public GameObject targetGameObject;
    public GameObject triangleGameObject;

    private SpriteRenderer targetSpriteRenderer;
    private LineRenderer lineRenderer;
    private SpriteRenderer triangleSpriteRenderer;
    private float distanceForShowArrow = 2.3f;
    private float triangleRenderingGap = 1.5f;
    private float lineRenderingGap = 2.3f;


    void Awake()
    {
        targetSpriteRenderer = targetGameObject.GetComponent<SpriteRenderer>();
        lineRenderer = targetGameObject.GetComponentInChildren<LineRenderer>();
        triangleSpriteRenderer = triangleGameObject.GetComponent<SpriteRenderer>();
    }

    public override void OnStartDrag()
    {
        targetSpriteRenderer.enabled = true;
        lineRenderer.enabled = true;
    }

    public override void OnDraggingInUpdate(Transform draggableObject)
    {
        var notNormalized = draggableObject.position - draggableObject.parent.position;
        var direction = notNormalized.normalized;
        float distanceToTarget = (direction * distanceForShowArrow).magnitude;

        if (notNormalized.magnitude > distanceToTarget)
        {
            lineRenderer.SetPositions(new Vector3[] { draggableObject.parent.position, draggableObject.position - direction * lineRenderingGap });
            lineRenderer.enabled = true;

            triangleSpriteRenderer.enabled = true;
            triangleSpriteRenderer.transform.position = draggableObject.position - triangleRenderingGap * direction;

            var zRotation = Mathf.Atan2(notNormalized.y, notNormalized.x) * Mathf.Rad2Deg;
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
        targetGameObject.transform.localPosition = Vector3.zero;
        targetSpriteRenderer.enabled = false;
        lineRenderer.enabled = false;
        triangleSpriteRenderer.enabled = false;
    }

    /// <summary>
    /// Retorna o objeto selecionado 
    /// </summary>
    /// <param name="expectedTags">Filtro de tag de objetos esperados</param>
    /// <returns>Objeto selecionado</returns>
    public GameObject GetSelectedGameObject(Func<string, bool> expectedTags)
    {
        var hits = Physics.RaycastAll(
            origin: Camera.main.transform.position,
            direction: (-Camera.main.transform.position + targetGameObject.transform.position).normalized,
            maxDistance: 30f);

        return hits.Where(hit => expectedTags(hit.transform.tag))
            .Select(hit => hit.transform.gameObject)
            .FirstOrDefault();
    }

    protected override bool DragSuccessful()
    {
        return true;
    }
}