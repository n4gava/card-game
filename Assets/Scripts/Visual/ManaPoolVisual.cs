using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ManaPoolVisual : MonoBehaviour
{

    public int TestFullCrystals;
    public int TestTotalCrystalsThisTurn;

    public Image[] Crystals;
    public Text ProgressText;

    public int TotalCrystals { get; private set; }

    public int AvailableCrystals { get; private set; }

    public int MaxCrystals => Crystals.Length;

    void Update()
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            SetTotalCrystals(TestTotalCrystalsThisTurn);
            SetAvailableCrystals(TestFullCrystals);
        }
    }

    public void SetAvailableCrystals(int availableCrystals)
    {
        if (availableCrystals > TotalCrystals)
            availableCrystals = TotalCrystals;
        else if (availableCrystals < 0)
            availableCrystals = 0;

        for (int i = 0; i < TotalCrystals; i++)
        {
            Crystals[i].color = i < availableCrystals ? Color.white : Color.gray;
        }

        this.AvailableCrystals = availableCrystals;
        UpdateProgressText();
    }

    public void SetTotalCrystals(int totalCrystals)
    {
        if (totalCrystals > MaxCrystals)
            totalCrystals = MaxCrystals;
        else if (totalCrystals < 0)
            totalCrystals = 0;

        for (int i = 0; i < Crystals.Length; i++)
        {
            if (i < totalCrystals)
            {
                if (Crystals[i].color == Color.clear)
                    Crystals[i].color = Color.gray;
            }
            else
                Crystals[i].color = Color.clear;
        }

        this.TotalCrystals = totalCrystals;

        if (this.AvailableCrystals > this.TotalCrystals)
            SetAvailableCrystals(this.TotalCrystals);

        UpdateProgressText();
    }

    private void UpdateProgressText()
    {
        ProgressText.text = string.Format("{0}/{1}", AvailableCrystals.ToString(), TotalCrystals.ToString());
    }
}
