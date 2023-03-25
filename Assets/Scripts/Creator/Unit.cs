using System.Collections.ObjectModel;
using System.Linq;
using System;
using UnityEngine;

public class Unit : IUnit
{
    public readonly Card.Creator Card;
    private readonly Card.Creator.Badge hp;
    public readonly Card.Creator.SlotDrawer<Ability> abilities;
    public readonly Card.Creator.SlotDrawer<Upgrade> upgrades;

    public readonly uint MAX_HP;
    public uint HP { get => hp.Count; set => hp.Count = value; }

    public ReadOnlyCollection<IAbility> Abilities => new(abilities.GetAll().Select(i => (IAbility)i).ToList());

    public void RemoveEffects()
    {
        abilities.GetAll().ForEach(a => a.RemoveEffects());
    }

    public Func<GameObject, Unit> FreshCopy;

    public Unit(GameObject parent, string title, uint hp, 
        Ability firstAbility, Ability secondAbility, Ability thirdAbility,
        Upgrade firstUpgrade, Upgrade secondUpgrade, string artwork)
    {
        Card = new Card.Creator(title, parent)
            .Background()
            .LeftTitle()
            .MaskedImage("Artwork", new Rect(-1, 0.4f, 4, 5), "Artwork", artwork, FSColor.White);
        this.hp = new Card.Creator.Badge(Card, hp, FSColor.Red);
        HP = MAX_HP = hp;
        abilities = new Card.Creator.SlotDrawer<Ability>(Card, "Abilities", 3, true, new Vector2(-2, -3.25f));
        abilities.Set(0, firstAbility);
        abilities.Set(1, secondAbility);
        abilities.Set(2, thirdAbility);
        upgrades = new Card.Creator.SlotDrawer<Upgrade>(Card, "Upgrades", 2, false, new Vector2(2, -1));
        upgrades.Set(0, firstUpgrade);
        upgrades.Set(1, secondUpgrade);
        FreshCopy = (GameObject parent) => new Unit(parent, title, hp, firstAbility, secondAbility, thirdAbility, firstUpgrade, secondUpgrade, artwork);
    }
}

