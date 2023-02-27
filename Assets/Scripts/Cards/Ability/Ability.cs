using System;
using UnityEngine;
using TMPro;

public class Ability : MonoBehaviour, IAbility
{
    public string Name = "Name";

    [SerializeField]
    public AbilityType _type = AbilityType.FastAttack;
    public AbilityType Type => _type;

    [SerializeField]
    [Range(0, 100)]
    private uint _percentage = 80;
    public uint Percentage { get => _percentage; set => _percentage = value; }

    [SerializeField]
    [Range(1, 99)]
    private uint _low = 6;
    public uint Low { get => _low; set => _low = value; }

    [SerializeField]
    [Range(1, 99)]
    private uint _high = 9;
    public uint High { get => _high; set => _high = value; }

    public AbilityCard Card;
    public Sprite Artwork;

    public AbilityIcon Icon;
    public Sprite Badge;

    public string GetRangeString()
    {
        return _low + "-" + _high;
    }
    public string GetPercentageString()
    {
        return _percentage + "%";
    }

    public IAbility MakeFreshCopy()
    {
        throw new NotImplementedException();
    }

    public void OnValidate()
    {
        if (Card != null)
        {
            Card.OnValidate();
        }
        if (Icon != null)
        {
            Icon.OnValidate();
        }
    }
}
