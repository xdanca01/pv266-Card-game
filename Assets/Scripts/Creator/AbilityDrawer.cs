using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityDrawer : SlotDrawer<Ability, AbilitySlot, AbilityDrawer>
{
    public static AbilityDrawer New(Creator creator)
    {
        return New(creator, "Abilities", 3, 1, new Vector2(-2, -3.25f));
    }
    public void SetCardSlot(CardSlot cardSlot)
    {
        foreach (var slot in list)
        {
            slot.CardSlot = cardSlot;
        }
    }
}
