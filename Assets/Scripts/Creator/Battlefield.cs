using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System;
using UnityEngine;

public class Battlefield : MonoBehaviour
{
    public CardSlot[,] AllySlots { get; private set; }
    public CardSlot[,] EnemySlots { get; private set; }
    private CardSlot cardSlotToMoveTo;

    // Start is called before the first frame update
    public static Battlefield New(string title, Dictionary<string, Unit> units,
        Dictionary<string, Upgrade> upgrades, GameObject parent, uint rowsCount, uint columnsCount)
    {
        var gameobject = new GameObject(title);
        var battlefield = gameobject.AddComponent<Battlefield>();
        gameobject.transform.parent = parent.transform;
        battlefield.AllySlots = new CardSlot[rowsCount, columnsCount];
        battlefield.EnemySlots = new CardSlot[rowsCount, columnsCount];
        Func<uint, uint, bool, Vector2> GetPosition = (row, column, friendly) =>
        {
            var verticalLine = friendly ? 0 : columnsCount + 0.5f;
            return new Vector2(
                (column + verticalLine + 1) * Generator.ColumnSize, 
                -(row + 2) * Generator.RowSize);
        };
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

    public void CardSlotClicked(CardSlot slot)
    {
        if (cardSlotToMoveTo != null)
        {
            cardSlotToMoveTo.SetColor(FSColor.Black);
        }
        slot.SetColor(FSColor.Green);
        cardSlotToMoveTo = slot;
    }
}
