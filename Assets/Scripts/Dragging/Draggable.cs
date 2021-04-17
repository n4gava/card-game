using UnityEngine;

public class Draggable : MonoBehaviour
{
    public static Draggable DraggingThis { get; private set; }

    private bool dragging = false;
    private float zDisplacement;
    private DraggingActions draggingAction;

    public Transform draggableObject;


    void Awake()
    {
        draggingAction = GetComponent<DraggingActions>();
    }

    void OnMouseDown()
    {
        if (draggingAction != null && draggingAction.CanDrag)
        {
            dragging = true;
            HoverPreview.PreviewsAllowed = false;
            DraggingThis = this;
            draggingAction.OnStartDrag();
            zDisplacement = -Camera.main.transform.position.z + draggableObject.position.z;
        }
    }

    void Update ()
    {
        if (dragging)
        { 
            Vector3 mousePos = MouseInWorldCoords();
            draggableObject.position = new Vector3(mousePos.x, mousePos.y, draggableObject.position.z);   
            draggingAction.OnDraggingInUpdate(draggableObject);
        }
    }
	
    void OnMouseUp()
    {
        if (dragging)
        {
            dragging = false;
            HoverPreview.PreviewsAllowed = true;
            DraggingThis = null;
            draggingAction.OnEndDrag();
        }
    }   

    private Vector3 MouseInWorldCoords()
    {
        var screenMousePos = Input.mousePosition;
        screenMousePos.z = zDisplacement;
        return Camera.main.ScreenToWorldPoint(screenMousePos);
    }
        
}
