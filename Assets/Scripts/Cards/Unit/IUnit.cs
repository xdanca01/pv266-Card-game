using System.Collections.ObjectModel;

// Unit is a single card on a battlefied
//  * Hero = player's unit
//  * Monster = enemy's unit
public interface IUnit
{
    // get & set current unit hp
    uint HP { get; set; }

    // make a copy with same hp but without applied effects
    // It is recomended that IUnit saves its abilities Start method
    IUnit Revert();

    // all abilities unit currently has
    // we can't extend them, but we can modify them
    ReadOnlyCollection<IAbility> Abilities { get; }

    // Notice that IUnit does NOT have effects list
}
