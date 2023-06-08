using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class EffectsDrawer : SlotDrawer<Upgrade, UpgradeSlot, EffectsDrawer>
{
    Background background;
    Canvas canvas;

    public static EffectsDrawer New(Creator creator)
    {        
        var border = Creator.cardWidthWithBorder - Creator.cardWidth;
        var x = Creator.cardWidthWithBorder / 2 + border / 2 + Creator.hexagonWidth / 2;
        var y = 0;
        var rows = 4u;
        var columns = 1u;
        var effectDrawer = New(creator, "Effects", columns, rows,
            new Vector2(x, -Creator.hexagonHeight * (rows - 1) / 2));
        effectDrawer.background = Background.NewWithDimensions(creator, effectDrawer.gameObject,
            new Rect(x, y, Creator.hexagonWidth * columns + border, Creator.hexagonHeight * rows + border));
        effectDrawer.canvas = effectDrawer.GetComponent<Canvas>();
        if (effectDrawer.canvas == null) {
            effectDrawer.canvas = effectDrawer.AddComponent<Canvas>();
        }
        return effectDrawer;
    }
    public void Show()
    {
        canvas.overrideSorting = true;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
