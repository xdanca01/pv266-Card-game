using System;
using UnityEngine;

public class Upgrade : Card, IUpgrade
{
    [Serializable]
    public class Visual
    {
        public string title;
        public uint value;
        public string description;
        public Sprite artwork;
    }

    [SerializeField]
    public Visual visual;


    [Serializable]
    public struct Icon
    {
        public string mainText;
        public string additionalText;
        public Sprite badge;
    }

    [SerializeField]
    public Icon icon;


    public Effect effect;

    public IEffect Effect => effect;
}
