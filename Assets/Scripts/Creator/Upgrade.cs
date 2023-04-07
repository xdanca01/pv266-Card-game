using System;
using Unity.VisualScripting;
using UnityEngine;

public class Upgrade : MonoBehaviour, IUpgrade, Interactable
{
    public Creator Card { get; private set; }
    public Icon Icon { get; private set; }
    public IEffect Effect { get; private set; }
    public Func<GameObject, Upgrade> FreshCopy { get; private set; }

    public static Upgrade New(GameObject parent, string title, string description, string iconTitle, string iconDescription, string icon, FSColor color)
    {
        var Card = new Creator(title, parent)
            .Background()
            .MiddleTitle()
            .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Icons", icon, color)
            .Description(description, FSFont.Dumbledor);
        var upgrade = Card.gameobject.GetOrAddComponent<Upgrade>();
        upgrade.Card = Card;
        upgrade.Icon = Icon.New(Card, Card.gameobject, iconTitle, iconDescription, icon, color);
        upgrade.Effect = new Poison(); // TODO!
        upgrade.FreshCopy = (GameObject parent) => New(parent, title, description, iconTitle, iconDescription, icon, color);
        return upgrade;
    }
}

