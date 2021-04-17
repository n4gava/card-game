using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragCreatureOnTable : DraggingActions 
{
    private VisualStates _initialVisualState;
    private Vector3 _initialPosition;
    private Vector3 _initialLocalPosition;
    private Quaternion _initialRotation;

    public WhereIsTheCardOrCreature whereIsCard;
    public OneCardManager cardManager;

    public override bool CanDrag => base.CanDrag && cardManager.CanBePlayedNow;

    public override void OnStartDrag()
    {
        _initialVisualState = whereIsCard.VisualState;
        _initialLocalPosition = this.transform.localPosition;
        _initialPosition = this.transform.position;
        _initialRotation = this.transform.rotation;

        this.transform.rotation = Quaternion.identity;
        whereIsCard.VisualState = VisualStates.Dragging;
        whereIsCard.BringToFront();
    }

    public override void OnEndDrag()
    {
        if (DragSuccessful())
        {
            int tablePos = playerOwner.PlayerArea.tableVisual.TablePosForNewCreature(Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - Camera.main.transform.position.z)).x);

            var idHolder = this.GetObjectUniqueID();
            playerOwner.PlayACreatureFromHand(idHolder, tablePos);
        }
        else
        {
            whereIsCard.BringToHandCards();
            whereIsCard.VisualState = _initialVisualState;
            var sequence = DOTween.Sequence();
            sequence.Insert(0f, this.transform.DOMove(_initialPosition, 1f));
            sequence.Insert(0f, this.transform.DORotateQuaternion(_initialRotation, 1f));
            sequence.Play();
        } 
    }

    protected override bool DragSuccessful()
    {
        bool tableIsFull = playerOwner.table.CreaturesOnTable.Count >= GlobalSettings.Instance.MaxCreaturesOnTable;
        return TableVisual.CursorOverSomeTable && !tableIsFull;
    }

    public override void OnDraggingInUpdate(Transform draggableObject) { }
}
