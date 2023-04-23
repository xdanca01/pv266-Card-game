// Attack damages opponent, heal restores health of allied unit
using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public enum AbilityType
{
    LightAttack,
    HeavyAttack,
    Heal,
}

public static class AbilityTypeUtils
{
    public static bool IsAttack(this AbilityType type) => type switch
    {
        AbilityType.LightAttack or AbilityType.HeavyAttack => true,
        AbilityType.Heal => false,
        _ => throw new NotImplementedException(),
    };
    public static bool IsHeal(this AbilityType type) => type switch
    {
        AbilityType.Heal => true,
        AbilityType.LightAttack or AbilityType.HeavyAttack => false,
        _ => throw new NotImplementedException(),
    };
    public static string ToShortString(this AbilityType type) => type switch
    {
        AbilityType.LightAttack => "LIGHT",
        AbilityType.HeavyAttack => "HEAVY",
        AbilityType.Heal => "HEAL",
        _ => throw new NotImplementedException(),
    };
    public static AbilityType Parse(string type) => type switch
    {
        "LIGHT" => AbilityType.LightAttack,
        "HEAVY" => AbilityType.HeavyAttack,
        "HEAL" => AbilityType.Heal,
        _ => throw new NotImplementedException(),
    };
}

// Ability is usable action that unit can make on round of fight
public interface IAbility
{
    // Percentage for successfully using the ability
    uint Percentage { get; set; }

    // Minimum output of ability
    uint Low { get; set; }

    // Maximum output of ability
    uint High { get; set; }

    // Does it heal units or damages them?
    AbilityType Type { get; }
}
