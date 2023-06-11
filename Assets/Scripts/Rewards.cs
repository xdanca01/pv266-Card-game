using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Extreme
}

public enum RewardType
{
    Upgrade, // armory
    Coins, // swanport
    Hero // sequoia
}

public class Rewards : MonoBehaviour
{
    [SerializeField] RewardsUI RewardsUUI;
    public static Rewards instance { get; private set; }
    private Dictionary<string, Difficulty> difficulties = new Dictionary<string, Difficulty>(){ 
        { "Adinkira 'hene", Difficulty.Easy },
        { "Dono", Difficulty.Hard },
        { "Dwenini aben", Difficulty.Extreme },
        { "Fihankra", Difficulty.Hard },
        { "Kuntinkantan", Difficulty.Hard },
        { "Nsoroma", Difficulty.Normal },
        { "Sankofa", Difficulty.Normal },
        { "Sepow", Difficulty.Extreme }
    }; 

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

    public (RewardType, string) GiveReward(Difficulty difficulty)
    {
        float rewardByChance = UnityEngine.Random.Range(0.0f, 1.0f);
        switch (difficulty)
        {
            case Difficulty.Easy:
                return RewardEasy(rewardByChance);
            case Difficulty.Normal:
                return RewardNormal(rewardByChance);
            case Difficulty.Hard:
                return RewardHard(rewardByChance);
            default:
            case Difficulty.Extreme:
                return RewardExtreme(rewardByChance);
        }
    }
    public (RewardType, string) GiveSomeReward(string Map)
    {
        return GiveReward(difficulties[Map]);
    }
    private (RewardType, string) GiveHero()
    {
        string Rew = "";
        Dictionary<string, Unit> heroes = filterHeroes(Deck.instance.heroes);
        int index = UnityEngine.Random.Range(0, heroes.Count);
        int cnt = 0;
        foreach(var h in heroes)
        {
            if(cnt == index)
            {
                Deck.instance.addHero(h.Value, h.Key);
                Debug.Log("A "+ h.Key + " joins the battle!");
                Rew = "Hero: " + h.Key;
                break;
            }
            ++cnt;
        }
        return (RewardType.Hero, Rew);
    }

    private Dictionary<string, Upgrade> filterUpgrades(Dictionary<string, Upgrade> list)
    {
        Dictionary<string, Upgrade> l = new();
        foreach (var h in list)
        {
            if (Deck.instance.possibleUpgrade(h.Value))
            {
                l.Add(h.Key, h.Value);
            }
        }
        return l;
    }

    private Dictionary<string, Unit> filterHeroes(Dictionary<string, Unit> list)
    {
        Dictionary<string, Unit> l = new();
        foreach (var h in list)
        {
            if (Deck.instance.possibleHero(h.Value))
            {
                l.Add(h.Key, h.Value);
            }
        }
        return l;
    }



    private (RewardType, string) RewardEasy(float chance)
    {
        //40% Upgrade
        if(chance <= 0.4f)
        {
            return GiveUpgrade(1, 1);
            
        }
        //60%
        return GiveCoins(3, 5);
    }

    private (RewardType, string) RewardNormal(float chance)
    {
        string Rew;
        //25% Ability
        if (chance <= 0.25f)
        {
            return GiveUpgrade(1, 1);
        }
        //25% Upgrade
        else if (chance <= 0.50f)
        {
            return GiveUpgrade(1, 1);
        }
        //50% coins
        else
        {
            return GiveCoins(5, 10);
        }
    }

    private (RewardType, string) RewardHard(float chance)
    {
        //30% Ability
        if (chance <= 0.3f)
        {
            return GiveAbility(1, 2);
        }
        //30% Upgrade
        else if (chance <= 0.60f)
        {
            return GiveUpgrade(1, 2);
        }
        //40% coins
        else
        {
            return GiveCoins(10, 15);
        }
    }

    private (RewardType, string) RewardExtreme(float chance)
    {
        //30% Ability
        if (chance <= 0.3f)
        {
            return GiveUpgrade(1, 3);
        }
        //10% Hero
        if (chance <= 0.40f)
        {
            return GiveHero();
        }
        //30% Upgrade
        if (chance <= 0.70f)
        {
            return GiveUpgrade(1, 2);
        }
        //30% coins
        return GiveCoins(20, 25);
    }

    private (RewardType, string) GiveCoins(int min, int max)
    {
        int coins = UnityEngine.Random.Range(min, max);
        Debug.Log("You recieved " + coins + " coins!");
        Deck.instance.coins += coins;
        return (RewardType.Coins, coins + " coins");
    }

    private (RewardType, string) GiveAbility(int min, int max)
    {
        //TODO
        return GiveUpgrade(min, max);
    }
    private (RewardType, string) GiveUpgrade(int min, int max)
    {
        Dictionary<string, Upgrade> upgrades = filterUpgrades(Deck.instance.upgrades);
        int numberOfUpgrades = UnityEngine.Random.Range(min, max + 1);
        Debug.Log("You recieved " + numberOfUpgrades + " new upgrades!");
        for (int i = 0; i < numberOfUpgrades; ++i)
        {
            int index = UnityEngine.Random.Range(0, upgrades.Count);
            int cnt = 0;
            foreach (var h in upgrades)
            {
                if (cnt == index)
                {
                    Deck.instance.addUpgrade(h.Value);
                    break;
                }
                ++cnt;
            }
        }
        return (RewardType.Upgrade, numberOfUpgrades + " upgrades");
    }

    //For tutorial purposes
    public (RewardType, string) GiveHero(Unit hero)
    {
        Deck.instance.addHero(hero, hero.name);
        return (RewardType.Hero, "hero: " + hero.name);
    }

    //For tutorial purposes
    public (RewardType, string) GiveUpgrade()
    {
        GiveUpgrade(1,1);
        return (RewardType.Upgrade, "upgrade");
    }


    public (RewardType, string) GiveTenCoins()
    {
        GiveCoins(10, 11);
        return (RewardType.Coins, "10 coins");
    }
}
