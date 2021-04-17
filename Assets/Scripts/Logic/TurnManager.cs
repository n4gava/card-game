using UnityEngine;
using DG.Tweening;

public class TurnManager : MonoBehaviour {

    public static TurnManager Instance;

    public CardAsset CoinCard;
    public RopeTimer timer;
    public Animation startGameAnimation;

    public Player WhoseTurn { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        OnGameStart();
    }

    public void OnGameStart()
    {
        CleanGameCache();

        foreach (Player player in Player.Players)
        {
            StartPlayer(player);
        }

        var animation = startGameAnimation.AnimationSequence();
        animation.OnComplete(() =>
        {
            GivePlayersCards();
        });
    }

    public void StartTurn(Player player)
    {
        WhoseTurn = player;

        timer.StartTimer();
        GlobalSettings.Instance.EnableEndTurnButtonOnStart(player);
        TurnMaker turnMaker = WhoseTurn.GetComponent<TurnMaker>();
        turnMaker.OnTurnStart();
        if (turnMaker is PlayerTurnMaker)
        {
            WhoseTurn.HighlightPlayableCards();
        }
        WhoseTurn.OtherPlayer.HighlightPlayableCards(true);
    }

    private void GivePlayersCards()
    {
        int rnd = Random.Range(0, 2); 
        WhoseTurn = Player.Players[rnd];

        int initDraw = 4;
        for (int i = 0; i < initDraw; i++)
        {
            WhoseTurn.OtherPlayer.DrawACard(true);
            WhoseTurn.DrawACard(true);
        }
        WhoseTurn.OtherPlayer.DrawACard(true);
        WhoseTurn.OtherPlayer.GetACardNotFromDeck(CoinCard);
        new StartATurnCommand(WhoseTurn).AddToQueue();
    }

    private void StartPlayer(Player player)
    {
        player.ManaThisTurn = 0;
        player.ManaLeft = 0;
        player.LoadCharacterInfoFromAsset();
        player.TransmitInfoAboutPlayerToVisual();
        player.PlayerArea.PDeck.CardsInDeck = player.deck.cards.Count;
    }

    private void CleanGameCache()
    {
        CardLogic.ResetCardsLogic();
        CreatureLogic.ResetCreaturesLogic();
    }


    public void EndTurn()
    {
        timer.StopTimer();
        WhoseTurn.OnTurnEnd();

        new StartATurnCommand(WhoseTurn.OtherPlayer).AddToQueue();
    }

    public void StopTheTimer()
    {
        timer.StopTimer();
    }

}

