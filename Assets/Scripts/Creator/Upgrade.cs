using System;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class Upgrade : MonoBehaviour, IUpgrade, Interactable
{
    public Creator Card { get; private set; }
    public Icon Icon { get; private set; }
    public Background Background { get; private set; }
    public IEffect Effect { get; private set; }
    public Func<GameObject, Upgrade> FreshCopy { get; private set; }

    public FSColor Color { get; private set; }

    public static Upgrade New(GameObject parent, string title, string description, string iconTitle, string iconDescription, string icon, FSColor color)
    {
        var Card = new Creator(title, parent)
            .MiddleTitle()
            .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Icons", icon, color)
            .Description(description, FSFont.Dumbledor);
        var upgrade = Card.gameobject.GetOrAddComponent<Upgrade>();
        upgrade.Card = Card;
        upgrade.Background = Background.New(Card);
        upgrade.Icon = Icon.New(Card, Card.gameobject, iconTitle, iconDescription, icon, color);
        upgrade.Color = color;
        upgrade.Effect = null;        
        foreach (var effect in new IEffect[] { new DoubleAttack(), new HealingSpring(), new Poisoned(), new Poison() })
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in effect.GetType().ToString())
            {
                if (c == char.ToUpper(c))
                {
                    builder.Append(' ');
                }
                builder.Append(c);
            }
            if (title.Trim().ToLower().Equals(builder.ToString().Trim().ToLower()))
            {
                upgrade.Effect = effect;
                break;
            }
        }
        if (upgrade.Effect == null)
        {
            Debug.LogWarning("Assigning NoEffect to " + title);
            upgrade.Effect = new NoEffect();
        }
        upgrade.FreshCopy = (GameObject parent) => New(parent, title, description, iconTitle, iconDescription, icon, color);
        upgrade.Icon.gameObject.SetActive(false);
        return upgrade;
    }
}

