using System;
using UnityEngine;
using UnityEngine.UI;

public class CreatureVisual : MonoBehaviour, IAssetVisualManager<CardAsset>
{
    [Header("Text Component References")]
    public Text HealthText;
    public Text AttackText;

    [Header("Image References")]
    public Image CreatureGraphicImage;
    public Image CreatureGlowImage;

    public CardAsset Asset { get; set; }

    void Awake()
    {
        ApplyLookFromAsset();
    }

    public void ApplyLookFromAsset()
    {
        if (Asset == null)
            return;

        ChangeCardGraphicImage(Asset.CardImage);
        SetCreatureStatsFromAsset(Asset);
    }

    /// <summary>
    /// Update the creature life points
    /// </summary>
    /// <param name="life"></param>
    public void SetCreatureLife(int life)
    {
        if (HealthText != null)
            HealthText.text = life.ToString();
    }

    /// <summary>
    /// Enable create glow image
    /// </summary>
    public void EnableGlowImage()
    {
        CreatureGlowImage.enabled = true;
    }

    /// <summary>
    /// Disable creature glow image
    /// </summary>
    public void DisableGlowImage()
    {
        CreatureGlowImage.enabled = false;
    }

    /// <summary>
    /// Changes the card image
    /// </summary>
    /// <param name="cardImage">Card image</param>
    public void ChangeCardGraphicImage(Sprite cardImage)
    {
        if (CreatureGraphicImage != null)
            CreatureGraphicImage.sprite = cardImage;
    }


    private void SetCreatureStatsFromAsset(CardAsset cardAsset)
    {
        if (AttackText != null)
            AttackText.text = cardAsset.Attack.ToString();

        SetCreatureLife(cardAsset.MaxHealth);
    }
}
