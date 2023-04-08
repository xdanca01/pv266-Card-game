using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

[Flags]
public enum CardFlag
{
    Default = 1,
    Entered = 2,
    Highlighted = 4,
}

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Unit unit;
    private Upgrade upgrade;
    public Creator creator;
    public Battlefield battlefield;
    public static Battlefield.CardAction actionInProgress;
    public Battlefield.CardAction actionCommited;
    private CardFlag flag;

    private GameObject Empty => gameObject.transform.GetChild(0).gameObject;

    public static CardSlot New(string reason, GameObject parent, Vector2 position, Battlefield battlefield)
    {
        var creator = new Creator(reason, parent).Background();
        var cardSlot = creator.gameobject.AddComponent<CardSlot>();
        cardSlot.creator = creator;
        cardSlot.gameObject.transform.position = position;
        cardSlot.battlefield = battlefield;
        cardSlot.flag = CardFlag.Default;
        return cardSlot;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AddFlag(CardFlag.Entered);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RemoveFlag(CardFlag.Entered);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!flag.HasFlag(CardFlag.Entered))
        {
            return;
        }
        if (actionInProgress == default)
        {
            if (!IsEmpty())
            {
                actionInProgress = new Battlefield.Move(battlefield, this);
                foreach (var target in actionInProgress.PossibleTargets())
                {
                    target.AddFlag(CardFlag.Highlighted);
                }
            }
        }
        else
        {
            var executor = actionInProgress.GetExecutor();
            executor.actionCommited = actionInProgress;
            var from = executor.gameObject.transform.position;
            executor.creator.Line("Action", from, transform.position, actionInProgress.color);
            foreach (var target in actionInProgress.PossibleTargets())
            {
                target.RemoveFlag(CardFlag.Highlighted);
            }
            actionInProgress = default;
        }
    }

    public bool IsEmpty()
    {
        return this.unit == null;
    }

    private void SetColor(FSColor color)
    {
        GameObject background = this.unit != null ? this.unit.transform.Find("Background").gameObject : Empty;
        background.GetComponent<Image>().color = color.ToColor(0.5f);
    }

    public void AddFlag(CardFlag flag)
    {
        this.flag |= flag;
        Redraw();
    }

    public void RemoveFlag(CardFlag flag)
    {
        this.flag &= ~flag;
        Redraw();
    }

    public void Redraw()
    {
        if (flag.HasFlag(CardFlag.Entered))
        {
            SetColor(FSColor.Green);
        }
        else if (flag.HasFlag(CardFlag.Highlighted))
        {
            SetColor(FSColor.Blue);
        }
        else
        {
            SetColor(FSColor.Black);
        }
    }

    // if icon = null then slot will become empty
    public void SetUnit(Unit unit)
    {
        if (this.unit != null)
        {
            DestroyImmediate(this.unit.Card.gameobject);
        }
        if (unit == null)
        {
            Empty.SetActive(upgrade == null);
            this.unit = null;
        }
        else
        {
            Empty.SetActive(false);
            this.unit = unit;
            this.unit.abilities.SetCardSlot(this);
            this.unit.transform.SetParent(gameObject.transform);
            this.unit.transform.position = this.gameObject.transform.position;
        }
    }

    public void SetUpgrade(Upgrade upgrade)
    {
        if (this.upgrade != null)
        {
            DestroyImmediate(this.upgrade.Card.gameobject);
        }
        if (upgrade == null)
        {
            Empty.SetActive(unit == null);
            this.upgrade = null;
        }
        else
        {
            Empty.SetActive(false);
            this.upgrade = upgrade;
            this.upgrade.transform.SetParent(gameObject.transform);
            this.upgrade.transform.position = this.gameObject.transform.position;
        }
    }

    public Unit GetUnit()
    {
        return this.unit;
    }

    public Upgrade GetUpgrade()
    {
        return this.upgrade;
    }

    public bool Contains(Vector2 worldPosition)
    {
        return worldPosition.x >= this.Empty.transform.position.x - Creator.cardWidthWithBorder/2f
            && worldPosition.x <= this.Empty.transform.position.x + Creator.cardWidthWithBorder/2f
            && worldPosition.y >= this.Empty.transform.position.y - Creator.cardHeightWithBorder/2f
            && worldPosition.y <= this.Empty.transform.position.y + Creator.cardHeightWithBorder/2f;
    }
}
