using System;
using UnityEngine;

public class Upgrade : IUpgrade, Interactable
{
    public Card.Creator Card { get; }
    public Card.Creator.Icon Icon { get; }
    public IEffect Effect { get; }
    public Func<GameObject, Upgrade> FreshCopy;

    public Upgrade(GameObject parent, string title, string description, string iconTitle, string iconDescription, string icon, FSColor color)
    {
        Card = new Card.Creator(title, parent)
            .Background()
            .MiddleTitle()
            .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Icons", icon, color)
            .Description(description, FSFont.Dumbledor);
        Icon = new Card.Creator.Icon(Card, title, iconTitle, iconDescription, icon, color);
        Effect = new Poison(); // TODO!
        FreshCopy = (GameObject parent) => new Upgrade(parent, title, description, iconTitle, iconDescription, icon, color);
    }
}

