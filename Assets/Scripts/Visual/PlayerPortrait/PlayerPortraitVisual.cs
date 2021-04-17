using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPortraitVisual : MonoBehaviour, IAssetVisualManager<CharacterAsset>
{
    [Header("Text Component References")]
    //public Text NameText;
    public Text HealthText;
    [Header("Image References")]
    public Image HeroPowerIconImage;
    public Image HeroPowerBackgroundImage;
    public Image PortraitImage;
    public Image PortraitBackgroundImage;

    public CharacterAsset Asset { get; set; }

    public void ApplyLookFromAsset()
    {
        if (Asset == null)
            return;

        SetHealth(Asset.MaxHealth);
        ChangeHeroPowerImage(Asset.HeroPowerIconImage);
        ChangePortraitImage(Asset.AvatarImage);

        ChangeHeroPowerBackgroundColor(Asset.HeroPowerBGTint);
        ChangePortraitBackgroundColor(Asset.AvatarBGTint);
    }


    public void SetHealth(int health)
    {
        if (HealthText != null)
            HealthText.text = health.ToString();
    }

    public void ChangeHeroPowerImage(Sprite heroPowerImage)
    {
        if (HeroPowerIconImage != null)
            HeroPowerIconImage.sprite = heroPowerImage;
    }

    public void ChangePortraitImage(Sprite portraitImage)
    {
        if (PortraitImage != null)
            PortraitImage.sprite = portraitImage;
    }
    public void ChangeHeroPowerBackgroundColor(Color color)
    {
        if (HeroPowerBackgroundImage != null)
            HeroPowerBackgroundImage.color = color;
    }

    public void ChangePortraitBackgroundColor(Color color)
    {
        if (PortraitBackgroundImage != null)
            PortraitBackgroundImage.color = color;
    }

    
    public void Explode()
    {
        Instantiate(GlobalSettings.Instance.ExplosionPrefab, transform.position, Quaternion.identity);
        Sequence s = DOTween.Sequence();
        s.PrependInterval(2f);
        s.OnComplete(() => GlobalSettings.Instance.GameOverPanel.SetActive(true));
    }
}
