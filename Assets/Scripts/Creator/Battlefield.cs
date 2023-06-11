using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using System.ComponentModel;
using System;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

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

    public string IslandName;

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

    private static Dictionary<string, Unit> scaleUnits(Dictionary<string, Unit> units)
    {
        float scalingFactor = 1.1f;
        Dictionary<string, Unit> scaledUnits = new();
        foreach (var unit in units)
        {
            var unitsParent = GameObject.FindGameObjectWithTag("Units");
            var unitCopy = unit.Value.FreshCopy(unitsParent);
            unitCopy.transform.position = unitsParent.transform.position;
            unitCopy.ScaleAbilities(Mathf.Pow(scalingFactor, MapController.loop));
            scaledUnits.Add(unit.Key, unitCopy);
        }
        return scaledUnits;
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
        var bottomSize = 1.2f * Generator.RowSize + 2;
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
            gridLayout.padding = new RectOffset(0, 0, 2, 0);
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
        var scaledUnits = scaleUnits(units);
        foreach (var (line, i) in table.Skip(1).Select((val, i) => (val, i)))
        {
            var columns = line.Split(",");
            var someTitle = Generator.GetColumn("Title", columns, columnNames);
            var row = uint.Parse(Generator.GetColumn("Row", columns, columnNames));
            var column = uint.Parse(Generator.GetColumn("Column", columns, columnNames));
            if (someTitle == title)
            {
                var unitOrEffect = Generator.GetColumn("Unit or Effect", columns, columnNames);
                var ally = Generator.GetColumn("Side", columns, columnNames) == "ALLY";
                var slots = ally ? battlefield.AllySlots : battlefield.EnemySlots;
                if (ally)
                {
                    if (units.TryGetValue(unitOrEffect, out Unit unit))
                    {
                        slots[row - 1, column - 1].SetUnit(unit.FreshCopy(gameobject));
                    }
                    else if (upgrades.TryGetValue(unitOrEffect.ToLower(), out Upgrade effect))
                    {
                        slots[row - 1, column - 1].SetUpgrade(effect.FreshCopy(gameobject));
                    }
                }
                else
                {
                    if (scaledUnits.TryGetValue(unitOrEffect, out Unit unit))
                    {
                        slots[row - 1, column - 1].SetUnit(unit.FreshCopy(gameobject));
                    }
                    else if (upgrades.TryGetValue(unitOrEffect.ToLower(), out Upgrade effect))
                    {
                        slots[row - 1, column - 1].SetUpgrade(effect.FreshCopy(gameobject));
                    }
                }
            }
        }
        battlefield.PlacementSlots = new();
        if (Application.isPlaying)
        {
            foreach (var (hero, i) in Deck.instance.deckOfHeroes.Select((val, i) => (val, i)))
            {
                CardSlot slot = CardSlot.New(creator, "Placement Unit " + i, placements,
                    new Vector2(i * (Generator.ColumnSize), -(rowsCount + 2) * Generator.RowSize),  
                    battlefield, CardSlotType.Placement);
                slot.SetUnit(hero.data.FreshCopy(gameobject));
                battlefield.PlacementSlots.Add(slot);
            }
            var numberOfHeroes = Deck.instance.deckOfHeroes.Count;
            foreach (var (upgrade, i) in Deck.instance.deckOfUpgrades.Select((val, i) => (val,i)))
            {
                CardSlot slot = CardSlot.New(creator, "Placement Upgrade " + i, placements,
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
            if(this.gameObject.name == "Tutorial 1")
            {
                Unit h;
                Deck.instance.heroes.TryGetValue(new string("Shaman"), out h);
                Rewards.instance.GiveHero(h);
                CameraController.instance.ShowPopUp("shaman", "crelanu");
            }
            else if(this.gameObject.name == "Tutorial 2")
            {
                Unit h;
                Rewards.instance.GiveUpgrade();
                CameraController.instance.ShowPopUp("upgrade", "Armory");
            }
            else if (this.gameObject.name == "Tutorial 3")
            {
                Rewards.instance.GiveTenCoins();
                CameraController.instance.ShowPopUp("10 coins", "Elven Forest");
            }
            else if (this.gameObject.name == "Sepow")
            {
                CameraController.instance.ShowWonPopUp();
            }
            else
            {
                (RewardType, string) reward = Rewards.instance.GiveSomeReward(gameObject.name);
                
                switch (reward.Item1)
                {
                    case RewardType.Upgrade:
                        CameraController.instance.ShowPopUp(reward.Item2, "Armory");
                        break;
                    case RewardType.Coins:
                        CameraController.instance.ShowPopUp(reward.Item2, "Swanport");
                        break;
                    case RewardType.Hero:
                        CameraController.instance.ShowPopUp(reward.Item2, "Sequoia Saplings");
                        break;
                }
            }
            return true;
        }
        if (!HasAnyUnitWithAbility() && PlacementSlots.All(slot => slot.GetUnit() == null))
        {
            CameraController.instance.ShowLostPopUp();
            return true;
        }
        return false;
    }

    [EditorCools.Button]
    public IEnumerator NextRound()
    {
        ForEachUnit(u => u.RoundStart());
        if (ConcludeBattleIfEnded())
        {
            yield return null;
        }
        if (HasAnyUnitWithAbility())
        {
            addAiActions();
        }
        List<LineAnimator> lineRenderers = new();
        foreach (var action in GetActionsInOrderOfExecution())
        {
            var ln = action.GetExecutor().RemoveActionLine(action);
            if (ln != null)
            {
                lineRenderers.Add(ln);
            }
        }
        yield return new WaitUntil(() => lineRenderers.All(ln => ln.IsFinished()));
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
        yield return null;
    }
}
