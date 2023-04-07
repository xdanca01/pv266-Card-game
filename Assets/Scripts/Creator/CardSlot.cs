using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject unit;
    private GameObject upgrade;
    private Creator creator;
    public Battlefield battlefield;

    private GameObject Empty => gameObject.transform.GetChild(0).gameObject;

    public static CardSlot New(string reason, GameObject parent, Vector2 position, Battlefield battlefield)
    {
        var creator = new Creator(reason, parent).Background();
        var cardSlot = creator.gameobject.AddComponent<CardSlot>();
        cardSlot.creator = creator;
        cardSlot.gameObject.transform.position = position;
        cardSlot.battlefield = battlefield;
        return cardSlot;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetColor(FSColor.Green);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetColor(FSColor.Black);
    }

    public bool IsEmpty()
    {
        return this.unit == null;
    }

    public void SetColor(FSColor color)
    {
        GameObject background = this.unit != null ? this.unit.transform.Find("Background").gameObject : Empty;
        background.GetComponent<Image>().color = color.ToColor(0.5f);
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
