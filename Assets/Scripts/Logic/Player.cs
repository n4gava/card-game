using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, ICharacter
{
    public static Player[] Players;

    public int PlayerID;
    public CharacterAsset charAsset;
    public PlayerArea PlayerArea;
    public SpellEffect HeroPowerEffect;

    public Deck deck;
    public Hand hand;
    public Table table;

    private int bonusManaThisTurn = 0;

    public int ID => PlayerID;

    public Player OtherPlayer => Players[0] == this ? Players[1] : Players[0];
    public bool usedHeroPowerThisTurn { get; set; } = false;

    private int manaThisTurn;
    public int ManaThisTurn
    {
        get 
        {             
            return manaThisTurn;
        }
        set
        {
            if (value < 0)
                manaThisTurn = 0;
            else if (value > PlayerArea.ManaBar.Crystals.Length)
                manaThisTurn = PlayerArea.ManaBar.Crystals.Length;
            else
                manaThisTurn = value;
            new UpdateManaCrystalsCommand(this, manaThisTurn, manaLeft).AddToQueue();
        }
    }

    private int manaLeft;
    public int ManaLeft
    {
        get
        { 
            return manaLeft;
        }
        set
        {
            if (value < 0)
                manaLeft = 0;
            else if (value > PlayerArea.ManaBar.Crystals.Length)
                manaLeft = PlayerArea.ManaBar.Crystals.Length;
            else
                manaLeft = value;
            
            new UpdateManaCrystalsCommand(this, ManaThisTurn, manaLeft).AddToQueue();
            if (TurnManager.Instance.WhoseTurn == this)
                HighlightPlayableCards();
        }
    }

    public int Health { get; private set; }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health > charAsset.MaxHealth)
            Health = charAsset.MaxHealth;

        if (Health <= 0)
            Die();
    }

    public int AttackPower => 0;

    // CODE FOR EVENTS TO LET CREATURES KNOW WHEN TO CAUSE EFFECTS
    public delegate void VoidWithNoArguments();
    //public event VoidWithNoArguments CreaturePlayedEvent;
    //public event VoidWithNoArguments SpellPlayedEvent;
    //public event VoidWithNoArguments StartTurnEvent;
    public event VoidWithNoArguments EndTurnEvent;


    void Awake()
    {
        // find all scripts of type Player and store them in Players array
        // (we should have only 2 players in the scene)
        Players = GameObject.FindObjectsOfType<Player>();
        // obtain unique id from IDFactory
        PlayerID = IDFactory.GetUniqueID();
    }

    public virtual void OnTurnStart()
    {
        usedHeroPowerThisTurn = false;
        ManaThisTurn++;
        ManaLeft = ManaThisTurn;
        foreach (CreatureLogic cl in table.CreaturesOnTable)
            cl.OnTurnStart();
        PlayerArea.HeroPower.WasUsedThisTurn = false;
    }

    public void OnTurnEnd()
    {
        if(EndTurnEvent != null)
            EndTurnEvent.Invoke();
        ManaThisTurn -= bonusManaThisTurn;
        bonusManaThisTurn = 0;
        GetComponent<TurnMaker>().StopAllCoroutines();
    }

    // STUFF THAT OUR PLAYER CAN DO

    // get mana from coin or other spells 
    public void GetBonusMana(int amount)
    {
        bonusManaThisTurn += amount;
        ManaThisTurn += amount;
        ManaLeft += amount;
    }

    public void DrawACard(bool fast = false)
    {
        if (deck.cards.Count > 0)
        {
            if (hand.CardsInHand.Count < GlobalSettings.Instance.MaxCardOnHand)
            {
                CardLogic newCard = new CardLogic(this, deck.cards[0]);
                hand.CardsInHand.Insert(0, newCard);
                deck.cards.RemoveAt(0);
                new DrawACardCommand(hand.CardsInHand[0], this, fast, fromDeck: true).AddToQueue(); 
            }
        }
        else
        {
            // there are no cards in the deck, take fatigue damage.
        }
       
    }

    // get card NOT from deck (a token or a coin)
    public void GetACardNotFromDeck(CardAsset cardAsset)
    {
        if (hand.CardsInHand.Count < GlobalSettings.Instance.MaxCardOnHand)
        {
            // 1) logic: add card to hand
            CardLogic newCard = new CardLogic(this, cardAsset);
            hand.CardsInHand.Insert(0, newCard);
            // 2) send message to the visual Deck
            new DrawACardCommand(hand.CardsInHand[0], this, fast: true, fromDeck: false).AddToQueue(); 
        }
        // no removal from deck because the card was not in the deck
    }

    public void PlaySpellFromHand(int spellCardId, int targetId)
    {
        var characterTarget = targetId < 0 ? null as ICharacter :
                              targetId == ID ? this as ICharacter :
                              targetId == OtherPlayer.ID ? OtherPlayer as ICharacter :
                              CreatureLogic.FindCreatureLogicById(targetId) as ICharacter;

        PlaySpellFromHand(spellCardId, characterTarget);
    }

    public void PlaySpellFromHand(int spellCardId, ICharacter target)
    {
        var cardLogic = CardLogic.FindCardLogicById(spellCardId);

        if (cardLogic == null)
            return;

        PlaySpellFromHand(cardLogic, target);
    }


    public void PlaySpellFromHand(CardLogic playedCard, ICharacter target)
    {
        ManaLeft -= playedCard.CurrentManaCost;
        playedCard.ActivateCardEffect(target);
        new PlayASpellCardCommand(this, playedCard).AddToQueue();
        hand.CardsInHand.Remove(playedCard);
    }

    public void PlayACreatureFromHand(int cardId, int tablePos)
    {
        PlayACreatureFromHand(CardLogic.FindCardLogicById(cardId), tablePos);
    }

    public void PlayACreatureFromHand(CardLogic playedCard, int tablePos)
    {
        ManaLeft -= playedCard.CurrentManaCost;
        CreatureLogic newCreature = new CreatureLogic(this, playedCard.CardAsset);
        table.CreaturesOnTable.Insert(tablePos, newCreature);
        hand.CardsInHand.Remove(playedCard);
        new PlayACreatureCommand(playedCard, this, tablePos, newCreature.ID).AddToQueue();
 
        newCreature.PlayInGame();
        HighlightPlayableCards();
    }

    public void Die()
    {
        // game over
        // block both players from taking new moves 
        PlayerArea.ControlsON = false;
        OtherPlayer.PlayerArea.ControlsON = false;
        TurnManager.Instance.StopTheTimer();
        new GameOverCommand(this).AddToQueue();
    }

    // use hero power - activate is effect like you`ve payed a spell
    public void UseHeroPower()
    {
        ManaLeft -= 2;
        usedHeroPowerThisTurn = true;
        HeroPowerEffect.ActivateEffect();
    }

    public void HighlightPlayableCards(bool removeAllHighlights = false)
    {
        foreach (CardLogic cardLogic in hand.CardsInHand)
        {
            GameObject g = IDHolder.GetGameObjectWithID(cardLogic.ID);
            if (g != null)
                g.GetComponentInChildren<OneCardManager>().CanBePlayedNow = (cardLogic.CurrentManaCost <= ManaLeft) && !removeAllHighlights;
        }

        foreach (CreatureLogic crl in table.CreaturesOnTable)
        {
            GameObject g = IDHolder.GetGameObjectWithID(crl.ID);
            if(g != null)
                g.GetComponentInChildren<OneCreatureManager>().CanAttackNow = (crl.AttacksLeftThisTurn > 0) && !removeAllHighlights;
        }   

        PlayerArea.HeroPower.Highlighted = (!usedHeroPowerThisTurn) && (ManaLeft > 1) && !removeAllHighlights;
    }

    // START GAME METHODS
    public void LoadCharacterInfoFromAsset()
    {
        Health = charAsset.MaxHealth;
        // change the visuals for portrait, hero power, etc...
        PlayerArea.Portrait.SetCharacterAsset(charAsset);

        if (charAsset.HeroPowerName != null && charAsset.HeroPowerName != "")
        {
            HeroPowerEffect = System.Activator.CreateInstance(System.Type.GetType(charAsset.HeroPowerName)) as SpellEffect;
        }
        else
        {
            Debug.LogWarning("Check hero powr name for character " + charAsset.ClassName);
        }
    }

    public void TransmitInfoAboutPlayerToVisual()
    {
        PlayerArea.Portrait.gameObject.AddComponent<IDHolder>().UniqueID = PlayerID;
        if (GetComponent<TurnMaker>() is AITurnMaker)
        {
            // turn off turn making for this character
            PlayerArea.AllowedToControlThisPlayer = false;
        }
        else
        {
            // allow turn making for this character
            PlayerArea.AllowedToControlThisPlayer = true;
        }
    }


}
