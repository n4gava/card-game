using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CardLogic: IIdentifiable
{
    private static Dictionary<int, CardLogic> CardsCreatedThisGame = new Dictionary<int, CardLogic>();

    public static void ResetCardsLogic()
    {
        CardsCreatedThisGame.Clear();
    }

    public static CardLogic FindCardLogicById(int uniqueCardId)
    {
        CardsCreatedThisGame.TryGetValue(uniqueCardId, out var cardLogic);
        return cardLogic;
    }


    private SpellEffect _effect;

    public CardLogic(Player owner, CardAsset cardAsset)
    {
        ID = IDFactory.GetUniqueID();
        Owner = owner;
        CardAsset = cardAsset;
        CardsCreatedThisGame.Add(ID, this);

        LoadCardLogicFromCardAsset(cardAsset);
    }

    public int ID { get; private set; }

    public Player Owner { get; private set; }

    public CardAsset CardAsset { get; private set; }

    public int CurrentManaCost{ get; set; }

    public bool CanBePlayed
    {
        get
        {
            var ownersTurn = (TurnManager.Instance.WhoseTurn == Owner);
            var fieldIsFull = CardAsset.IsCreatureCard ? Owner.table.CreaturesOnTable.Count >= GlobalSettings.Instance.MaxCreaturesOnTable : false;
            var hasMana = CurrentManaCost <= Owner.ManaLeft;

            return ownersTurn && !fieldIsFull && hasMana;
        }
    }

    public void ActivateCardEffect(ICharacter target)
    {
        if (_effect == null)
        {
            Debug.LogWarning($"Spell Card {CardAsset.name} does not have effect");
            return;
        }

        _effect.ActivateEffect(CardAsset.SpecialSpellAmount, target);
    }

    private void LoadCardLogicFromCardAsset(CardAsset cardAsset)
    {
        CurrentManaCost = cardAsset.ManaCost;
        LoadCardEffect(cardAsset);
    }

    private void LoadCardEffect(CardAsset cardAsset)
    {
        if (!string.IsNullOrEmpty(cardAsset.SpellScriptName))
        {
            _effect = Activator.CreateInstance(Type.GetType(cardAsset.SpellScriptName)) as SpellEffect;
        }
    }
}
