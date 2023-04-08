using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilitySlot : ItemSlot<Ability, AbilitySlot>, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public CardSlot CardSlot { private get; set; }
    private FSColor actionColor;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CardSlot.actionInProgress != default)
        {
            foreach (var target in CardSlot.actionInProgress.PossibleTargets())
            {
                target.RemoveFlag(CardFlag.Highlighted);
            }
        }
        CardSlot.actionInProgress = new Battlefield.AbilityAction(CardSlot.battlefield, CardSlot, interactible);
        actionColor = interactible.Type.ToFSColor();
        foreach (var target in CardSlot.actionInProgress.PossibleTargets())
        {
            target.AddFlag(CardFlag.Highlighted);
        }
    }

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
            icon.TextColor = FSColor.White;          
            if (CardSlot.Contains(eventData.pointerCurrentRaycast.worldPosition))
            {
                CardSlot.AddFlag(CardFlag.Entered);
            }
        }
    }
}
