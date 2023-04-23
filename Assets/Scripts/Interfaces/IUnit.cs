using System.Collections.ObjectModel;

// Unit is a single card on a battlefied
//  * Hero = player's unit
//  * Monster = enemy's unit
public interface IUnit
{   
    // get & set current unit hp
    uint HP { get; set; }

    // get & set max unit hp
    uint MAX_HP { get; set; }

    public void ApplyEffect(Upgrade effect);

    public void RemoveEffect(Upgrade effect);

    // all abilities unit currently has
    // we can't extend them, but we can modify them
    ReadOnlyCollection<IAbility> Abilities { get; }

    // Notice that IUnit does NOT have effects list
}
