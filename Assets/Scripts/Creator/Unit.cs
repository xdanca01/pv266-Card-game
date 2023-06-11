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
    public uint MAX_HP { get; set; }
    public uint HP { get => badge.Count; set => badge.Count = value; }

    public ReadOnlyCollection<IAbility> Abilities => new(abilities.GetAll().Select(i => (IAbility)i).ToList());

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
        unit.Background = Background.New(Card, unit.gameObject);
        unit.badge = Badge.New(Card, hp, FSColor.Red);
        unit.HP = hp;
        unit.MAX_HP = hp;
        unit.abilities = AbilityDrawer.New(Card);
        unit.abilities.Set(0, firstAbility != null ? firstAbility.FreshCopy(unit.abilities.gameObject) : null);
        unit.abilities.Set(1, secondAbility != null ? secondAbility.FreshCopy(unit.abilities.gameObject) : null);
        unit.abilities.Set(2, thirdAbility != null ? thirdAbility.FreshCopy(unit.abilities.gameObject) : null);
        unit.upgrades = UpgradeDrawer.New(Card);
        unit.upgrades.Set(0, firstUpgrade != null ? firstUpgrade.FreshCopy(unit.upgrades.gameObject) : null);
        unit.upgrades.Set(1, secondUpgrade != null ? secondUpgrade.FreshCopy(unit.upgrades.gameObject) : null);
        unit.effects = EffectsDrawer.New(Card);
        unit.effects.Hide();
        unit.FreshCopy = (GameObject parent) => Unit.New(parent, title, hp, firstAbility, secondAbility, thirdAbility, firstUpgrade, secondUpgrade, artwork);
        return unit;
    }

    private void RefreshAbilities()
    {
        var abis = abilities.GetAll();
        abilities = AbilityDrawer.New(Card);
        uint cnt = 0;
        foreach(var ability in abis)
        {
            abilities.Set(cnt, ability != null ? ability.FreshCopy(ability.gameObject) : null);
            ++cnt;
        }   
    }

    public void ScaleAbilities(float scaleFactor)
    {
        foreach(var ability in abilities.GetAll())
        {
            ability.Low = (uint)(ability.Low * scaleFactor);
            ability.High = (uint)(ability.High * scaleFactor);
        }
        RefreshAbilities();
    }

    public bool HasEffect(EffectType effect)
    {
        return this.effects.GetAll().Concat(this.upgrades.GetAll()).Any(e => e.Effect.Type == effect);
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
    public void AddUpgrade(Upgrade upgrade)
    {
        this.upgrades.Add(upgrade);
        upgrade.Effect.Once(this);
    }
    public void ApplyEffect(Upgrade effect)
    {   
        if (!this.effects.GetAll().Any(e => e.Effect.Type == effect.Effect.Type))
        {
            this.effects.Add(effect);
            Apply(e => e.Effect.Once(this));
        }
    }
    public void RemoveEffect(Upgrade effect)
    {
        this.effects.Remove(effect);
    }

    private void Apply(Action<Upgrade> action)
    {
        effects.GetAll().ForEach(action);
        upgrades.GetAll().ForEach(action);
    }

    public void RoundStart() => Apply(e => e.Effect.RoundStart(this));
    public void RoundEnd() => Apply(e => e.Effect.RoundEnd(this));
    public void LightAttackSuccess(IUnit target)
    {
        Apply(e => e.Effect.LightAttackSuccess(this, target));
        Apply(e => e.Effect.AttackSuccess(this, target));
        Apply(e => e.Effect.AbilitySuccess(this, target));
    }
    public void HeavyAttackSuccess(IUnit target)
    {
        Apply(e => e.Effect.HeavyAttackSuccess(this, target));
        Apply(e => e.Effect.AttackSuccess(this, target));
        Apply(e => e.Effect.AbilitySuccess(this, target));
    }
    public void HealSuccess(IUnit target)
    {
        Apply(e => e.Effect.HealSuccess(this, target));
        Apply(e => e.Effect.AbilitySuccess(this, target));
    }
    public void LightAttackFail(IUnit target)
    {
        Apply(e => e.Effect.LightAttackFail(this, target));
        Apply(e => e.Effect.AttackFail(this, target));
        Apply(e => e.Effect.AbilityFail(this, target));
    }
    public void HeavyAttackFail(IUnit target)
    {
        Apply(e => e.Effect.HeavyAttackFail(this, target));
        Apply(e => e.Effect.AttackFail(this, target));
        Apply(e => e.Effect.AbilityFail(this, target));
    }
    public void HealFail(IUnit target)
    {
        Apply(e => e.Effect.HealFail(this, target));
        Apply(e => e.Effect.AbilityFail(this, target));
    }
    public void Move() => Apply(e => e.Effect.Move(this));
}