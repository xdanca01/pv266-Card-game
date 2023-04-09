using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

public class Battlefield : MonoBehaviour
{
    public CardSlot[,] AllySlots { get; private set; }
    public CardSlot[,] EnemySlots { get; private set; }

    private Dictionary<CardSlot, CardAction> actions;

    private record CardPosition {
        public bool Ally { get; init; }
        public uint Row { get; init; }
        public uint Column { get; init; }
    }

    public static Battlefield New(string title, Dictionary<string, Unit> units,
        Dictionary<string, Upgrade> upgrades, GameObject parent, uint rowsCount, uint columnsCount)
    {
        var gameobject = new GameObject(title);
        var battlefield = gameobject.AddComponent<Battlefield>();
        gameobject.transform.parent = parent.transform;
        battlefield.AllySlots = new CardSlot[rowsCount, columnsCount];
        battlefield.EnemySlots = new CardSlot[rowsCount, columnsCount];
        battlefield.actions = new();
        Vector2 GetPosition(uint row, uint column, bool friendly) => new(
            (column + (float)(friendly ? 0 : columnsCount + 0.5f) + 1) * Generator.ColumnSize,
            -(row + 2) * Generator.RowSize);
        for (uint row = 0; row < rowsCount; row++)
        {
            for (uint column = 0; column < columnsCount; column++)
            {
                battlefield.AllySlots[row, column] = CardSlot.New(
                    "Ally Row " + row + " Column " + column, gameobject,
                    GetPosition(row, column, true), battlefield);
                battlefield.EnemySlots[row, column] = CardSlot.New(
                    "Enemy Row " + row + " Column " + column, gameobject,
                    GetPosition(row, column, false), battlefield);
            }
        }
        var table = File.ReadLines("Assets/Data/Battlefield.csv");
        var columnNames = table.First().Split(",");
        foreach (var (line, i) in table.Skip(1).Select((val, i) => (val, i)))
        {
            var columns = line.Split(",");
            var someTitle = Generator.GetColumn("Title", columns, columnNames);
            var row = uint.Parse(Generator.GetColumn("Row", columns, columnNames));
            var column = uint.Parse(Generator.GetColumn("Column", columns, columnNames));
            if (someTitle == title)
            {
                var unitOrEffect = Generator.GetColumn("Unit or Effect", columns, columnNames);
                var slots = Generator.GetColumn("Side", columns, columnNames) == "ALLY"
                    ? battlefield.AllySlots : battlefield.EnemySlots;
                if (units.TryGetValue(unitOrEffect, out Unit unit))
                {
                    slots[row - 1, column - 1].SetUnit(unit.FreshCopy(gameobject));
                }
                else if (upgrades.TryGetValue(unitOrEffect.ToLower(), out Upgrade effect))
                {
                    slots[row - 1, column - 1].SetUpgrade(effect.FreshCopy(gameobject));
                }
            }
        }
        return battlefield;
    }

    private CardSlot[,] Slots(bool ally) => ally ? AllySlots : EnemySlots;
    private CardSlot Get(CardPosition position) => Slots(position.Ally)[position.Row, position.Column];

    private CardPosition FindPosition(CardSlot of)
    {
        for (uint side = 0; side < 2; side++)
        {
            var ally = side == 0;
            var slots = Slots(ally);
            for (uint row = 0; row < slots.GetLength(0); row++)
            {
                for (uint column = 0; column < slots.GetLength(1); column++)
                {
                    if (slots[row, column] == of)
                    {
                        return new CardPosition{ Ally=ally, Row=row, Column=column};
                    }
                }
            }
        }
        throw new System.Exception("FindPosition found nothing");
    }


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
            battlefield.actions[this.executor] = this;
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
            var executorUnit = executor.GetUnit();
            executor.SetUnit(target.GetUnit());
            target.SetUnit(executorUnit);
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
}