using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using System.ComponentModel;
using System;
using UnityEngine.UI;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit { }
}

public class Battlefield : MonoBehaviour
{
    public CardSlot[,] AllySlots { get; private set; }
    public CardSlot[,] EnemySlots { get; private set; }

    public List<CardSlot> PlacementSlots { get; private set; }

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
        var creator = new Creator(title, parent);
        var battlefield = creator.gameobject.AddComponent<Battlefield>();
        var gameobject = creator.gameobject;
        var battleLayout = gameobject.AddComponent<VerticalLayoutGroup>();
        battleLayout.childForceExpandHeight = false;
        battleLayout.childForceExpandWidth = false;
        battleLayout.childControlWidth = false;
        battleLayout.childControlHeight = false;
        gameobject.GetComponent<RectTransform>().SetParent(parent.transform);
        battlefield.AllySlots = new CardSlot[rowsCount, columnsCount];
        battlefield.EnemySlots = new CardSlot[rowsCount, columnsCount];
        battlefield.actions = new();
        var sides = creator.FindGameObject("Sides");
        var rectSides = sides.GetComponent<RectTransform>();
        rectSides.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Generator.WorldWidth);
        var bottomSize = 1.25f * Generator.RowSize + 2;
        rectSides.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Generator.WorldHeight - bottomSize);
        var horizontal = sides.AddComponent<HorizontalLayoutGroup>();       
        horizontal.childControlHeight = true;
        horizontal.childControlWidth = true;
        GameObject CreateSideObject(string name, bool friendly) {
            var gameobject = creator.FindGameObject(name);
            gameobject.GetComponent<RectTransform>().SetParent(sides.transform);
            var gridLayout = gameobject.AddComponent<GridLayoutGroup>();
            var rect = gridLayout.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (columnsCount + 1f) * Generator.ColumnSize);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (rowsCount + 1) * Generator.RowSize);
            gridLayout.cellSize = new Vector2(Creator.cardWidthWithBorder, Creator.cardHeightWithBorder);
            gridLayout.spacing = new Vector2(Generator.ColumnSize - Creator.cardWidthWithBorder, Generator.RowSize - Creator.cardHeightWithBorder);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.childAlignment = TextAnchor.MiddleCenter;
            gridLayout.constraintCount = (int) columnsCount;
            var image = gameobject.AddComponent<Image>();
            image.color = friendly ? new Color(120f / 255f, 113f / 255f, 53f / 255f) : new Color(120f / 255f, 71 / 255f, 55f / 255f);
            return gameobject;
        }        
        var allies = CreateSideObject("Allies", true);
        var enemies = CreateSideObject("Enemies", false);
        var placements = creator.FindGameObject("Placements");
        var plGrid = placements.AddComponent<GridLayoutGroup>();
        plGrid.cellSize = new Vector2(Creator.cardWidthWithBorder, Creator.cardHeightWithBorder);
        plGrid.spacing = new Vector2(Generator.ColumnSize - Creator.cardWidthWithBorder, Generator.RowSize - Creator.cardHeightWithBorder);
        plGrid.padding = new RectOffset(1, 0, 1, 0);
        var plGridSides = plGrid.GetComponent<RectTransform>();
        plGridSides.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Generator.WorldWidth);
        plGridSides.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bottomSize);
        var plImage = placements.AddComponent<Image>();
        plImage.color = new Color(47f / 255f, 54f / 255f, 64f / 255f);
        for (uint row = 0; row < rowsCount; row++)
        {
            for (uint column = 0; column < columnsCount; column++)
            {
                battlefield.AllySlots[row, column] = CardSlot.New(
                    creator, "Ally Row " + row + " Column " + column, allies,
                    Vector2.zero, battlefield, CardSlotType.Ally);
                battlefield.EnemySlots[row, column] = CardSlot.New(
                    creator, "Enemy Row " + row + " Column " + column, enemies,
                    Vector2.zero, battlefield, CardSlotType.Enemy);
            }
        }
        var table = Resources.Load<TextAsset>("Data/Battlefield").text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
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
        battlefield.PlacementSlots = new();
        if (Application.isPlaying)
        {
            foreach (var (hero, i) in Deck.instance.deckOfHeroes.Select((val, i) => (val, i)))
            {
                CardSlot slot = CardSlot.New(creator, "Placement Slot " + i, placements,
                    new Vector2(i * (Generator.ColumnSize), -(rowsCount + 2) * Generator.RowSize),  
                    battlefield, CardSlotType.Placement);
                slot.SetUnit(hero.data.FreshCopy(gameobject));
                battlefield.PlacementSlots.Add(slot);
            }
            var numberOfHeroes = Deck.instance.deckOfHeroes.Count;
            foreach (var (upgrade, i) in Deck.instance.deckOfUpgrades.Select((val, i) => (val,i)))
            {
                CardSlot slot = CardSlot.New(creator, "Placement Slot " + i, placements,
                    new Vector2((numberOfHeroes + i) * (Generator.ColumnSize), -(rowsCount + 2) * Generator.RowSize),
                    battlefield, CardSlotType.Placement);
                slot.SetUpgrade(upgrade.data.FreshCopy(gameobject));
                battlefield.PlacementSlots.Add(slot);
            }
        }
        return battlefield;
    }

    public CardSlot[,] Slots(bool ally) => ally ? AllySlots : EnemySlots;

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
        for (uint i = 0; i < PlacementSlots.Count; i++)
        {
            if (PlacementSlots[(int)i] == of)
            {
                return new CardPosition { Ally = true, Row = uint.MaxValue, Column = i };
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
        return actions.Values.Where(a => a.GetType() != typeof(MoveAction))
            .Concat(actions.Values.Where(a => a.GetType() == typeof(MoveAction)));
    }

    private void ReenumeratePlacementSlots()
    {
        for (int i = PlacementSlots.Count-1; i >= 0; i--)
        {
            if (PlacementSlots[i].IsEmpty() && PlacementSlots[i].GetUpgrade() == null)
            {
                Destroy(PlacementSlots[i].gameObject);
                PlacementSlots.RemoveAt(i);
            }
        }
        foreach (var (slot, i) in PlacementSlots.Select((slot, i) => (slot, i)))
        {
            slot.SetPosition(new Vector2(i * (Generator.ColumnSize), -(AllySlots.GetLength(0) + 2) * Generator.RowSize));
        }
    }

    private void ForEachUnit(Action<Unit> action)
    {
        foreach (var ally in AllySlots)
        {
            if (!ally.IsEmpty())
            {
                action(ally.GetUnit());
            }
        }
        foreach (var enemy in EnemySlots)
        {
            if (!enemy.IsEmpty())
            {
                action(enemy.GetUnit());
            }
        }
    }

    private bool HasAnyUnitWithAbility()
    {
        bool allies = false;
        foreach (var ally in AllySlots)
        {
            if (!ally.IsEmpty() && ally.GetUnit().Abilities.Any())
            {
                allies = true;
                break;
            }
        }
        return allies;
    }

    private bool ConcludeBattleIfEnded()
    {
        bool enemies = false;
        foreach (var enemy in EnemySlots)
        {
            if (!enemy.IsEmpty())
            {
                enemies = true;
                break;
            }
        }
        if (!enemies)
        {
            Debug.Log("Good Job :)");
            CameraController.instance.CameraIslands();
            Rewards.instance.GiveSomeReward();
            return true;
        }
        if (!HasAnyUnitWithAbility() && !PlacementSlots.Any())
        {
            Debug.Log("You Died :(");
            return true;
        }
        return false;
    }

    [EditorCools.Button]
    public void NextRound()
    {
        ForEachUnit(u => u.RoundStart());
        if (ConcludeBattleIfEnded())
        {
            return;
        }
        if (HasAnyUnitWithAbility())
        {
            addAiActions();
        }
        foreach (var action in GetActionsInOrderOfExecution())
        {
            action.GetExecutor().RemoveActionLine();
            action.Execute();
        }
        actions.Clear();
        ReenumeratePlacementSlots();
        foreach (var ally in AllySlots)
        {
            ally.ClearUnitIfDead();
        }
        foreach (var enemy in EnemySlots)
        {
            enemy.ClearUnitIfDead();
        }
        ForEachUnit(u => u.RoundEnd());
        ConcludeBattleIfEnded();
    }
}
