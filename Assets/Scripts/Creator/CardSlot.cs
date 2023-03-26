﻿using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CardSlot : MonoBehaviour
{
    private GameObject unit;
    private GameObject upgrade;
    private Creator creator;

    private GameObject Empty => gameObject.transform.GetChild(0).gameObject;

    public static CardSlot New(string reason, GameObject parent, Vector2 position)
    {
        var creator = new Creator(reason, parent).Background();
        var cardSlot = creator.gameobject.GetOrAddComponent<CardSlot>();
        cardSlot.creator = creator;
        cardSlot.gameObject.transform.position = position;
        return cardSlot;
    }

    // if icon = null then slot will become empty
    public void SetUnit(Unit unit)
    {
        if (this.unit != null)
        {
            DestroyImmediate(this.unit);
        }
        if (unit == null)
        {
            Empty.SetActive(upgrade == null);
            this.unit = null;
        }
        else
        {
            Empty.SetActive(false);
            this.unit = unit.Card.gameobject;
            this.unit.transform.SetParent(gameObject.transform);
            this.unit.transform.position = this.gameObject.transform.position;
        }
    }

    public void SetUpgrade(Upgrade upgrade)
    {
        if (this.upgrade != null)
        {
            DestroyImmediate(this.upgrade);
        }
        if (upgrade == null)
        {
            Empty.SetActive(unit == null);
            this.upgrade = null;
        }
        else
        {
            Empty.SetActive(false);
            this.upgrade = upgrade.Card.gameobject;
            this.upgrade.transform.SetParent(gameObject.transform);
            this.upgrade.transform.position = this.gameObject.transform.position;
        }
    }
}
