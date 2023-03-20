using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

public partial class Card : MonoBehaviour
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
        var columnsCount = 7;
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
            var ability = new Ability(gameObject, title, type, percentage, low, high, icon);
            ability.Card.SetPosition(new Vector2(ColumnSize * (i % columnsCount) - ColumnSize * columnsCount, RowSize * (i / columnsCount)));
            abilities.Add(title.ToLower(), ability);
        }
        return abilities;
    }

    [EditorCools.Button]
    private Dictionary<string, Upgrade> CreateUpgrades()
    {
        var table = File.ReadLines("Assets/Data/Effects.csv");
        var columnNames = table.First().Split(",");
        var columnsCount = 4;
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
            var upgrade = new Upgrade(gameObject, title, description, iconTitle, iconDescription, icon, color);
            upgrade.Card.SetPosition(new Vector2(ColumnSize * (i % columnsCount), RowSize * (i / columnsCount)));
            upgrades.Add(title.ToLower(), upgrade);
        }
        return upgrades;
    }

    [EditorCools.Button]
    public void CreateUnits()
    {
        DeleteAll();
        var table = File.ReadLines("Assets/Data/Units.csv");
        var columnNames = table.First().Split(",");
        var columnsCount = 7;
        var upgrades = CreateUpgrades();
        var abilities = CreateAbilities();
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
            var unit = new Unit(gameObject, title, hp, firstAbility, secondAbility, thirdAbility, firstUpgrade, secondUpgrade, artwork);
            unit.Card.SetPosition(new Vector2(ColumnSize * (i % columnsCount) - ColumnSize * columnsCount, RowSize * (i / columnsCount) - RowSize * 3));
        }
    }

    public void CreateExampleCard()
    {
        var upgrade = new Upgrade(gameObject, "Poison", "Unit you hit gets poisoned. It takes 3 damage each round.", "Hit", "Poison", "erlenmeyer", FSColor.Blue);
        upgrade.Card.SetPosition(new Vector2(7, 0));
        var ability = new Ability(gameObject, "Elven Sword", AbilityType.LightAttack, 70, 9, 6, "broadsword");
        ability.Card.SetPosition(new Vector2(14, 0));

        for(int row = 0; row < 3; row++)
        {
            for(int column = 0; column < 4; column++)
            {
                var unit = new Unit(gameObject, "Cavalier " + row + " " + column, 30, ability, null, ability, upgrade, null, "addroran");
                unit.Card.SetPosition(new Vector2(column * ColumnSize - 21, row * RowSize - 10));
            }
        }
    }


}
