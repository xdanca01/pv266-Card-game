using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    [SerializeField] public GameObject UpgradesGrid;
    [SerializeField] public GameObject HeroesGrid;
    [SerializeField] public GameObject UpgradePrefab;
    [SerializeField] public GameObject HeroPrefab;
    [SerializeField] public GameObject DisableButton;
    [SerializeField] public GameObject EnableButton;
    [SerializeField] public Color DisabledColor;
    int LastAvailableID = 1;
    public class UpgradeData
    {
        public bool active = false;
        public Upgrade data;
        public int ID;
        public UpgradeData(Upgrade upgradeData, int id, bool state = true)
        {
            this.active = state;
            this.data = upgradeData;
            ID = id;
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

    public void deactiveUpgrade(UpgradeData upgrade)
    {
        foreach(var u in deckOfUpgrades)
        {
            if(u.ID == upgrade.ID)
            {
                u.active = false;
                return;
            }
        }
    }

    public void activateUpgrade(UpgradeData upgrade)
    {
        foreach (var u in deckOfUpgrades)
        {
            if (u.ID == upgrade.ID)
            {
                u.active = true;
                return;
            }
        }
    }

    public void changeState(UpgradeData upgrade, bool state)
    {
        foreach(var u in deckOfUpgrades)
        {
            if (u.ID == upgrade.ID)
            {
                u.active = state;
                return;
            }
        }
    }

    public void Generate()
    {
        //Test TODO remove this for cycle
        //deckOfUpgrades.Clear();
        deckOfHeroes.Clear();
        for (int i = 0; i < 3; ++i)
        {
            deckOfUpgrades.Add(new UpgradeData(new Upgrade(), LastAvailableID++));
            deckOfHeroes.Add(new Unit());
        }
        DisableButton.SetActive(false);
        GenerateUpgrades();
        GenerateHeroes();
    }

    public void GenerateUpgrades()
    {
        foreach (Transform child in UpgradesGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        List<UpgradeData> Active = new();
        foreach(var upgrade in deckOfUpgrades)
        {
            if(upgrade.active == false)
            {
                GameObject u = Instantiate(UpgradePrefab, UpgradesGrid.transform);
                u.GetComponent<Image>().color = DisabledColor;
                Button button = u.GetComponent<Button>();
                button.onClick.AddListener(delegate() { PreviewUpgrade(upgrade); });
            }
            else
            {
                Active.Add(upgrade);
            }
        }
        foreach(var upgrade in Active)
        {
            GameObject u = Instantiate(UpgradePrefab, UpgradesGrid.transform);
            Button button = u.GetComponent<Button>();
            button.onClick.AddListener(delegate() { PreviewUpgrade(upgrade); });
        }
    }

    public void GenerateHeroes()
    {
        foreach (Transform child in HeroesGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        List<UpgradeData> Active = new();
        foreach (var hero in deckOfHeroes)
        {
            GameObject u = Instantiate(UpgradePrefab, HeroesGrid.transform);
            Button button = u.GetComponent<Button>();
            button.onClick.AddListener(delegate () { PreviewHero(hero); });
        }
    }

    public void PreviewUpgrade(UpgradeData upgrade)
    {
        Button button;
        if (upgrade.active == true)
        {
            DisableButton.SetActive(true);
            EnableButton.SetActive(false);
            button = DisableButton.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate () { changeState(upgrade, false); }); 
        }
        else
        {
            DisableButton.SetActive(false);
            EnableButton.SetActive(true);
            button = EnableButton.GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(delegate () { changeState(upgrade, true); });
        }
        button.onClick.AddListener(delegate () { GenerateUpgrades(); });
        Debug.Log("Upgrade: " + upgrade);
    }

    public void PreviewHero(Unit hero)
    {
        DisableButton.SetActive(false);
        Debug.Log("Upgrade: " + hero);
    }
}
