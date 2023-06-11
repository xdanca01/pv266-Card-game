using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Unity.VisualScripting;

public class AI : MonoBehaviour
{
    private Battlefield b;
    private List<CardSlot> targeted = new();
    public static AI instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    /// <summary>
    /// Calculates moves for each Monster.
    /// </summary>
    /// <returns>
    ///     Returns Dictionary<Me, <Target, Ability>>
    /// </returns>
    public Dictionary<CardSlot, CardAction> chooseTargets(Battlefield bf)
    {
        b = bf;
        Dictionary<CardSlot, CardAction> actions = new Dictionary<CardSlot, CardAction>();
        //calculate priority
        Dictionary<CardSlot, float> priority = new();
        float prio = 10.0f;
        bool empty = true;
        foreach (var ally in bf.AllySlots)
        {
            if (!ally.IsEmpty())
            {
                empty = false;
                break;
            }
        }
        if (empty)
        {
            return actions;
        }
        
        for(int row = 0; row < bf.AllySlots.GetLength(0); ++row)
        {
            for (int col = 0; col < bf.AllySlots.GetLength(1); ++col)
            {
                if (bf.AllySlots[row, col].IsEmpty())
                {
                    continue;
                }
                prio = getPriority(bf.AllySlots[row, col]);
                priority.Add(bf.AllySlots[row, col], prio);
            }
        }

        //Calculate actions
        float chance = 0.0f;
        //Target chance between 60% and 100% (average for each attack)
        float wantedChance = UnityEngine.Random.Range(0.6f, 1.0f);
        float damage = 0.0f;
        CardSlot target = getTargetWithPriority(priority);
        for (int row = 0; row < bf.EnemySlots.GetLength(0); ++row)
        {
            for (int col = 0; col < bf.EnemySlots.GetLength(1); ++col)
            {
                Ability bestAttack = null;
                if (bf.EnemySlots[row, col].IsEmpty())
                {
                    continue;
                }
                foreach (Ability attack in bf.EnemySlots[row, col].GetUnit().Abilities)
                {
                    if(attack == null)
                    {
                        continue;
                    }
                    float perc = (float)attack.Percentage / 100.0f;
                    if ((attack.Type != AbilityType.Heal && chance + perc >= wantedChance) ||
                        (attack.Type != AbilityType.Heal && bestAttack == null))
                    {
                        bestAttack = attack;
                    }
                }
                if (bestAttack == null)
                {
                    continue;
                }
                //If the damage is higher without this attack, add percentage
                if (damage >= target.GetUnit().HP)
                {
                    float attackDamage = (bestAttack.Low + bestAttack.High) / 2.0f;
                    float damageRatio = attackDamage / damage;
                    chance += (float)bestAttack.Percentage / 100.0f * damageRatio;
                }
                else
                {
                    damage += (((float)bestAttack.Low + (float)bestAttack.High) / 2.0f);
                    //average chance for each attack
                    chance += (chance * actions.Count + (float)bestAttack.Percentage / 100.0f) / ((float)actions.Count + 1.0f);
                }
                CardAction t = new AbilityAction(bf, bf.EnemySlots[row, col], bestAttack);
                t.Assign(target);
                actions.Add(bf.EnemySlots[row, col], t);
            }
            if (damage > target.GetUnit().HP && chance >= wantedChance)
            {
                //TODO change target to another one in other row if it exists
                priority.Remove(target);
                target = getTargetWithPriority(priority);
            }
        }
        return actions;
    }

    private bool isTargetImmovable(CardSlot target)
    {
        if (target)
        {
            return true;
        }
        return false;
    }

    private bool isItNonCoveringImmovable(CardSlot target)
    {
        if (isTargetImmovable(target))
        {
            List<CardSlot> inFront = new();
            int col, row;
            var indexes = IndexesOf(target, b.AllySlots);
            row = indexes.Item1;
            col = indexes.Item2;
            foreach (CardSlot slot in b.AllySlots)
            {
                Tuple<int, int> indexes2 = IndexesOf(slot, b.AllySlots);
                if (row == indexes2.Item1 && !b.AllySlots[indexes2.Item1, indexes2.Item2].IsEmpty() && !isTargetImmovable(slot))
                {
                    inFront.Add(slot);
                }
            }
            if(inFront.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    private CardSlot getTargetWithPriority(Dictionary<CardSlot, float> list)
    {
        CardSlot best = null;
        float priority = 0.0f;
        foreach(var unit in list)
        {
            if(priority < unit.Value && !isItNonCoveringImmovable(unit.Key))
            {
                best = unit.Key;
            }
        }
        List<CardSlot> units = getUnitsInFront(best);
        if (units.Count == 0)
        {
            return best;
        }
        best = units[units.Count - 1];
        //Target already computed before
        if(targeted.Contains(best) == true && list.Count > 1)
        {
            var listCopy = list;
            listCopy.Remove(best);
            best = getTargetWithPriority(listCopy);
        }
        if(best == null)
        {
            best = targeted[0];
        }
        else
        {
            targeted.Add(best);
        }
        return best;
    }

    //Including me (hero)
    private List<CardSlot> getUnitsInFront(CardSlot hero)
    {
        List<CardSlot> inFront = new();
        int col, row;
        var indexes = IndexesOf(hero, b.AllySlots);
        row = indexes.Item1;
        col = indexes.Item2;
        foreach(CardSlot slot in b.AllySlots)
        {
            Tuple<int, int> indexes2 = IndexesOf(slot, b.AllySlots);
            if(row == indexes2.Item1 && col <= indexes2.Item2 && !b.AllySlots[indexes2.Item1, indexes2.Item2].IsEmpty())
            {
                inFront.Add(slot);
            }
        }
        return inFront;
    }

    private float getPriorityAttack(IUnit unit)
    {
        float attackScore = 0;
        //Doesnt have any attack, so we prevent division by 0
        if(unit.Abilities.Count <= 0)
        {
            return 0.0f;
        }
        foreach(Ability attack in unit.Abilities)
        {
            float chance = (float)attack.Percentage / 100.0f;
            attackScore += (float)(attack.Low + attack.High) / 2.0f * chance;
        }
        attackScore /= ((float)unit.Abilities.Count * 10.0f);
        attackScore *= 3.0f;
        return attackScore;
    }

    private float getPriority(CardSlot unit)
    {
        float priority = 10.0f;
        //HP
        priority -= (float)unit.GetUnit().HP / 10.0f;
        //Average attack
        priority += getPriorityAttack(unit.GetUnit());
        //heroes in front of me
        foreach (var frontHero in getUnitsInFront(unit))
        {
            if (frontHero != unit)
            {
                priority -= frontHero.GetUnit().HP / 20.0f;
            }
        }
        return priority;
    }

    private Tuple<int, int> IndexesOf(CardSlot hero, CardSlot[,] slots)
    {
        for(int row = 0; row < slots.GetLength(0); ++row)
        {
            for (int col = 0; col < slots.GetLength(1); ++col)
            {
                if(hero == slots[row, col])
                {
                    return new Tuple<int, int>(row, col);
                }
            }
        }
        return new Tuple<int, int>(-1, -1);
    }

    private void moveActions(Battlefield bf)
    {

    }
}
