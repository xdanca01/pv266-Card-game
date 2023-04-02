using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Unity.VisualScripting;

public class AI : MonoBehaviour
{
    private ReadOnlyCollection<ReadOnlyCollection<IUnit>> Heroes;
    private ReadOnlyCollection<ReadOnlyCollection<IUnit>> Monsters;
    private List<IUnit> targeted = new();
    /// <summary>
    /// Calculates moves for each Monster.
    /// </summary>
    /// <returns>
    ///     Returns Dictionary<Me, <Target, Ability>>
    /// </returns>
    public Dictionary<Unit, Tuple<Unit, Ability>> chooseTargets(IBattlefield bf)
    {
        Monsters = (ReadOnlyCollection<ReadOnlyCollection<IUnit>>)bf.Monsters.AsReadOnlyCollection();
        Heroes = (ReadOnlyCollection<ReadOnlyCollection<IUnit>>)bf.Heroes.AsReadOnlyCollection();
        Dictionary<Unit, Tuple<Unit, Ability>> actions = new Dictionary<Unit, Tuple<Unit, Ability>>();
        
        //calculate priority
        Dictionary<Unit, float> priority = new();
        float prio = 10.0f;
        //Todo change from Interface to object
        foreach(var row in Heroes)
        {
            foreach(Unit unit in row)
            {
                prio = getPriority(row, unit);
                priority.Add(unit, prio);
            }
        }

        //Calculate actions
        float chance = 0.0f;
        //Target chance between 60% and 100% (average for each attack)
        float wantedChance = UnityEngine.Random.Range(0.6f, 1.0f);
        float damage = 0.0f;
        Unit target = getTargetWithPriority(priority);
        foreach (var row in Monsters)
        {
            foreach(Unit monster in row)
            {
                Ability bestAttack = null;
                foreach(Ability attack in monster.Abilities)
                {
                    float perc = (float)attack.Percentage / 100.0f;
                    if ((attack.Type != AbilityType.Heal && chance + perc >= wantedChance) || 
                        (attack.Type != AbilityType.Heal && bestAttack == null))
                    {
                        bestAttack = attack;
                    }
                }
                //If the damage is higher without this attack, add percentage
                if(damage >= target.HP)
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
                Tuple<Unit, Ability> t = new Tuple<Unit, Ability>(target, bestAttack);
                actions.Add(monster, t);
            }
            if(damage > target.HP && chance >= wantedChance)
            {
                //TODO change target to another one in other row if it exists
                priority.Remove(target);
                target = getTargetWithPriority(priority);
            }
        }
        return actions;
    }

    private Unit getTargetWithPriority(Dictionary<Unit, float> list)
    {
        Unit best = null;
        float priority = 0.0f;
        foreach(var unit in list)
        {
            if(priority < unit.Value)
            {
                best = unit.Key;
            }
        }
        ReadOnlyCollection<IUnit> row = getRowWithUnit(best);
        List<Unit> units = getUnitsInFront(row, best);
        best = units[units.Count - 1];
        //Target already computed before
        if(targeted.Contains(best) == true)
        {

        }
        targeted.Add(best);
        return best;
    }

    //Including me (hero)
    private List<Unit> getUnitsInFront(ReadOnlyCollection<IUnit> heroes, IUnit hero)
    {
        List<Unit> inFront = new();
        int col = heroes.IndexOf(hero);
        foreach(Unit h in heroes)
        {
            if (heroes.IndexOf(h) >= col) inFront.Add(h);
        }
        return inFront;
    }

    private float getPriorityAttack(IUnit unit)
    {
        float attackScore = 0;
        foreach(Ability attack in unit.Abilities)
        {
            float chance = (float)attack.Percentage / 100.0f;
            attackScore += (float)(attack.Low + attack.High) / 2.0f * chance;
        }
        attackScore /= ((float)unit.Abilities.Count * 10.0f);
        attackScore *= 3.0f;
        return attackScore;
    }

    private ReadOnlyCollection<IUnit> getRowWithUnit(IUnit unit)
    {
        foreach(var row in Heroes)
        {
            if(row.Contains(unit) == true)
            {
                return row;
            }
        }
        return null;
    }

    private float getPriority(ReadOnlyCollection<IUnit> row, Unit unit)
    {
        float priority = 10.0f;
        //HP
        priority -= (float)unit.HP / 10.0f;
        //Average attack
        priority += getPriorityAttack(unit);
        //heroes in front of me
        foreach (var frontHero in getUnitsInFront(row, unit))
        {
            if(frontHero != unit)
            {
                priority -= frontHero.HP / 20.0f;
            }
        }
        return priority;
    }

    private void moveActions(IBattlefield bf)
    {

    }
}
