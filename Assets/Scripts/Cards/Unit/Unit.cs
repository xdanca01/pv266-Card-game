using System;
using TMPro;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private string _name = "Name";
    public string Name
    {
        get
        {
            return _name;
        }
        set
        {
            _name = value;
            visual.Name.text = value;
        }
    }

    [SerializeField]
    [Range(1, 99)]
    private uint _hp = 30;
    public uint HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
            visual.HP.text = _hp.ToString();
        }
    }

    [SerializeField]
    private GameObject[] _upgrades;
    public GameObject[] Upgrades => _upgrades;

    [SerializeField]
    private Ability[] _abilities;
    public Ability[] Abilities => _abilities;

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
        Name = _name;
        HP = _hp;
        visual.Abilities.OnValidate();
        visual.Upgrades.OnValidate();
    }

}
