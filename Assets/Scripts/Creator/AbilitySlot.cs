using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlot : ItemSlot<Ability, AbilitySlot>, IPointerEnterHandler, IPointerExitHandler
{
    public CardSlot CardSlot { private get; set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.icon != null)
        {
            CardSlot.RemoveFlag(CardFlag.Entered);
            icon.TextColor = FSColor.Green;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.icon != null)
        {
            CardSlot.AddFlag(CardFlag.Entered);
            icon.TextColor = FSColor.White;
        }
    }
}
