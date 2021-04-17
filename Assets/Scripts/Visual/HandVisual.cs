using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class HandVisual : MonoBehaviour
{
    // PUBLIC FIELDS
    public AreaPosition owner;
    public bool TakeCardsOpenly = true;

    [Header("Transform References")]
    public Transform DrawPreviewSpot;
    public Transform DeckTransform;
    public Transform OtherCardDrawSourceTransform;
    public Transform PlayPreviewSpot;

    [Header("Curve card visual")]
    public AnimationCurve cardCurve;
    public float widthSize;
    public float maxDistance;
    public float rotationAngle = -5f;

    private List<GameObject> CardsInHand = new List<GameObject>();

    void Start()
    {
        AdjustCardsInHand();
    }

    public void AdjustCardsInHand()
    {
        var cardsCount = CardsInHand.Count;
        if (cardsCount <= 0)
            return;

        // Busca a distancia entre as cartas
        var cardsGap = widthSize / (cardsCount + 1);
        // se a distancia for maior que o definido, então altera para o valor definido
        cardsGap = cardsGap > maxDistance ? maxDistance : cardsGap;

        // Calcula o valor inicial a esquerda, conforme o número de cartas.
        var leftPadding = ((cardsCount - 1) / 2f) * cardsGap * -1;
        var order = 0;

        foreach (var child in CardsInHand)
        {
            child.transform.localPosition = new Vector3(leftPadding, GetYAccordingCurve(leftPadding), 0);
            var cardVisual = child.GetComponentInChildren<CardVisual>();
            cardVisual.CardCanvas.sortingOrder = order;

            var whereIsCreature = child.GetComponent<WhereIsTheCardOrCreature>();
            if (whereIsCreature == null)
                whereIsCreature = child.GetComponentInChildren<WhereIsTheCardOrCreature>();

            whereIsCreature.Slot = order;
            order++;

            child.transform.rotation = this.transform.rotation;
            child.transform.RotateAround(child.transform.position, Vector3.forward, leftPadding * rotationAngle);

            leftPadding += cardsGap;
        }
    }

    float GetYAccordingCurve(float xPosition)
    {
        var curveIndex = xPosition / (widthSize / 2);
        return cardCurve.Evaluate(curveIndex);
    }

    public void AddCard(GameObject card)
    {
        CardsInHand.Insert(0, card);
        card.transform.SetParent(this.transform);
    }

    public void RemoveCard(GameObject card)
    {
        CardsInHand.Remove(card);
    }

    public void RemoveCardAtIndex(int index)
    {
        CardsInHand.RemoveAt(index);
    }

    public GameObject GetCardAtIndex(int index)
    {
        return CardsInHand[index];
    }

    public GameObject GetCardGameObjectOnPosition(int slot)
    {
        return this.transform.GetChild(slot).gameObject;
    }

    public Vector3 GetLastCardPosition()
    {
        if (this.CardsInHand.Count == 0)
            return Vector3.zero;

        return this.transform.GetChild(this.CardsInHand.Count - 1).localPosition;
    }


    GameObject CreateACardAtPosition(CardAsset cardAsset, Vector3 position, Vector3 eulerAngles)
    {
        GameObject card;
        if (cardAsset.IsCreatureCard)
        {
            card = GameObject.Instantiate(GlobalSettings.Instance.CreatureCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
        }
        else
        {
            if (cardAsset.Targets == TargetingOptions.NoTarget)
                card = GameObject.Instantiate(GlobalSettings.Instance.NoTargetSpellCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
            else
            {
                card = GameObject.Instantiate(GlobalSettings.Instance.TargetedSpellCardPrefab, position, Quaternion.Euler(eulerAngles)) as GameObject;
                // pass targeting options to DraggingActions
                DragSpellOnTarget dragSpell = card.GetComponentInChildren<DragSpellOnTarget>();
                dragSpell.Targets = cardAsset.Targets;
            }

        }

        // apply the look of the card based on the info from CardAsset
        OneCardManager manager = card.GetComponentInChildren<OneCardManager>();
        manager.cardAsset = cardAsset;
        manager.LoadFromCardAsset();

        return card;
    }

    // gives player a new card from a given position
    public void GivePlayerACard(CardAsset cardAsset, int UniqueID, bool fast = false, bool fromDeck = true)
    {
        var lastCardPosition = GetLastCardPosition();
        var originPosition = fromDeck ? DeckTransform.position : OtherCardDrawSourceTransform.position;

        var card = CreateACardAtPosition(cardAsset, originPosition, new Vector3(0f, -180f, 0f));

        foreach (Transform t in card.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString()+"Card";

        AddCard(card);

        var whereIs = card.GetComponent<WhereIsTheCardOrCreature>();
        if (whereIs == null)
            whereIs = card.GetComponentInChildren<WhereIsTheCardOrCreature>();

        whereIs.BringToFront();
        whereIs.Slot = 0; 
        whereIs.VisualState = VisualStates.Transition;

        // pass a unique ID to this card.
        IDHolder id = card.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;

        // move card to the hand;
        Sequence s = DOTween.Sequence();
        if (!fast)
        {
            s.Append(card.transform.DOMove(DrawPreviewSpot.position, GlobalSettings.Instance.CardTransitionTime));
            if (TakeCardsOpenly)
                s.Insert(0f, card.transform.DORotate(Vector3.zero, GlobalSettings.Instance.CardTransitionTime)); 

            s.AppendInterval(GlobalSettings.Instance.CardPreviewTime);
            
            s.Append(card.transform.DOLocalMove(lastCardPosition, GlobalSettings.Instance.CardTransitionTime));
        }
        else
        {
            s.Append(card.transform.DOLocalMove(lastCardPosition, GlobalSettings.Instance.CardTransitionTimeFast));
            if (TakeCardsOpenly)    
                s.Insert(0f,card.transform.DORotate(Vector3.zero, GlobalSettings.Instance.CardTransitionTimeFast)); 
        }

        s.OnComplete(()=> {
            ChangeLastCardStatusToInHand(card, whereIs);
            AdjustCardsInHand();
        });
    }

    // this method will be called when the card arrived to hand 
    void ChangeLastCardStatusToInHand(GameObject card, WhereIsTheCardOrCreature w)
    {
        if (owner == AreaPosition.Low)
            w.VisualState = VisualStates.LowHand;
        else
            w.VisualState = VisualStates.TopHand;

        // set correct sorting order
        w.BringToHandCards();
        // end command execution for DrawACArdCommand
        Command.CommandExecutionComplete();
    }

   
    // PLAYING SPELLS

    // 2 Overloaded method to show a spell played from hand
    public void PlayASpellFromHand(int cardId)
    {
        var card = IDHolder.GetGameObjectWithID(cardId);
        PlayASpellFromHand(card);
    }

    public void PlayASpellFromHand(GameObject CardVisual)
    {
        Command.CommandExecutionComplete();
        var whereIs = CardVisual.GetComponent<WhereIsTheCardOrCreature>();
        if (whereIs == null)
            whereIs = CardVisual.GetComponentInChildren<WhereIsTheCardOrCreature>();

        whereIs.VisualState = VisualStates.Transition;
        RemoveCard(CardVisual);

        CardVisual.transform.SetParent(null);
        AdjustCardsInHand();

        Sequence s = DOTween.Sequence();
        s.Append(CardVisual.transform.DOMove(PlayPreviewSpot.position, 1f));
        s.Insert(0f, CardVisual.transform.DORotate(Vector3.zero, 1f));
        s.AppendInterval(2f);
        s.OnComplete(()=>
            {
                Destroy(CardVisual);
            });
    }


}
