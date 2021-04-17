public class StartATurnCommand : Command {

    private Player player;

    public StartATurnCommand(Player player)
    {
        this.player = player;
    }

    public override void StartCommandExecution()
    {
        TurnManager.Instance.StartTurn(player);
        CommandExecutionComplete();
    }
}
