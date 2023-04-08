using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlot : ItemSlot<Ability, AbilitySlot>, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.icon != null)
        {
            icon.TextColor = FSColor.Green;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.icon != null)
        {
            icon.TextColor = FSColor.White;
        }
    }
}
