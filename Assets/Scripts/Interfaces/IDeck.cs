using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDeck
{
    IReadOnlyList<IUnit> GetUnits();
    IReadOnlyList<IUpgrade> GetUpgrades();
}
