using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

class Unit : IUnit
{
    private readonly Card.Creator creator;
    private readonly Card.Creator.Badge hp;
    public readonly Card.Creator.SlotDrawer<Ability> abilities;
    public readonly Card.Creator.SlotDrawer<Upgrade> upgrades;

    public uint HP { get => hp.Count; set => hp.Count = value; }

    public ReadOnlyCollection<IAbility> Abilities => new(abilities.GetAll().Select(i => (IAbility)i).ToList());

    public void RemoveEffects()
    {
        abilities.GetAll().ForEach(a => a.RemoveEffects());
    }

    public Unit(GameObject parent, uint hp, Ability ability)
    {
        creator = new Card.Creator("Warrior", parent)
            .Background()
            .LeftTitle()
            .MaskedImage("Artwork", new Rect(-1, 0.4f, 4, 5), "Artwork", "addroran", FSColor.White, 1.0f);
        abilities = new Card.Creator.SlotDrawer<Ability>(creator, "Abilities", 3, true, new Vector2(-2, -3.25f));
        abilities.Set(1, ability);
        abilities.Set(1, null);
        abilities.Set(2, ability);
        upgrades = new Card.Creator.SlotDrawer<Upgrade>(creator, "Upgrades", 2, false, new Vector2(2, -1));
        this.hp = new Card.Creator.Badge(creator, hp, FSColor.Red);
        HP = 25;
    }    
}

