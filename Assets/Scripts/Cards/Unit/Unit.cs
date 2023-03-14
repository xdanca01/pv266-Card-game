using System;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private string cardName = "Name";
    public string Name
    {
        get
        {
            return cardName;
        }
        set
        {
            cardName = value;
            visual.Name.text = value;
        }
    }

    [SerializeField]
    [Range(1, 99)]
    private uint hp = 30;
    public uint HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            visual.HP.text = hp.ToString();
        }
    }

    [SerializeField]
    private Upgrade[] upgrades;
    public Upgrade[] Upgrades => upgrades;

    [SerializeField]
    private Ability[] abilities;
    public Ability[] Abilities => abilities;

    [Serializable]
    public struct Visual
    {
        public TextMeshProUGUI Name;
        public TextMeshProUGUI HP;
        public UnitUpgrades Upgrades; 
        public UnitAbilities Abilities;
    }

    [SerializeField]
    public Visual visual;

    void OnValidate()
    {
        Name = cardName;
        HP = hp;
        visual.Abilities.OnValidate();
        visual.Upgrades.OnValidate();
    }

}
