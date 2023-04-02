using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System;
using UnityEngine;


public class Battlefield : MonoBehaviour
{
    public CardSlot[,,] CardSlots { get; private set; }

    // Start is called before the first frame update
    public static GameObject New(string title, Dictionary<string, Unit> units, Dictionary<string, Upgrade> upgrades, GameObject parent, uint rowsCount, uint columnsCount)
    {
        var gameobject = new GameObject(title);
        var battlefield = gameobject.AddComponent<Battlefield>();
        gameobject.transform.parent = parent.transform;
        battlefield.CardSlots = new CardSlot[rowsCount, columnsCount, 2];
        Func<uint, uint, bool, Vector2> getPosition = (row, column, friendly) =>
        {
            var verticalLine = friendly ? 0 : columnsCount + 0.5f;
            return new Vector2((column + verticalLine + 1) * Generator.ColumnSize, -(row + 2) * Generator.RowSize);
        };
        for (uint row = 0; row < rowsCount; row++)
        {
            for (uint column = 0; column < columnsCount; column++)
            {
                battlefield.CardSlots[row, column, 1] = CardSlot.New(
                    "Ally Row " + row + " Column " + column, gameobject,
                    getPosition(row, column, true));
                battlefield.CardSlots[row, column, 0] = CardSlot.New(
                    "Enemy Row " + row + " Column " + column, gameobject,
                    getPosition(row, column, false));
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
                var friendly = Generator.GetColumn("Side", columns, columnNames) == "ALLY" ? 1 : 0;
                if (units.TryGetValue(unitOrEffect, out Unit unit))
                {
                    battlefield.CardSlots[row - 1, column - 1, friendly].SetUnit(unit.FreshCopy(gameobject));
                }
                else if (upgrades.TryGetValue(unitOrEffect.ToLower(), out Upgrade effect))
                {
                    battlefield.CardSlots[row - 1, column - 1, friendly].SetUpgrade(effect.FreshCopy(gameobject));
                }
            }
        }
        return gameobject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
