using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using System.ComponentModel;
using UnityEngine.UIElements;

public abstract class CardAction
{
    protected CardSlot executor;
    protected CardSlot target;
    protected Battlefield battlefield;
    public FSColor color { get; private set; }

    public CardAction(Battlefield battlefield, CardSlot executor, FSColor color)
    {
        this.battlefield = battlefield;
        this.executor = executor;
        this.color = color;
    }

    public CardSlot GetExecutor()
    {
        return this.executor;
    }

    public abstract IReadOnlyList<CardSlot> PossibleTargets();

    public bool Assign(CardSlot target)
    {
        if (!PossibleTargets().Contains(target))
        {
            return false;
        }
        this.target = target;
        battlefield.AddAction(this);
        return true;
    }

    public abstract void Execute();
}

public class Move : CardAction
{
    public Move(Battlefield battlefield, CardSlot executor) : base(battlefield, executor, FSColor.Blue) { }

    public override IReadOnlyList<CardSlot> PossibleTargets()
    {
        var executorPosition = battlefield.FindPosition(this.executor);
        var slots = battlefield.Slots(executorPosition.Ally);
        List<CardSlot> list = new();
        foreach (var cardSlot in slots)
        {
            if (cardSlot != this.executor)
            {
                list.Add(cardSlot);
            }
        }
        return list;
    }

    public override void Execute()
    {
        var exPos = battlefield.FindPosition(executor);
        var tgPos = battlefield.FindPosition(target);
        if (exPos.Ally != tgPos.Ally)
        {
            throw new System.Exception("Cannot move to opponent's battlefield!");
        }
        var swapUnit = executor.GetUnit();

        executor.SetUnit(target.GetUnit());
        target.SetUnit(swapUnit);
    }
}

public class AbilityAction : CardAction
{
    Ability ability;

    public AbilityAction(Battlefield battlefield, CardSlot executor, Ability ability) : base(battlefield, executor, ability.Type.ToFSColor())
    {
        this.ability = ability;
    }

    public override void Execute()
    {
        if (this.ability.Percentage <= Random.Range(1, 101))
        {
            return;
        }
        var value = Random.Range((int)this.ability.Low, (int)this.ability.High + 1);
        var attack = this.ability.Type == AbilityType.LightAttack || this.ability.Type == AbilityType.HeavyAttack;
        var executorUnit = executor.GetUnit();
        var targetUnit = target.GetUnit();
        // Attack with positive value = affect target
        if (attack && value > 0)
        {
            targetUnit.HP = (uint)System.Math.Max(targetUnit.HP - value, 0);
        }
        // Attack with negative value = affect self negatively
        else if (attack && value < 0)
        {
            executorUnit.HP = (uint)System.Math.Max(executorUnit.HP + value, 0);
        }
        // Heal with positive value = affect target
        else if (!attack && value > 0)
        {
            targetUnit.HP = (uint)System.Math.Min(targetUnit.HP + value, targetUnit.MAX_HP);
        }
        // Heal with negative value = affect target negatively
        else if (!attack && value < 0)
        {
            targetUnit.HP = (uint)System.Math.Max(targetUnit.HP + value, 0);
        }
    }

    // attackable units are the first in each row
    private bool IsAttackable(CardSlot slot)
    {
        var position = battlefield.FindPosition(slot);
        var start = position.Ally ? (position.Column + 1) : 0;
        var end = position.Ally ? (uint) battlefield.Slots(position.Ally).GetLength(0) : position.Column;
        for (var i = start; i < end; i++)
        {
            if (!battlefield.Slots(position.Ally)[position.Row, i].IsEmpty())
            {
                return false;
            }
        }
        return true;
    }

    // healable units are those which are behind
    private bool IsHealable(CardSlot slot, Battlefield.CardPosition from)
    {
        return from.Ally ? battlefield.FindPosition(slot).Column <= from.Column : battlefield.FindPosition(slot).Column >= from.Column;
    }

    public override IReadOnlyList<CardSlot> PossibleTargets()
    {
        var position = battlefield.FindPosition(this.executor);
        return this.ability.Type switch
        {
            AbilityType.LightAttack or AbilityType.HeavyAttack => battlefield
                .Slots(!position.Ally).Cast<CardSlot>()
                .Where(slot => !slot.IsEmpty() && IsAttackable(slot))
                .ToArray(),
            AbilityType.Heal => battlefield
                .Slots(position.Ally).Cast<CardSlot>()
                .Where(slot => !slot.IsEmpty() && IsHealable(slot, position))
                .ToList(),
            _ => throw new System.Exception("Unknown AbilityType"),
        };
    }
}