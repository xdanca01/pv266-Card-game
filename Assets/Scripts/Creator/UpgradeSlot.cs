using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeSlot : ItemSlot<Upgrade, UpgradeSlot>, IPointerMoveHandler, IPointerExitHandler
{
    private Vector2 cardDefaultPosition = Vector2.zero;

    public static Vector2 VisiblePosition(Vector2 originalPosition)
    {
        float screenHeight = Camera.main.orthographicSize;
        float screenWidth = screenHeight * Camera.main.aspect;
        float objectWidth = Creator.cardWidthWithBorder * 0.6f;
        float objectHeight = Creator.cardHeightWithBorder * 0.6f;
        Vector2 camPos = Camera.main.transform.position;
        Vector2 transformed = originalPosition - camPos;
        if (transformed.x + 2 * objectWidth < screenWidth)
        {
            transformed.x += objectWidth;
        }
        else
        {
            transformed.x -= objectWidth;
        }
        if (transformed.y + 2 * objectHeight < screenHeight)
        {
            transformed.y += objectHeight;
        }
        else
        {
            transformed.y -= objectHeight;
        }
        return transformed + camPos;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (interactible != default)
        {
            Vector2 position = VisiblePosition(eventData.pointerCurrentRaycast.worldPosition);
            if (cardDefaultPosition == Vector2.zero)
            {
                cardDefaultPosition = interactible.Card.gameobject.transform.position;
            }
            interactible.Card.SetPosition(position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interactible != default)
        {
            interactible.Card.SetPosition(cardDefaultPosition);
            cardDefaultPosition = Vector2.zero;
        }
    }

}
