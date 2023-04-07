using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System;
using UnityEngine;
using UnityEditor;

public class Generator : MonoBehaviour
{
    public static readonly int ColumnSize = 7;
    public static readonly int RowSize = 10;
    Battlefield battlefield;
    bool generated = false;

    public static string GetColumn(string columnName, string[] columns, string[] columnNames)
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
        var rowsCount = 0u;
        var columnsCount = 0u;
        foreach (var (line, i) in table.Skip(1).Select((val, i) => (val, i)))
        {
            var columns = line.Split(",");
            var title = GetColumn("Title", columns, columnNames);
            if (title == basedTitle)
            {
                rowsCount = uint.Parse(GetColumn("Rows", columns, columnNames));
                columnsCount = uint.Parse(GetColumn("Columns", columns, columnNames));
                break;
            }
        }
        if (rowsCount == 0u && columnsCount == 0u)
        {
            throw new System.Exception("Battlefield not found: " + basedTitle);
        }
        var bats = new GameObject("Battlefields");
        bats.transform.parent = transform;
        battlefield = Battlefield.New(basedTitle, units, upgrades, bats, rowsCount, columnsCount);
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

    private void Update()
    {
        if (EditorApplication.isPlaying && !generated)
        {
            CreateBattlefield();
            generated = true;
        }
    }
}
