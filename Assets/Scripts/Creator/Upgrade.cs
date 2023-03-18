using UnityEngine;

class Upgrade : IUpgrade, Interactable
{
    public Card.Creator Card { get; }
    public Card.Creator.Icon Icon { get; }
    public IEffect Effect { get; }
    public Upgrade(GameObject parent, IEffect effect)
    {
        Card = new Card.Creator("Poison", parent)
            .Background()
            .MiddleTitle()
            .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Artwork", "Potion Making", FSColor.White, 1.0f)
            .Description("Creature you hit gets poisoned. It takes 8 damage each round.", FSFont.Dumbledor);
        Icon = new Card.Creator.Icon(Card, "Poison", "Hit", "Poison", "erlenmeyer", FSColor.Blue);
        Effect = effect;
    }
}

