﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

[Flags]
public enum CardFlag
{
    Default = 1,
    Entered = 2,
    Highlight = 4,
}

public class CardSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Unit unit;
    private Upgrade upgrade;
    private Creator creator;
    private Battlefield battlefield;
    private static Battlefield.CardAction actionInProgress;
    private CardFlag flag;
    private LineRenderer actionLine;
    private Background empty;

    public static CardSlot New(string reason, GameObject parent, Vector2 position, Battlefield battlefield)
    {
        var creator = new Creator(reason, parent);
        var cardSlot = creator.gameobject.AddComponent<CardSlot>();
        cardSlot.empty = Background.New(creator);
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
        CardSlotClick();
    }

    private void RemoveHighlights()
    {
        if (actionInProgress != default)
        {
            foreach (var target in actionInProgress.PossibleTargets())
            {
                target.RemoveFlag(CardFlag.Highlight);
            }
        }
    }

    private void AddHighlights()
    {
        if (actionInProgress != default)
        {
            foreach (var target in actionInProgress.PossibleTargets())
            {
                target.AddFlag(CardFlag.Highlight);
            }
        }
    }

    public void RemoveActionLine()
    {
        if (actionLine != default)
        {
            Destroy(actionLine.gameObject);
            actionLine = null;
        }
    }

    public void CardSlotClick()
    {
        if (!flag.HasFlag(CardFlag.Entered) || (actionInProgress == default && IsEmpty()))
        {
            return;
        }
        if (actionInProgress == default)
        {
            actionInProgress = new Battlefield.Move(battlefield, this);
            AddHighlights();
            return;
        }
        if (actionInProgress.Assign(this))
        {
            var executor = actionInProgress.GetExecutor();
            executor.actionLine = executor.creator.Line("Action",
                executor.gameObject.transform.position,
                transform.position,
                actionInProgress.color);
        }
        RemoveHighlights();
        actionInProgress = default;
    }

    public void AbilitySlotClick(Ability ability)
    {
        RemoveHighlights();
        actionInProgress = new Battlefield.AbilityAction(battlefield, this, ability);
        AddHighlights();
    }

    public bool IsEmpty()
    {
        return this.unit == null;
    }

    private void SetColor(FSColor color)
    {
        Background background = this.unit != null ? this.unit.Background : this.upgrade != null ? this.upgrade.Background : empty;
        background.SetColor(color);
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
        else if (flag.HasFlag(CardFlag.Highlight))
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
        RemoveEffectFromUnit();
        if (unit == null)
        {
            empty.gameObject.SetActive(upgrade == null);
            upgrade?.gameObject.SetActive(true);
            this.unit = null;
        }
        else
        {
            empty.gameObject.SetActive(false);
            upgrade?.gameObject.SetActive(false);
            this.unit = unit;
            this.unit.abilities.SetCardSlot(this);
            this.unit.transform.SetParent(gameObject.transform);
            this.unit.transform.position = this.gameObject.transform.position;
        }
        ApplyEffectOnUnit();
    }

    public void SetUpgrade(Upgrade upgrade)
    {
        RemoveEffectFromUnit();
        if (upgrade == null)
        {
            empty.gameObject.SetActive(unit == null);
            this.upgrade = null;            
        }
        else
        {
            empty.gameObject.SetActive(false);
            this.upgrade = upgrade;
            this.upgrade.transform.SetParent(gameObject.transform);
            this.upgrade.transform.position = this.gameObject.transform.position;
        }
        ApplyEffectOnUnit();
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
        return worldPosition.x >= this.empty.transform.position.x - Creator.cardWidthWithBorder/2f
            && worldPosition.x <= this.empty.transform.position.x + Creator.cardWidthWithBorder/2f
            && worldPosition.y >= this.empty.transform.position.y - Creator.cardHeightWithBorder/2f
            && worldPosition.y <= this.empty.transform.position.y + Creator.cardHeightWithBorder/2f;
    }

    private void ApplyEffectOnUnit()
    {
        if (this.upgrade != null && this.unit != null)
        {
            this.unit.ApplyEffect(this.upgrade);
        }
    }
    private void RemoveEffectFromUnit()
    {
        if (this.upgrade != null && this.unit != null)
        {
            this.unit.RemoveEffect(this.upgrade);
        }
    }
}
