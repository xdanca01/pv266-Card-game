using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public interface IBattlefield
{
    // all units on the player's side of the battlefield
    // we can't add new units, but we can modify them
    IReadOnlyList<IReadOnlyList<IUnit>> Heroes { get; } 

    // all units on the enemy's side of the battlefield
    // we can't add new units, but we can modify them
    IReadOnlyList<IReadOnlyList<IUnit>> Monsters { get; }

    // get who is unit targeting with its ability
    ReadOnlyDictionary<IUnit, IUnit> Targets { get; }

    // all units on the same side of the battlefield
    // we can't add new units, but we can modify them
    ReadOnlyCollection<IUnit> AlliesOf(IUnit unit);

    // all units on the opposite side of the battlefield
    // we can't add new units, but we can modify them
    ReadOnlyCollection<IUnit> OponentsOf(IUnit unit);

    // Implementation of Effects on individual spots is left for concrete implemenation
}
