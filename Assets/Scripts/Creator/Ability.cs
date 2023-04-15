using Unity.VisualScripting;
using UnityEngine;

public class Ability : MonoBehaviour, IAbility, Interactable
{
    public Creator Card { get; private set; }
    public Icon Icon { get; private set; }
    public Background Background { get; private set; }

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

    public AbilityType Type { get; private set; }


    private void UpdateDescription()
    {
        var percentage = Percentage + "%";
        var range = Low + "-" + High;
        Card.Description(percentage + " " + range + "\n" + Type.ToShortString(), FSFont.DeadRevolution);
        Icon.Title = percentage;
        Icon.Description = range;
    }

    public void RemoveEffects() => throw new System.NotImplementedException();
        
    public static Ability New(GameObject parent, string title, AbilityType type, uint percentage, uint low, uint high, string spriteName)
    {
        var Card = new Creator(title, parent)
            .MiddleTitle()
            .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Icons", spriteName, type.ToFSColor());
        var ability = Card.gameobject.GetOrAddComponent<Ability>();      
        ability.Card = Card;
        ability.Background = Background.New(Card);
        ability.percentage = percentage;
        ability.low = low;
        ability.high = high;
        ability.Type = type;
        ability.Icon = Icon.New(Card, Card.gameobject, "", "", spriteName, type.ToFSColor());
        ability.Icon.gameObject.SetActive(false);
        ability.UpdateDescription();
        return ability;
    }
}

