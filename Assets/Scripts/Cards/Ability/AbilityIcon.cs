using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityIcon : CardIcon
{
    private static readonly Color FastAttackColor = new(0.75f, 0.625f, 0, 0.5f);
    private static readonly Color SlowAttackColor = new(0.75f, 0.375f, 0, 0.5f);
    private static readonly Color HealColor = new(0.75f, 0, 0, 0.5f);

    public Ability ability;

    public override string MainText => ability.GetPercentageString();

    public override string AdditionalText => ability.GetRangeString();

    public override Sprite Badge => ability.icon.badge;

    public override Color BadgeColor => ability.Type switch
    {
        AbilityType.FastAttack => FastAttackColor,
        AbilityType.SlowAttack => SlowAttackColor,
        AbilityType.Heal => HealColor,
        _ => throw new System.NotImplementedException(),
    };
}
