using UnityEngine;
using System.Collections;

public class HeroPowerDrawCardTakeDamage : SpellEffect {

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null)
    {
        new DealDamageCommand(TurnManager.Instance.WhoseTurn.PlayerID, 2, TurnManager.Instance.WhoseTurn.Health - 2).AddToQueue();
        TurnManager.Instance.WhoseTurn.TakeDamage(2);
        TurnManager.Instance.WhoseTurn.DrawACard();
    }
}
