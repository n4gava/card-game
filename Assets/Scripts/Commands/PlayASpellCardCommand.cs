public class PlayASpellCardCommand: Command
{
    private readonly CardLogic _card;
    private readonly Player _player;

    public PlayASpellCardCommand(Player player, CardLogic cardLogic)
    {
        _card = cardLogic;
        _player = player;
    }

    public override void StartCommandExecution()
    {
        _player.PlayerArea.handVisual.PlayASpellFromHand(_card.ID);
    }
}
