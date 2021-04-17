using UnityEngine;

public class OneCardManager : MonoBehaviour
{
    public CardAsset cardAsset;
    public CardVisual cardVisual;
    public CardVisual cardVisualPreview;

    void Awake()
    {
        if (cardAsset != null)
        {
            LoadFromCardAsset();
        }
    }

    public void LoadFromCardAsset()
    {
        cardVisual.LoadFromCardAsset(cardAsset);
        cardVisualPreview.LoadFromCardAsset(cardAsset);
    }


    private bool canBePlayedNow = false;
    public bool CanBePlayedNow
    {
        get
        {
            return canBePlayedNow;
        }
        set
        {
            canBePlayedNow = value;
            if (canBePlayedNow)
                cardVisual.EnableGlowImage();
            else
                cardVisual.DisableGlowImage();
        }
    }
}
