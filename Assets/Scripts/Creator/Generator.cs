using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System;
using UnityEngine;

public class Generator : MonoBehaviour
{
    private static int ColumnSize = 7;
    private static int RowSize = 10;

    private static string GetColumn(string columnName, string[] columns, string[] columnNames)
    {
        foreach ((string name, string value) in columnNames.Zip(columns, (a, b) => (a, b)))
        {
            if (name.Equals(columnName))
            {
                return value;
            }
        }
        throw new System.Exception("Can't find column " + columnName);
    }

    [EditorCools.Button]
    private void DeleteAll()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    [EditorCools.Button]
    private Dictionary<string, Ability> CreateAbilities()
    {
        var table = File.ReadLines("Assets/Data/Abilities.csv");
        var columnNames = table.First().Split(",");
        var columnsCount = 6;
        var parent = new GameObject("Abilities");
        parent.transform.parent = gameObject.transform;
        Dictionary<string, Ability> abilities = new();
        foreach (var (line, i) in table.Skip(1).Select((val, i) => (val, i)))
        {
            var columns = line.Split(",");
            var title = GetColumn("Title", columns, columnNames);
            var percentageString = GetColumn("Percentage", columns, columnNames);            
            var percentage = uint.Parse(percentageString[..(percentageString.Length-1)]); // remove percent symbol
            var low = uint.Parse(GetColumn("Low", columns, columnNames));
            var high = uint.Parse(GetColumn("High", columns, columnNames));
            var icon = GetColumn("Icon", columns, columnNames);
            var type = AbilityTypeUtils.Parse(GetColumn("Type", columns, columnNames));
            var ability = Ability.New(parent, title, type, percentage, low, high, icon);
            ability.Card.SetPosition(new Vector2(ColumnSize * (i % columnsCount) - ColumnSize * (columnsCount + 1), RowSize * (i / columnsCount + 1)));
            abilities.Add(title.ToLower(), ability);
        }
        return abilities;
    }

    [EditorCools.Button]
    private Dictionary<string, Upgrade> CreateUpgrades()
    {
        var table = File.ReadLines("Assets/Data/Effects.csv");
        var columnNames = table.First().Split(",");
        var columnsCount = 6;
        var parent = new GameObject("Upgrades");
        parent.transform.parent = gameObject.transform;
        Dictionary<string, Upgrade> upgrades = new();
        foreach (var (line, i) in table.Skip(1).Select((val, i) => (val, i)))
        {
            var columns = line.Split(",");
            var title = GetColumn("Title", columns, columnNames);
            var description = GetColumn("Description", columns, columnNames);
            var iconTitle = GetColumn("Icon Title", columns, columnNames);
            var iconDescription = GetColumn("Icon Description", columns, columnNames);
            var icon = GetColumn("Icon", columns, columnNames);
            var color = FSColorMethods.Parse(GetColumn("Color", columns, columnNames));
            var upgrade = Upgrade.New(parent, title, description, iconTitle, iconDescription, icon, color);
            upgrade.Card.SetPosition(new Vector2(ColumnSize * (i % columnsCount + 1), RowSize * (i / columnsCount + 1)));
            upgrades.Add(title.ToLower(), upgrade);
        }
        return upgrades;
    }

    [EditorCools.Button]
    private (Dictionary<string, Upgrade>, Dictionary<string, Ability>, Dictionary<string, Unit>) CreateUnits()
    {
        DeleteAll();
        var table = File.ReadLines("Assets/Data/Units.csv");
        var columnNames = table.First().Split(",");
        var columnsCount = 6;
        var upgrades = CreateUpgrades();
        var abilities = CreateAbilities();
        var parent = new GameObject("Units");
        parent.transform.parent = gameObject.transform;
        Dictionary<string, Unit> units = new();
        foreach (var (line, i) in table.Skip(1).Select((val, i) => (val, i)))
        {
            var columns = line.Split(",");
            var title = GetColumn("Title", columns, columnNames);
            var hp = uint.Parse(GetColumn("HP", columns, columnNames));
            var artwork = GetColumn("Artwork", columns, columnNames);
            var firstAbilityStr = GetColumn("First Ability", columns, columnNames).ToLower();
            var firstAbility = firstAbilityStr != "" ? abilities[firstAbilityStr] : null;
            var secondAbilityStr = GetColumn("Second Ability", columns, columnNames).ToLower();
            var secondAbility = secondAbilityStr != "" ? abilities[secondAbilityStr] : null;
            var thirdAbilityStr = GetColumn("Third Ability", columns, columnNames).ToLower();
            var thirdAbility = thirdAbilityStr != "" ? abilities[thirdAbilityStr] : null;
            var firstUpgradeStr = GetColumn("First Upgrade", columns, columnNames).ToLower();
            var firstUpgrade = firstUpgradeStr != "" ? upgrades[firstUpgradeStr] : null;
            var secondUpgradeStr = GetColumn("Second Upgrade", columns, columnNames).ToLower();
            var secondUpgrade = secondUpgradeStr != "" ? upgrades[secondUpgradeStr] : null;
            var unit = Unit.New(parent, title, hp, firstAbility, secondAbility, thirdAbility, firstUpgrade, secondUpgrade, artwork);
            unit.Card.SetPosition(new Vector2(ColumnSize * (i % columnsCount) - ColumnSize * (columnsCount + 1), - RowSize * (i / columnsCount + 2)));
            units.Add(title, unit);
        }
        return (upgrades, abilities, units);
    }

    [EditorCools.Button]
    private void CreateBattlefield()
    {
        var (upgrades, abilities, units) = CreateUnits();
        var table = File.ReadLines("Assets/Data/Map.csv");
        var columnNames = table.First().Split(",");
        string basedTitle = "Dono";
        var rows = 0u;
        var columnss = 0u;
        foreach (var (line, i) in table.Skip(1).Select((val, i) => (val, i)))
        {
            var columns = line.Split(",");
            var title = GetColumn("Title", columns, columnNames);
            if (title == basedTitle)
            {
                rows = uint.Parse(GetColumn("Rows", columns, columnNames));
                columnss = uint.Parse(GetColumn("Columns", columns, columnNames));
                break;
            }
        }
        if (rows == 0u && columnss == 0u)
        {
            throw new System.Exception("Battlefield not found: " + basedTitle);
        }
        var bats = new GameObject("Battlefields");
        bats.transform.parent = transform;
        var parent = new GameObject(basedTitle);
        parent.transform.parent = bats.transform;
        var cardSlots = new CardSlot[rows,columnss,2];
        Func<uint, uint, bool, Vector2> getPosition = (row, column, friendly) =>
        {
            var verticalLine = friendly ? 0: columnss + 0.5f;
            return new Vector2((column + verticalLine + 1) * ColumnSize, -(row + 2) * RowSize);
        };
        for (uint row = 0; row < rows; row++)
        {
            for (uint column = 0; column < columnss; column++)
            {
                cardSlots[row, column, 1] = CardSlot.New(
                    "Ally Row " + row + " Column " + column, parent,
                    getPosition(row, column, true));
                cardSlots[row,column,0] = CardSlot.New(
                    "Enemy Row " + row + " Column " + column, parent,
                    getPosition(row, column, false));
            }
        }
        table = File.ReadLines("Assets/Data/Battlefield.csv");
        columnNames = table.First().Split(",");
        foreach (var (line, i) in table.Skip(1).Select((val, i) => (val, i)))
        {
            var columns = line.Split(",");
            var title = GetColumn("Title", columns, columnNames);
            var row = uint.Parse(GetColumn("Row", columns, columnNames));
            var column = uint.Parse(GetColumn("Column", columns, columnNames));
            if (title == basedTitle)
            {
                var unitOrEffect = GetColumn("Unit or Effect", columns, columnNames);
                var friendly = GetColumn("Side", columns, columnNames) == "ALLY" ? 1 : 0;                
                if (units.TryGetValue(unitOrEffect, out Unit unit))
                {
                    cardSlots[row - 1, column - 1, friendly].SetUnit(unit.FreshCopy(bats));
                }
                else if (upgrades.TryGetValue(unitOrEffect.ToLower(), out Upgrade effect))
                {
                    cardSlots[row - 1, column - 1, friendly].SetUpgrade(effect.FreshCopy(bats));
                }
            }
        }
    }
    
    public void CreateExampleCard()
    {
        var upgrade = Upgrade.New(gameObject, "Poison", "Unit you hit gets poisoned. It takes 3 damage each round.", "Hit", "Poison", "erlenmeyer", FSColor.Blue);
        upgrade.Card.SetPosition(new Vector2(7, 0));
        var ability = Ability.New(gameObject, "Elven Sword", AbilityType.LightAttack, 70, 9, 6, "broadsword");
        ability.Card.SetPosition(new Vector2(14, 0));

        for(int row = 0; row < 3; row++)
        {
            for(int column = 0; column < 4; column++)
            {
                var unit = Unit.New(gameObject, "Cavalier " + row + " " + column, 30, ability, null, ability, upgrade, null, "addroran");
                unit.Card.SetPosition(new Vector2(column * ColumnSize - 21, row * RowSize - 10));
            }
        }
    }
}
