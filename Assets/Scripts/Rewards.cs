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
public class Rewards : MonoBehaviour
{
    public static Rewards instance { get; private set; }

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GiveReward(Difficulty difficulty)
    {
        float rewardByChance = UnityEngine.Random.Range(0.0f, 1.0f);
        switch (difficulty)
        {
            case Difficulty.Easy:
                RewardEasy(rewardByChance);
                break;
            case Difficulty.Normal:
                RewardNormal(rewardByChance);
                break;
            case Difficulty.Hard:
                RewardHard(rewardByChance);
                break;
            case Difficulty.Extreme:
                RewardExtreme(rewardByChance);
                break;
        }
    }
    private void GiveHero()
    {
        Dictionary<string, Unit> heroes = filterHeroes(Deck.instance.heroes);
        int index = UnityEngine.Random.Range(0, heroes.Count);
        int cnt = 0;
        foreach(var h in heroes)
        {
            if(cnt == index)
            {
                Deck.instance.addHero(h.Value, h.Key);
                Debug.Log("A "+ h.Key + " joins the battle!");
                break;
            }
            ++cnt;
        }
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

    private void GiveAbility(int min, int max)
    {
        //TODO
        GiveUpgrade(min, max);
    }
    private void GiveUpgrade(int min, int max)
    {
        Dictionary<string, Upgrade> upgrades = filterUpgrades(Deck.instance.upgrades);
        int numberOfUpgrades = UnityEngine.Random.Range(min, max+1);
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
    }

    private void GiveCoins(int min, int max)
    {
        int coins = UnityEngine.Random.Range(min, max);
        Debug.Log("You recieved " + coins + " coins!");
        Deck.instance.coins += coins;
    }

    private void RewardEasy(float chance)
    {
        //40% Upgrade
        if(chance <= 0.4f)
        {
            GiveUpgrade(1, 1);
        }
        //60%
        else
        {
            GiveCoins(3, 5);
        }
    }

    private void RewardNormal(float chance)
    {
        //25% Ability
        if (chance <= 0.25f)
        {
            GiveUpgrade(1, 1);
        }
        //25% Upgrade
        else if (chance <= 0.50f)
        {
            GiveUpgrade(1, 1);
        }
        //50% coins
        else
        {
            GiveCoins(5, 10);
        }
    }

    private void RewardHard(float chance)
    {
        //30% Ability
        if (chance <= 0.3f)
        {
            GiveAbility(1, 2);
        }
        //30% Upgrade
        else if (chance <= 0.60f)
        {
            GiveUpgrade(1, 2);
        }
        //40% coins
        else
        {
            GiveCoins(10, 15);
        }
    }

    private void RewardExtreme(float chance)
    {
        //30% Ability
        if (chance <= 0.3f)
        {
            GiveUpgrade(1, 3);
        }
        //10% Hero
        else if (chance <= 0.40f)
        {
            GiveHero();
        }
        //30% Upgrade
        else if (chance <= 0.70f)
        {
            GiveUpgrade(1, 2);
        }
        //30% coins
        else
        {
            GiveCoins(20, 25);
        }
    }
}
