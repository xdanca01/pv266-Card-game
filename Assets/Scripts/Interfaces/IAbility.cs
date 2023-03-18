// Attack damages opponent, heal restores health of allied unit
using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public enum AbilityType
{
    FastAttack,
    SlowAttack,
    Heal,
}

public static class AbilityTypeUtils
{
    public static bool IsAttack(this AbilityType type) => type switch
    {
        AbilityType.FastAttack or AbilityType.SlowAttack => true,
        AbilityType.Heal => false,
        _ => throw new NotImplementedException(),
    };
    public static bool IsHeal(this AbilityType type) => type switch
    {
        AbilityType.Heal => true,
        AbilityType.FastAttack or AbilityType.SlowAttack => false,
        _ => throw new NotImplementedException(),
    };
    public static string ToString(this AbilityType type) => type switch
    {
        AbilityType.FastAttack => "FAST DMG",
        AbilityType.SlowAttack => "SLOW DMG",
        AbilityType.Heal => "HEAL",
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

    // Remove all applied effects
    // It is recomended that ability saves its default values in Start method
    void RemoveEffects();
}
