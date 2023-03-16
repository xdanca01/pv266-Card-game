using UnityEngine;

public class AbilityIcon : CardIcon
{
    public Ability ability;

    public override string MainText => ability.GetPercentageString();

    public override string AdditionalText => ability.GetRangeString();

    public override Sprite Badge => ability.icon.badge;

    public override Color BadgeColor => AbilityColor.Get(ability.Type);
}
