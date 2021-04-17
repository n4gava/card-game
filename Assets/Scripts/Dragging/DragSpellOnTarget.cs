using UnityEngine;

public class DragSpellOnTarget : DraggingTargetActions
{
    public TargetingOptions Targets = TargetingOptions.AllCharacters;
    public OneCardManager manager;
    public WhereIsTheCardOrCreature whereIsThisCard;

    private VisualStates tempVisualState;

    public override bool CanDrag => base.CanDrag && manager.CanBePlayedNow;

    public override void OnStartDrag()
    {
        base.OnStartDrag();
        tempVisualState = whereIsThisCard.VisualState;
        whereIsThisCard.VisualState = VisualStates.Dragging;
    }

    public override void OnEndDrag()
    {
        var target = GetSelectedGameObject((tag) => tag.IsPlayer() || tag.IsCreature());
        var owner = tag.GetOwnerFromTag();
        var targetIsValid = Targets.IsTargetValid(owner, target);

        if (targetIsValid)
            targetIsValid = AttackTarget(owner, target);

        if (!targetIsValid)
        { 
            whereIsThisCard.VisualState = tempVisualState;
            whereIsThisCard.BringToHandCards();
        }

        base.OnEndDrag();
    }

    private bool AttackTarget(Player owner, GameObject target)
    {
        var targetId = target.GetObjectUniqueID();
        var targetCharacter = target.tag.IsPlayer() ?
            target.tag.GetOwnerFromTag() as ICharacter :
            CreatureLogic.FindCreatureLogicById(targetId);

        if (targetCharacter != null)
            owner.PlaySpellFromHand(this.GetObjectUniqueID(), targetCharacter);

        return targetCharacter != null;
    }
}
