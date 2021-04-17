using System;
using UnityEngine;

public class PlayerPortraitManager : MonoBehaviour
{
    public CharacterAsset characterAsset;
    public PlayerPortraitVisual playerPortraitVisual;

    public void SetCharacterAsset(CharacterAsset characterAsset)
    {
        this.characterAsset = characterAsset;
        playerPortraitVisual.LoadFromCardAsset(characterAsset);
    }

    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            playerPortraitVisual.SetHealth(healthAfter);
        }
    }


    public void Explode()
    {
        /*
        Instantiate(GlobalSettings.Instance.ExplosionPrefab, transform.position, Quaternion.identity);
        Sequence s = DOTween.Sequence();
        s.PrependInterval(2f);
        s.OnComplete(() => GlobalSettings.Instance.GameOverPanel.SetActive(true));
        */
    }
}
