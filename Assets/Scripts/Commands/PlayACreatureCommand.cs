using UnityEngine;
using System.Collections;

public class PlayACreatureCommand : Command
{
    private CardLogic _cardLogic;
    private int _tablePos;
    private Player _player;
    private int _creatureID;

    public PlayACreatureCommand(CardLogic cardLogic, Player player, int tablePos, int creatureID)
    {
        this._player = player;
        this._cardLogic = cardLogic;
        this._tablePos = tablePos;
        this._creatureID = creatureID;
    }

    public override void StartCommandExecution()
    {
        var playerHand = _player.PlayerArea.handVisual;
        var card = IDHolder.GetGameObjectWithID(_cardLogic.ID);
        playerHand.RemoveCard(card);
        GameObject.Destroy(card);
        playerHand.AdjustCardsInHand();

        HoverPreview.PreviewsAllowed = true;
        _player.PlayerArea.tableVisual.AddCreatureAtIndex(_cardLogic.CardAsset, _creatureID, _tablePos);
    }
}
