using System;
using TMPro;
using UnityEngine;

public class AbilityVisual : CardVisual
{
    public Ability ability;

    public override Sprite Artwork => ability.visual.artwork;

    public override uint Value => ability.visual.value;

    public override string Title => ability.visual.title;

    public override string Description => ability.GetPercentageString() + " " + ability.GetRangeString() + " " + ability.Type switch
    {
        AbilityType.FastAttack => "Light DMG",
        AbilityType.SlowAttack => "Heavy DMG",
        AbilityType.Heal => "Heal",
        _ => throw new System.NotImplementedException(),
    };


}
