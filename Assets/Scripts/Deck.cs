using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public static Deck instance { get; private set; }

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

    [SerializeField] public GameObject UpgradesGrid;
    [SerializeField] public GameObject PreviewGrid;
    [SerializeField] public GameObject HeroesGrid;

    [SerializeField] public GameObject UpgradePrefab;
    [SerializeField] public GameObject HeroPrefab;

    [SerializeField] public GameObject DisableButton;
    [SerializeField] public GameObject EnableButton;
    [SerializeField] public Color DisabledColor;

    private bool _generated = false;

    public Dictionary<string, Upgrade> upgrades = new();
    public Dictionary<string, Unit> heroes = new();

    public int coins = 0;

    [SerializeField] public Generator generator;
    public int LastAvailableID = 1;
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
    public class HeroData
    {
        public bool active = true;
        public Unit data;
        public int ID;
        public string name;
        public HeroData(Unit unitData, int id, string name, bool state = true)
        {
            this.active = state;
            this.data = unitData;
            this.name = name;
            ID = id;
        }
    }

    public List<HeroData> deckOfHeroes = new();
    public List<UpgradeData> deckOfUpgrades = new();

    private void Update()
    {
        if(upgrades.Keys.Count > 0 && heroes.Keys.Count > 0 && _generated == false)
        {
            _generated = true;
            LoadData();
        }
    }

    private void LoadData()
    {
        deckOfUpgrades = new();
        foreach (var upgrade in upgrades)
        {
            if (possibleUpgrade(upgrade.Value) == true)
            {
                UpgradeData data = new UpgradeData(upgrade.Value, LastAvailableID++, true);
                deckOfUpgrades.Add(data);
            }
        };
        deckOfHeroes = new();
        List<string> heroNames = new() { "Warrior", "Mage", "Shaman" };
        foreach (var hero in heroes)
        {
            if(possibleHero(hero.Value) == true && heroNames.Contains(hero.Key))
            {
                HeroData data = new HeroData(hero.Value, LastAvailableID++, hero.Key, true);
                deckOfHeroes.Add(data);
            }
        }
        List<string> upgradeNames = new() { "Poison", "Headshot" };
    }

    public List<HeroData> getHeroes()
    {
        return deckOfHeroes;
    }

    public void addHero(Unit hero, string name)
    {
        HeroData H = new(hero, LastAvailableID++, name, true);
        deckOfHeroes.Add(H);
    }

    public void removeHero(HeroData hero)
    {
        foreach (var u in deckOfHeroes)
        {
            if (u.ID == hero.ID)
            {
                deckOfHeroes.Remove(hero);
                return;
            }
        }
    }

    public void addUpgrade(UpgradeData upgrade)
    {
        UpgradeData U = new(upgrade.data, LastAvailableID++, true);
        deckOfUpgrades.Add(U);
    }

    public void addUpgrade(Upgrade upgrade)
    {
        UpgradeData U = new(upgrade, LastAvailableID++, true);
        deckOfUpgrades.Add(U);
    }

    public void removeUpgrade(UpgradeData upgrade)
    {
        foreach (var u in deckOfUpgrades)
        {
            if (u.ID == upgrade.ID)
            {
                deckOfUpgrades.Remove(upgrade);
                return;
            }
        }
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
        //deckOfHeroes.Clear();
        //deckOfUpgrades.Clear();
        for (int i = 0; i < 3; ++i)
        {
            //deckOfUpgrades.Add(new UpgradeData(new Upgrade(), LastAvailableID++));
            //deckOfHeroes.Add(new Unit());
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
                UpgradeButton data = u.GetComponent<UpgradeButton>();
                data.SetPrefabData(upgrade.data.name);
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
            UpgradeButton data = u.GetComponent<UpgradeButton>();
            data.SetPrefabData(upgrade.data.name);
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
            GameObject u = Instantiate(HeroPrefab, HeroesGrid.transform);
            Button button = u.GetComponent<Button>();
            HeroButton data = u.GetComponent<HeroButton>();
            data.SetPrefabData(hero.name, hero.data.MAX_HP.ToString());
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
        GameObject upgradeObject = generator.GetUpgrade(upgrade.data.name);
        foreach(Transform child in PreviewGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject obj = Instantiate(upgradeObject, PreviewGrid.transform);
        obj.transform.localScale = new Vector3(30.0f, 30.0f, 30.0f);
        button.onClick.AddListener(delegate () { GenerateUpgrades(); });
        Debug.Log("Upgrade: " + upgrade);
    }

    public void PreviewHero(HeroData hero)
    {
        DisableButton.SetActive(false);
        EnableButton.SetActive(false);
        GameObject heroObject = generator.GetHero(hero.name);
        foreach (Transform child in PreviewGrid.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject obj = Instantiate(heroObject, PreviewGrid.transform);
        obj.GetComponent<GraphicRaycaster>().enabled = false;
        obj.transform.localScale = new Vector3(30.0f, 30.0f, 30.0f);
        Debug.Log("Hero: " + hero);
    }
    
    public bool possibleHero(Unit hero)
    {
        List<string> names = new List<string> { "Warrior", "Archer", "Mage", "Barbarian", "Rogue", "Shaman" };
        if(names.Contains(hero.name))
        {
            return true;
        }
        return false;
    }

    public bool possibleUpgrade(Upgrade upgrade)
    {
        List<string> names = new List<string> { "Double Attack", "Healing spring", "Poison" };
        if (names.Contains(upgrade.name))
        {
            return true;
        }
        return false;
    }
}
