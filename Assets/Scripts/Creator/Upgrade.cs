using UnityEngine;

class Upgrade : IUpgrade, Interactable
{
    public Card.Creator Card { get; }
    public Card.Creator.Icon Icon { get; }
    public IEffect Effect { get; }
    public Upgrade(GameObject parent, string title, string description, string artwork, string icon, IEffect effect)
    {
        Card = new Card.Creator(title, parent)
            .Background()
            .MiddleTitle()
            .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Artwork", artwork, FSColor.White)
            .Description(description, FSFont.Dumbledor);
        Icon = new Card.Creator.Icon(Card, "Poison", "Hit", "Poison", icon, FSColor.Blue);
        Effect = effect;
    }
}

