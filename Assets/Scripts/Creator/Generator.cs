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
    private Dictionary<string, Upgrade> upgradesGenerated;
    private Dictionary<string, Ability> abilitiesGenerated;
    private Dictionary<string, Unit> unitsGenerated;
    public Battlefield battlefield { get; private set; }

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
        Deck.instance.upgrades = upgrades;
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
        Deck.instance.heroes = units;
        return (upgrades, abilities, units);
    }

    [EditorCools.Button]
    public Battlefield CreateBattlefield()
    {
        return CreateBattlefield("Dono");
    }

    public Battlefield CreateBattlefield(string mapName)
    {
        var (upgrades, abilities, units) = CreateUnits();
        upgradesGenerated = upgrades;
        abilitiesGenerated = abilities;
        unitsGenerated = units;
        var table = File.ReadLines("Assets/Data/Map.csv");
        var columnNames = table.First().Split(",");
        string basedTitle = mapName;
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
        return battlefield;
    }

    public Battlefield CreateOnlyBattlefield(string mapName)
    {
        var table = File.ReadLines("Assets/Data/Map.csv");
        var columnNames = table.First().Split(",");
        string basedTitle = mapName;
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
        GameObject.Destroy(transform.Find("Battlefields").gameObject);
        var bats = new GameObject("Battlefields");
        bats.transform.parent = transform;
        battlefield = Battlefield.New(basedTitle, unitsGenerated, upgradesGenerated, bats, rowsCount, columnsCount);
        return battlefield;
    }

    private void Update()
    {
        if (EditorApplication.isPlaying && battlefield == null)
        {
            CreateBattlefield();
        }
    }

    public GameObject GetUpgrade(string name)
    {
        GameObject upgrade;
        Transform upgrades = transform.Find("Upgrades");
        upgrade = upgrades.Find(name).gameObject;
        return upgrade;
    }
    public GameObject GetHero(string name)
    {
        GameObject hero;
        Transform heroes = transform.Find("Units");
        hero = heroes.Find(name).gameObject;
        return hero;
    }
}
