using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// This class will show damage dealt to creatures or players
/// </summary>

public class DamageEffect : MonoBehaviour {

    public CanvasGroup cg;
    public Text AmountText;


    private IEnumerator<WaitForSeconds> ShowDamageEffect()
    {
        cg.alpha = 1f;

        yield return new WaitForSeconds(0.75f);

        while (cg.alpha > 0)
        {
            cg.alpha -= 0.15f;
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(this.gameObject);
    }
    /// <summary>
    /// Creates the damage effect.
    /// This is a static method, so it should be called like this: DamageEffect.CreateDamageEffect(transform.position, 5);
    /// </summary>
    /// <param name="position">Position</param>
    /// <param name="amount">Amount of damage</param>
   
    public static void CreateDamageEffect(Vector3 position, int amount)
    {
        if (amount == 0)
            return;

        GameObject newDamageEffect = GameObject.Instantiate(GlobalSettings.Instance.DamageEffectPrefab, position, Quaternion.identity) as GameObject;
        DamageEffect de = newDamageEffect.GetComponent<DamageEffect>();
        
        if (amount < 0)
        {
            de.AmountText.text = "+" + (-amount).ToString();
            de.AmountText.color = Color.green;
        }
        else
            de.AmountText.text = "-"+amount.ToString();

        de.StartCoroutine(de.ShowDamageEffect());
    }
}
