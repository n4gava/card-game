using UnityEngine;

public class OneCreatureManager : MonoBehaviour 
{
    public CardAsset cardAsset;
    public CreatureVisual creatureVisual;
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
        creatureVisual.LoadFromCardAsset(cardAsset);
        cardVisualPreview.LoadFromCardAsset(cardAsset);
    }

    private bool canAttackNow = false;
    public bool CanAttackNow
    {
        get
        {
            return canAttackNow;
        }

        set
        {
            canAttackNow = value;
            
            if (canAttackNow)
                creatureVisual.EnableGlowImage();
            else
                creatureVisual.DisableGlowImage();
        }
    }

    public void TakeDamage(int amount, int healthAfter)
    {
        if (amount > 0)
        {
            //DamageEffect.CreateDamageEffect(transform.position, amount);
            creatureVisual.SetCreatureLife(healthAfter);
        }
    }
}
