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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public string GiveReward(Difficulty difficulty)
    {
        string Rew = "";
        float rewardByChance = UnityEngine.Random.Range(0.0f, 1.0f);
        switch (difficulty)
        {
            case Difficulty.Easy:
                Rew = RewardEasy(rewardByChance);
                break;
            case Difficulty.Normal:
                Rew = RewardNormal(rewardByChance);
                break;
            case Difficulty.Hard:
                Rew = RewardHard(rewardByChance);
                break;
            case Difficulty.Extreme:
                Rew = RewardExtreme(rewardByChance);
                break;
        }
        return Rew;
    }
    public string GiveSomeReward(string Map)
    {
        return GiveReward(difficulties[Map]);
    }
    private string GiveHero()
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
        return Rew;
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

    private string GiveAbility(int min, int max)
    {
        //TODO
        return GiveUpgrade(min, max);
    }
    private string GiveUpgrade(int min, int max)
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
        return numberOfUpgrades + " upgrades";
    }

    private string GiveCoins(int min, int max)
    {
        int coins = UnityEngine.Random.Range(min, max);
        Debug.Log("You recieved " + coins + " coins!");
        Deck.instance.coins += coins;
        return coins + " coins";
    }

    private string RewardEasy(float chance)
    {
        string Rew;
        //40% Upgrade
        if(chance <= 0.4f)
        {
            Rew = GiveUpgrade(1, 1);
        }
        //60%
        else
        {
            Rew = GiveCoins(3, 5);
        }
        return Rew;
    }

    private string RewardNormal(float chance)
    {
        string Rew;
        //25% Ability
        if (chance <= 0.25f)
        {
            Rew = GiveUpgrade(1, 1);
        }
        //25% Upgrade
        else if (chance <= 0.50f)
        {
            Rew = GiveUpgrade(1, 1);
        }
        //50% coins
        else
        {
            Rew = GiveCoins(5, 10);
        }
        return Rew;
    }

    private string RewardHard(float chance)
    {
        string Rew;
        //30% Ability
        if (chance <= 0.3f)
        {
            Rew = GiveAbility(1, 2);
        }
        //30% Upgrade
        else if (chance <= 0.60f)
        {
            Rew = GiveUpgrade(1, 2);
        }
        //40% coins
        else
        {
            Rew = GiveCoins(10, 15);
        }
        return Rew;
    }

    private string RewardExtreme(float chance)
    {
        string Rew;
        //30% Ability
        if (chance <= 0.3f)
        {
            Rew = GiveUpgrade(1, 3);
        }
        //10% Hero
        else if (chance <= 0.40f)
        {
            Rew = GiveHero();
        }
        //30% Upgrade
        else if (chance <= 0.70f)
        {
            Rew = GiveUpgrade(1, 2);
        }
        //30% coins
        else
        {
            Rew = GiveCoins(20, 25);
        }
        return Rew;
    }

    //For tutorial purposes
    public string GiveHero(Unit hero)
    {
        Deck.instance.addHero(hero, hero.name);
        return "hero: " + hero;
    }

    //For tutorial purposes
    public void GiveUpgrade()
    {
        Deck.instance.LoadUpgrades();
    }
}
