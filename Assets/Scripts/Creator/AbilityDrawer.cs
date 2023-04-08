using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDrawer : SlotDrawer<Ability, AbilitySlot, AbilityDrawer>
{
    public void SetCardSlot(CardSlot cardSlot)
    {
        foreach (var slot in list)
        {
            slot.CardSlot = cardSlot;
        }
    }
}
