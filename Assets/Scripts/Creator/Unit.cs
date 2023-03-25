using System.Collections.ObjectModel;
using System.Linq;
using System;
using UnityEngine;
using Unity.VisualScripting;

public class Unit : MonoBehaviour, IUnit
{
    public Creator Card { get; private set; }
    public Badge badge { get; private set; }
    public SlotDrawer abilities { get; private set; }
    public SlotDrawer upgrades { get; private set; }

    public uint MAX_HP { get; private set; }
    public uint HP { get => badge.Count; set => badge.Count = value; }

    public ReadOnlyCollection<IAbility> Abilities => new(abilities.GetAll<Ability>().Select(i => (IAbility)i).ToList());

    public void RemoveEffects()
    {
        abilities.GetAll<Ability>().ForEach(a => a.RemoveEffects());
    }

    public Func<GameObject, Unit> FreshCopy;

    public static Unit New(GameObject parent, string title, uint hp, 
        Ability firstAbility, Ability secondAbility, Ability thirdAbility,
        Upgrade firstUpgrade, Upgrade secondUpgrade, string artwork)
    {
        var Card = new Creator(title, parent)
            .Background()
            .LeftTitle()
            .MaskedImage("Artwork", new Rect(-1, 0.4f, 4, 5), "Artwork", artwork, FSColor.White);
        var unit = Card.gameobject.AddComponent<Unit>();
        unit.Card = Card;
        unit.badge = Badge.New(Card, hp, FSColor.Red);
        unit.HP = hp;
        unit.MAX_HP = hp;
        unit.abilities = SlotDrawer.New(Card, "Abilities", 3, true, new Vector2(-2, -3.25f));
        unit.abilities.Set(0, firstAbility);
        unit.abilities.Set(1, secondAbility);
        unit.abilities.Set(2, thirdAbility);
        unit.upgrades = SlotDrawer.New(Card, "Upgrades", 2, false, new Vector2(2, -1));
        unit.upgrades.Set(0, firstUpgrade);
        unit.upgrades.Set(1, secondUpgrade);
        unit.FreshCopy = (GameObject parent) => Unit.New(parent, title, hp, firstAbility, secondAbility, thirdAbility, firstUpgrade, secondUpgrade, artwork);
        return unit;
    }
}

