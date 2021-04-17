using UnityEngine;
using System.Collections;

public class DragCreatureAttack : DraggingTargetActions
{
    public WhereIsTheCardOrCreature whereIsThisCreature;
    public OneCreatureManager creatureManager;

    public override bool CanDrag => base.CanDrag && creatureManager.CanAttackNow;

    public override void OnStartDrag()
    {
        base.OnStartDrag();
        whereIsThisCreature.VisualState = VisualStates.Dragging;
    }


    public override void OnEndDrag()
    {
        var target = GetSelectedGameObject((tag) => (tag.IsPlayer() || tag.IsCreature()) && tag.IsEnemy(this.gameObject));
        var owner = tag.GetOwnerFromTag();
        var targetIsValid = TargetingOptions.EnemyCharacters.IsTargetValid(owner, target);
        
        if (targetIsValid)
            targetIsValid = AttackTarget(target);

        if (!targetIsValid)
        { 
            whereIsThisCreature.VisualState = tag.IsFromLow() ? VisualStates.LowTable : VisualStates.TopTable;
            whereIsThisCreature.BringToCreaturesTable();
        }

        base.OnEndDrag();
    }

    private bool AttackTarget(GameObject target)
    {
        var attackingCreatureLogic = CreatureLogic.FindCreatureLogicById(this.GetObjectUniqueID());
        var targetId = target.GetObjectUniqueID();
        var targetCharacter = target.tag.IsPlayer() ?
            target.tag.GetOwnerFromTag() as ICharacter :
            CreatureLogic.FindCreatureLogicById(targetId);

        if (targetCharacter != null)
            attackingCreatureLogic.Attack(targetCharacter);

        return targetCharacter != null;
    }

    protected override bool DragSuccessful()
    {
        return true;
    }
}
