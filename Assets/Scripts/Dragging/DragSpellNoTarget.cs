using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragSpellNoTarget: DraggingActions
{
    private int savedHandSlot;
    private WhereIsTheCardOrCreature whereIsCard;
    private OneCardManager manager;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    public override bool CanDrag
    {
        get
        { 
            // TEST LINE: this is just to test playing creatures before the game is complete 
            // return true;

            // TODO : include full field check
            return base.CanDrag && manager.CanBePlayedNow;
        }
    }

    void Awake()
    {
        whereIsCard = GetComponent<WhereIsTheCardOrCreature>();
        manager = GetComponent<OneCardManager>();
    }

    public override void OnStartDrag()
    {
        savedHandSlot = whereIsCard.Slot;

        whereIsCard.VisualState = VisualStates.Dragging;
        whereIsCard.BringToFront();
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    public override void OnDraggingInUpdate(Transform draggableObject)
    {
        
    }

    public override void OnEndDrag()
    {
        // 1) Check if we are holding a card over the table
        if (DragSuccessful())
        {
            // play this card
            var idHolder = GetComponent<IDHolder>();
            if (idHolder == null)
                idHolder = GetComponentInParent<IDHolder>();

            playerOwner.PlaySpellFromHand(idHolder.UniqueID, -1);
        }
        else
        {
            // Set old sorting order 
            whereIsCard.Slot = savedHandSlot;
            if (tag.Contains("Low"))
                whereIsCard.VisualState = VisualStates.LowHand;
            else
                whereIsCard.VisualState = VisualStates.TopHand;
            // Move this card back to its slot position
            HandVisual PlayerHand = playerOwner.PlayerArea.handVisual;

            whereIsCard.BringToHandCards();
            transform.DOLocalMove(initialPosition, 1f);
            transform.DOLocalRotateQuaternion(initialRotation, 1f);
        } 
    }

    protected override bool DragSuccessful()
    {
        return TableVisual.CursorOverSomeTable;;
    }


}
