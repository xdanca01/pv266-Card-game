using UnityEngine;

public class AbilityColor
{
    private static readonly Color FastAttackColor = new(0.75f, 0.625f, 0, 0.5f);
    private static readonly Color SlowAttackColor = new(0.75f, 0.375f, 0, 0.5f);
    private static readonly Color HealColor = new(0.75f, 0, 0, 0.5f);

    public static Color Get(AbilityType type) => type switch
    {
        AbilityType.FastAttack => FastAttackColor,
        AbilityType.SlowAttack => SlowAttackColor,
        AbilityType.Heal => HealColor,
        _ => throw new System.NotImplementedException(),
    };
}
