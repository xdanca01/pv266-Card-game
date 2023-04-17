using System.Collections.ObjectModel;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IUnit, IPointerEnterHandler, IPointerExitHandler
{
    public Creator Card { get; private set; }
    public Badge badge { get; private set; }
    public AbilityDrawer abilities { get; private set; }
    public UpgradeDrawer upgrades { get; private set; }
    public EffectsDrawer effects { get; private set; }
    public Background Background { get; private set; }
    public uint MAX_HP { get; private set; }
    public uint HP { get => badge.Count; set => badge.Count = value; }

    public ReadOnlyCollection<IAbility> Abilities => new(abilities.GetAll().Select(i => (IAbility)i).ToList());

    public void RemoveEffects()
    {
        abilities.GetAll().ForEach(a => a.RemoveEffects());
    }

    public Func<GameObject, Unit> FreshCopy;

    public static Unit New(GameObject parent, string title, uint hp, 
        Ability firstAbility, Ability secondAbility, Ability thirdAbility,
        Upgrade firstUpgrade, Upgrade secondUpgrade, string artwork)
    {
        var Card = new Creator(title, parent)
            .LeftTitle()
            .MaskedImage("Artwork", new Rect(-1, 0.4f, 4, 5), "Artwork", artwork, FSColor.White);
        var unit = Card.gameobject.AddComponent<Unit>();
        unit.Card = Card;
        unit.Background = Background.New(Card);
        unit.badge = Badge.New(Card, hp, FSColor.Red);
        unit.HP = hp;
        unit.MAX_HP = hp;
        unit.abilities = AbilityDrawer.New(Card);
        unit.abilities.Set(0, firstAbility);
        unit.abilities.Set(1, secondAbility);
        unit.abilities.Set(2, thirdAbility);
        unit.upgrades = UpgradeDrawer.New(Card);
        unit.upgrades.Set(0, firstUpgrade);
        unit.upgrades.Set(1, secondUpgrade);
        unit.effects = EffectsDrawer.New(Card);
        unit.effects.Hide();
        unit.FreshCopy = (GameObject parent) => Unit.New(parent, title, hp, firstAbility, secondAbility, thirdAbility, firstUpgrade, secondUpgrade, artwork);
        return unit;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!this.effects.IsCompletelyEmpty())
        {
            this.effects.Show();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.effects.Hide();
    }

    public void ApplyEffect(Upgrade effect)
    {
        this.effects.Add(effect);
    }
    public void RemoveEffect(Upgrade effect)
    {
        this.effects.Remove(effect);
    }
}