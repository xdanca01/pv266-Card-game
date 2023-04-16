using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Trader : MonoBehaviour
{
    public static Trader instance { get; private set; }
    [SerializeField] public GameObject BuyGrid;
    [SerializeField] public GameObject PreviewGrid;
    [SerializeField] public GameObject SellGrid;
    [SerializeField] public TextMeshProUGUI Coins;
    [SerializeField] public GameObject SellButton;
    [SerializeField] public GameObject BuyButton;
    [SerializeField] public GameObject ShopItemPrefab;
    [SerializeField] public Generator generator;

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

    List<Deck.HeroData> deckOfHeroes = new();
    List<Deck.UpgradeData> deckOfUpgrades = new();
    public void Generate()
    {
        CreateItemsInShop();
        GenerateShop();
        GenerateInventory();
    }

    private void CreateItemsInShop()
    {
        deckOfHeroes = new();
        deckOfUpgrades = new();
        for (int i = 0; i < 5; ++i)
        {
            //Generate heroes
            int index = UnityEngine.Random.Range(0, Deck.instance.heroes.Count);
            int cnt = 0;
            foreach (var h in Deck.instance.heroes)
            {
                if (cnt == index)
                {
                    deckOfHeroes.Add(new Deck.HeroData(h.Value, Deck.instance.LastAvailableID++, h.Key));
                    break;
                }
                ++cnt;
            }
            //Generate upgrades
            index = UnityEngine.Random.Range(0, Deck.instance.upgrades.Count);
            cnt = 0;
            foreach (var u in Deck.instance.upgrades)
            {
                if (cnt == index)
                {
                    deckOfUpgrades.Add(new Deck.UpgradeData(u.Value, Deck.instance.LastAvailableID++));
                    break;
                }
                ++cnt;
            }
        }
    }

    private void GenerateShop()
    {
        //Delete old inv
        foreach (Transform child in BuyGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var upgrade in deckOfUpgrades)
        {
            GameObject u = Instantiate(ShopItemPrefab, BuyGrid.transform);
            Button button = u.GetComponent<Button>();
            ShopItem data = u.GetComponent<ShopItem>();
            data.InitItem(upgrade.data.name, null, "10");
            button.onClick.AddListener(delegate () { PreviewUpgrade(upgrade, false); });
        }
        foreach (var hero in deckOfHeroes)
        {
            GameObject u = Instantiate(ShopItemPrefab, BuyGrid.transform);
            Button button = u.GetComponent<Button>();
            ShopItem data = u.GetComponent<ShopItem>();
            data.InitItem(hero.name, hero.data.MAX_HP.ToString(), "30");
            button.onClick.AddListener(delegate () { PreviewHero(hero, false); });
        }
    }

    private void GenerateInventory()
    {
        //Delete old inv
        foreach (Transform child in SellGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (var upgrade in Deck.instance.deckOfUpgrades)
        {
            GameObject u = Instantiate(ShopItemPrefab, SellGrid.transform);
            Button button = u.GetComponent<Button>();
            ShopItem data = u.GetComponent<ShopItem>();
            data.InitItem(upgrade.data.name, null, "5");
            button.onClick.AddListener(delegate () { PreviewUpgrade(upgrade, true); });
        }
        foreach (var hero in Deck.instance.deckOfHeroes)
        {
            GameObject u = Instantiate(ShopItemPrefab, SellGrid.transform);
            Button button = u.GetComponent<Button>();
            ShopItem data = u.GetComponent<ShopItem>();
            data.InitItem(hero.name, hero.data.MAX_HP.ToString(), "15");
            button.onClick.AddListener(delegate () { PreviewHero(hero, true); });
        }
    }

    private void PreviewUpgrade(Deck.UpgradeData upgrade, bool sell)
    {
        foreach (Transform child in PreviewGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject upgradeObject = generator.GetUpgrade(upgrade.data.name);
        GameObject obj = Instantiate(upgradeObject, PreviewGrid.transform);
        obj.transform.localScale = new Vector3(30.0f, 30.0f, 30.0f);
        if (sell == true)
        {
            SellButton.SetActive(true);
            BuyButton.SetActive(false);
        }
        else
        {
            SellButton.SetActive(false);
            BuyButton.SetActive(true);
        }
    }

    private void PreviewHero(Deck.HeroData hero, bool sell)
    {
        foreach (Transform child in PreviewGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject heroObject = generator.GetHero(hero.name);
        GameObject obj = Instantiate(heroObject, PreviewGrid.transform);
        obj.transform.localScale = new Vector3(30.0f, 30.0f, 30.0f);
        if (sell == true)
        {
            SellButton.SetActive(true);
            BuyButton.SetActive(false);
        }
        else
        {
            SellButton.SetActive(false);
            BuyButton.SetActive(true);
        }
    }
}
