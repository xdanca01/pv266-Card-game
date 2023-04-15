using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using System.ComponentModel;

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

    public override IReadOnlyList<CardSlot> PossibleTargets()
    {
        return this.ability.Type switch
        {
            AbilityType.LightAttack or AbilityType.HeavyAttack => battlefield
                .Slots(!battlefield.FindPosition(this.executor).Ally).Cast<CardSlot>()
                .Where(slot => !slot.IsEmpty())
                .ToArray(),
            AbilityType.Heal => battlefield
                .Slots(battlefield.FindPosition(this.executor).Ally).Cast<CardSlot>()
                .Where(slot => !slot.IsEmpty())
                .ToList(),
            _ => throw new System.Exception("Unknown AbilityType"),
        };
    }
}