using UnityEngine;
using System.Collections;

public class DamageAllOpponentCreatures : SpellEffect {

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null)
    {
        CreatureLogic[] CreaturesToDamage = TurnManager.Instance.WhoseTurn.OtherPlayer.table.CreaturesOnTable.ToArray();
        foreach (CreatureLogic creatureLogic in CreaturesToDamage)
        {
            new DealDamageCommand(creatureLogic.ID, specialAmount, healthAfter: creatureLogic.Health - specialAmount).AddToQueue();
            creatureLogic.TakeDamage(specialAmount);
        }
    }
}
