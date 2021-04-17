﻿using UnityEngine;
using System.Collections;

public class UpdateManaCrystalsCommand : Command {

    private Player p;
    private int TotalMana;
    private int AvailableMana;

    public UpdateManaCrystalsCommand(Player p, int TotalMana, int AvailableMana)
    {
        this.p = p;
        this.TotalMana = TotalMana;
        this.AvailableMana = AvailableMana;
    }

    public override void StartCommandExecution()
    {
        p.PlayerArea.ManaBar.SetTotalCrystals(TotalMana);
        p.PlayerArea.ManaBar.SetAvailableCrystals(AvailableMana);
        CommandExecutionComplete();
    }
}
