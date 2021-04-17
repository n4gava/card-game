public class DrawACardCommand : Command {

    private readonly Player _player;
    private readonly CardLogic _cardLogic;
    private readonly bool _fast;
    private readonly bool _fromDeck;

    public DrawACardCommand(CardLogic cardLogic, Player player, bool fast, bool fromDeck)
    {        
        this._cardLogic = cardLogic;
        this._player = player;
        this._fast = fast;
        this._fromDeck = fromDeck;
    }

    public override void StartCommandExecution()
    {
        _player.PlayerArea.PDeck.CardsInDeck--;
        _player.PlayerArea.handVisual.GivePlayerACard(_cardLogic.CardAsset, _cardLogic.ID, _fast, _fromDeck);
    }
}
