using UnityEngine;

class Ability : IAbility, Interactable
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
        Card.Description(percentage + " " + range + "\n" + Type.ToString(), FSFont.DeadRevolution);
        Icon.Title = percentage;
        Icon.Description = range;
    }

    public void RemoveEffects() => throw new System.NotImplementedException();
        
    public Ability(GameObject parent, uint percentage, uint low, uint high, AbilityType type)
    {
        Card = new Card.Creator("Elven Sword", parent)
            .Background()
            .MiddleTitle()
            .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Icons", "broadsword", type.ToFSColor());
        this.percentage = percentage;
        this.low = low;
        this.high = high;
        this.Type = type;
        Icon = new Card.Creator.Icon(Card, "Elven Sword", "", "", "broadsword", type.ToFSColor());
        UpdateDescription();
    }
}

