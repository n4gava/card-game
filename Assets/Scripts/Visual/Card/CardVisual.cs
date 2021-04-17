using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[ExecuteInEditMode]
public class CardVisual : MonoBehaviour, IAssetVisualManager<CardAsset>
{
    [Header("Text Component References")]
    public Text NameText;
    public Text ManaCostText;
    public Text DescriptionText;
    public Text HealthText;
    public Text AttackText;

    [Header("Image References")]
    public Image CardGraphicImage;
    public Image CardFaceGlowImage;
    public Image CardBackGlowImage;

    [Header("Creature Card")]
    public GameObject AttackGameObject;
    public GameObject HealthGameObject;

    [Header("Card Images for Character Aspect")]
    public Image CardBodyImage;
    public Image RibbonImage;

    [Header("Canvas")]
    public Canvas CardCanvas;

    public CardAsset Asset { get; set; }

    void Awake()
    {
        ApplyLookFromAsset();
    }
    
    /// <summary>
    /// Read card asset and load its values
    /// </summary>
    public void ApplyLookFromAsset()
    {
        if (Asset == null)
            return;

        ChangeCardName(Asset.name);
        SetCardManaCost(Asset.ManaCost);
        SetCardDescription(Asset.Description);
        ChangeGraphicImage(Asset.CardImage);

        if (Asset.IsCreatureCard)
            SetCreatureCardStatsFromCardAsset(Asset);
        else
            SetSpellCardStatsFromCardAsset(Asset);

        ChangeCardColorBasedOnCharacterAsset(Asset.CharacterAsset);
    }

    public void ChangeGraphicImage(Sprite cardImage)
    {
        if (CardGraphicImage != null)
            CardGraphicImage.sprite = cardImage;
    }

    public void SetCardDescription(string description)
    {
        if (DescriptionText != null)
            DescriptionText.text = description;
    }

    public void SetCardManaCost(int manaCost)
    {
        if (ManaCostText != null)
            ManaCostText.text = manaCost.ToString();
    }

    public void ChangeCardName(string name)
    {
        if (NameText != null)
            NameText.text = name;
    }

    private void ChangeCardColorBasedOnCharacterAsset(CharacterAsset characterAsset)
    {
        if (CardBodyImage != null)
            CardBodyImage.color = characterAsset != null ? characterAsset.ClassCardTint : Color.white;

        if (RibbonImage != null)
            RibbonImage.color = characterAsset != null ? characterAsset.ClassRibbonsTint : Color.white;
    }

    /// <summary>
    /// Enable the glow image
    /// </summary>
    public void EnableGlowImage()
    {
        CardFaceGlowImage.enabled = true;
    }

    /// <summary>
    /// Disable the glow image
    /// </summary>
    public void DisableGlowImage()
    {
        CardFaceGlowImage.enabled = false;
    }

    private void SetSpellCardStatsFromCardAsset(CardAsset cardAsset)
    {
        if (AttackGameObject != null)
            AttackGameObject.SetActive(false);

        if (HealthGameObject != null)
            HealthGameObject?.SetActive(false);
    }

    private void SetCreatureCardStatsFromCardAsset(CardAsset cardAsset)
    {
        if (AttackGameObject != null)
            AttackGameObject?.SetActive(true);

        if (HealthGameObject != null)
            HealthGameObject?.SetActive(true);

        if (AttackText != null)
            AttackText.text = cardAsset.Attack.ToString();

        if (HealthText != null)
            HealthText.text = cardAsset.MaxHealth.ToString();
    }
}
