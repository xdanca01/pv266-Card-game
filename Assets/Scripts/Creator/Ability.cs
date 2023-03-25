using UnityEngine;

public class Ability : IAbility, Interactable
{
    public Card.Creator Card { get; }
    public Card.Creator.Icon Icon { get; }

    private uint percentage;
    public uint Percentage { get => percentage; set {
            percentage = value;
            UpdateDescription();
        }
    }

    private uint low;
    public uint Low { get => low; set {
            low = value;
            UpdateDescription();
        }
    }

    private uint high;
    public uint High { get => high; set {
            high = value;
            UpdateDescription();
        }
    }

    public AbilityType Type { get; }

    private void UpdateDescription()
    {
        var percentage = Percentage + "%";
        var range = Low + "-" + High;
        Card.Description(percentage + " " + range + "\n" + Type.ToShortString(), FSFont.DeadRevolution);
        Icon.Title = percentage;
        Icon.Description = range;
    }

    public void RemoveEffects() => throw new System.NotImplementedException();
        
    public Ability(GameObject parent, string title, AbilityType type, uint percentage, uint low, uint high, string spriteName)
    {
        Card = new Card.Creator(title, parent)
            .Background()
            .MiddleTitle()
            .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Icons", spriteName, type.ToFSColor());
        this.percentage = percentage;
        this.low = low;
        this.high = high;
        this.Type = type;
        Icon = new Card.Creator.Icon(Card, title, "", "", spriteName, type.ToFSColor());
        UpdateDescription();
    }
}

