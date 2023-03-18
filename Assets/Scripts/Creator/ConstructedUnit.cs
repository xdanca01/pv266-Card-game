using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

class ConstructedUnit : IUnit
{
    private readonly Card.Creator creator;
    private readonly Card.Creator.Badge hp;
    public readonly Card.Creator.SlotDrawer<ConstructedAbility> abilities;
    public readonly Card.Creator.SlotDrawer<ConstructedUpgrade> upgrades;

    public uint HP { get => hp.Count; set => hp.Count = value; }

    public ReadOnlyCollection<IAbility> Abilities => new(abilities.GetAll().Select(i => (IAbility)i).ToList());

    public void RemoveEffects()
    {
        abilities.GetAll().ForEach(a => a.RemoveEffects());
    }

    public ConstructedUnit(GameObject parent, uint hp, ConstructedAbility ability)
    {
        creator = new Card.Creator("Warrior", parent)
            .Background()
            .LeftTitle()
            .MaskedImage("Artwork", new Rect(-1, 0.4f, 4, 5), "Artwork", "addroran", FSColor.White, 1.0f);
        abilities = new Card.Creator.SlotDrawer<ConstructedAbility>(creator, "Abilities", 3, true, new Vector2(-2, -3.25f));
        abilities.Set(1, ability);
        abilities.Set(1, null);
        abilities.Set(2, ability);
        upgrades = new Card.Creator.SlotDrawer<ConstructedUpgrade>(creator, "Upgrades", 2, false, new Vector2(2, -1));
        this.hp = new Card.Creator.Badge(creator, hp, FSColor.Red);
        HP = 25;
    }    
}

