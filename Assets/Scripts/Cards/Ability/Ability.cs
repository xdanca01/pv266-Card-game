using System;
using UnityEngine;
using TMPro;

public class Ability : Card, IAbility
{
    [Serializable]
    public struct Visual
    {
        public string title;
        public uint value;
        public Sprite artwork;
    }

    public Visual visual;

    [Serializable]
    public struct Icon
    {
        public Sprite badge;
    }

    public Icon icon;


    [SerializeField]
    private AbilityType type = AbilityType.FastAttack;
    public AbilityType Type => type;

    [SerializeField]
    [Range(0, 100)]
    private uint percentage = 80;
    public uint Percentage { get => percentage; set => percentage = value; }

    [SerializeField]
    [Range(1, 99)]
    private uint low = 6;
    public uint Low { get => low; set => low = value; }

    [SerializeField]
    [Range(1, 99)]
    private uint high = 9;
    public uint High { get => high; set => high = value; }

    public string GetRangeString()
    {
        return low + "-" + high;
    }
    public string GetPercentageString()
    {
        return percentage + "%";
    }

    public IAbility MakeFreshCopy()
    {
        throw new NotImplementedException();
    }
}
