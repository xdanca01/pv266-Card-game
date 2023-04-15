using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeDrawer : SlotDrawer<Upgrade, UpgradeSlot, UpgradeDrawer>
{
    public static UpgradeDrawer New(Creator creator)
    {
        return New(creator, "Upgrades", 1, 2, new Vector2(2, -1));
    }
}
