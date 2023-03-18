using UnityEngine;

class ConstructedAbility : IAbility, Interactable
{
    public Card.Creator Card { get; }
    public Card.Creator.Icon Icon { get; }

    public uint Percentage { get => Percentage; set => throw new System.NotImplementedException(); }
    public uint Low { get => Low; set => throw new System.NotImplementedException(); }
    public uint High { get => High; set => throw new System.NotImplementedException(); }

    public AbilityType Type { get; }

    public void RemoveEffects() => throw new System.NotImplementedException();
        
    public ConstructedAbility(GameObject parent, uint percentage, uint low, uint high, AbilityType type)
    {
        Card = new Card.Creator("Elven Sword", parent)
            .Background()
            .MiddleTitle()
            .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Icons", "broadsword", FSColor.Yellow, 1.0f)
            .Description("80% 6-9\nLIGHT DMG", FSFont.DeadRevolution);
        Icon = new Card.Creator.Icon(Card, "Elven Sword", "80%", "6-9", "broadsword", FSColor.Yellow);
    }
}

