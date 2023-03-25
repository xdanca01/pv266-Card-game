using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CardSlot : MonoBehaviour
{
    private GameObject gameobject;
    private GameObject unit;
    private GameObject upgrade;
    private Creator creator;

    private GameObject Empty => gameobject.transform.GetChild(0).gameObject;

    public static CardSlot New(string reason, GameObject parent, Vector2 position)
    {
        var creator = new Creator(reason, parent).Background();
        var cardSlot = creator.gameobject.GetOrAddComponent<CardSlot>();
        cardSlot.creator = creator;
        cardSlot.gameobject = cardSlot.creator.gameobject;
        cardSlot.gameobject.transform.position = position;
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
            this.unit.transform.SetParent(gameobject.transform);
            this.unit.transform.position = this.gameobject.transform.position;
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
            this.upgrade.transform.SetParent(gameobject.transform);
            this.upgrade.transform.position = this.gameobject.transform.position;
        }
    }
}
