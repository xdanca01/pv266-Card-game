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

    public record CardPosition {
        public bool Ally { get; init; }
        public uint Row { get; init; }
        public uint Column { get; init; }
    }

    //TODO calculate difficulty for the battlefield
    public string GetDifficulty()
    {
        return "Hard";
    }

    public int CountEnemies()
    {
        int cnt = 0;
        foreach(var slot in EnemySlots)
        {
            if(slot.IsEmpty() == false)
            {
                ++cnt;
            }
        }
        return cnt;
    }

    public void AddAction(CardAction action)
    {
        this.actions[action.GetExecutor()] = action;
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
                    GetPosition(row, column, true), battlefield, true);
                battlefield.EnemySlots[row, column] = CardSlot.New(
                    "Enemy Row " + row + " Column " + column, gameobject,
                    GetPosition(row, column, false), battlefield, false);
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

    public CardSlot[,] Slots(bool ally) => ally ? AllySlots : EnemySlots;
    private CardSlot Get(CardPosition position) => Slots(position.Ally)[position.Row, position.Column];

    public CardPosition FindPosition(CardSlot of)
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


    private void addAiActions()
    {
        var moves = AI.instance.chooseTargets(this);
        foreach(var move in moves)
        {
            if (!actions.ContainsKey(move.Key))
            {
                actions.Add(move.Key, move.Value);
            }
        }
    }

    // MoveAction must be last in order of execution

    private IEnumerable<CardAction> GetActionsInOrderOfExecution()
    {
        return actions.Values.Where(a => a.GetType() != typeof(Move))
            .Concat(actions.Values.Where(a => a.GetType() == typeof(Move)));
    }

    [EditorCools.Button]
    public void NextRound()
    {
        addAiActions();
        foreach (var action in GetActionsInOrderOfExecution())
        {
            action.GetExecutor().RemoveActionLine();
            action.Execute();
        }
        actions.Clear();
        foreach (var ally in AllySlots)
        {
            ally.ClearUnitIfDead();
        }
        foreach (var enemy in EnemySlots)
        {
            enemy.ClearUnitIfDead();
        }
    }
}
