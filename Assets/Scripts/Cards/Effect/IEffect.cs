// when effect should be applied
public enum EffectApplication
{
    RoundStart, // at the start of each round
    RoundEnd, // at the end of each round

    AbilitySuccess, // after successfully using some ability 
    AttackSuccess, // after successfully using some attack 
    FastSuccess, // after successfully using fast attack
    SlowSuccess, // after successfully using slow attack
    HealSuccess, // after successfully using heal

    AbilityFail, // after failing using any ability 
    AttackFail, // after failing using any attack 
    FastFail, // after failing using fast attack
    SlowFail, // after failing using slow attack
    HealFail, // after failing using heal
}

// Affect a unit once per turn on particular trigger event
// For example, it can be battlefield effect, or on an recieved effect from upgrade
// Examples:
//  * 2x Damage - at the start of the turn filter ally's abilities for all attacks and double their low and high
//  * Poisoned  - at the start of the turn deal 3 dmg to host
//  * Poison    - after successful attack add target Poisioned
//  * Heavy Blow - after successful attack search for opponents in the same row and damage them manually
public interface IEffect
{
    // when should effect be applied
    EffectApplication Condition { get; }

    // Apply effect on unit that has it (unit on is on special spot in battlefield, unit that has an upgrade, etc)
    // Assume the unit has cleared all effects on its abilities from previous turn
    // How should it work? Unit creates a fresh copy using MakeFreshCopy()
    //                     Copied unit applies all effects that happen during the round
    //                     On the end of the round unit sets its HP based on the copy and then deletes it
    void Affect(IUnit host);
}
