using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public class UpgradeData
    {
        public bool active = false;
        public Upgrade data;
        public UpgradeData(Upgrade upgradeData, bool state = true)
        {
            this.active = state;
            this.data = upgradeData;
        }
    }
    List<Unit> deckOfHeroes = new();
    List<UpgradeData> deckOfUpgrades = new();

    public List<Unit> getHeroes()
    {
        return deckOfHeroes;
    }

    public void addHero(Unit hero)
    {
        deckOfHeroes.Add(hero);
    }

    public void removeHero(Unit hero)
    {
        deckOfHeroes.Remove(hero);
    }

    public List<Upgrade> getUpgrades()
    {
        List<Upgrade> upgrades = new();
        foreach(var upgrade in deckOfUpgrades)
        {
            if(upgrade.active == true)
            {
                upgrades.Add(upgrade.data);
            }
        }
        return upgrades;
    }

    public void deactiveUpgrade(Upgrade upgrade)
    {
        foreach(var u in deckOfUpgrades)
        {
            if(u.data == upgrade)
            {
                u.active = false;
                return;
            }
        }
    }

    public void activateUpgrade(Upgrade upgrade)
    {
        foreach (var u in deckOfUpgrades)
        {
            if (u.data == upgrade)
            {
                u.active = true;
                return;
            }
        }
    }

    public void changeState(Upgrade upgrade)
    {
        foreach (var u in deckOfUpgrades)
        {
            if (u.data == upgrade)
            {
                u.active = !u.active;
                return;
            }
        }
    }
}
