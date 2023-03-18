using System.Collections.ObjectModel;
using UnityEngine;

public class CardCreator : MonoBehaviour
{
    class ConstructedUnit : IUnit
    {
        private readonly Creator creator;
        private readonly Creator.Badge hp;
        private readonly Creator.SlotDrawer abilities;
        private readonly Creator.SlotDrawer upgrades;

        public uint HP { get => hp.Count; set => hp.Count = value; }

        public ReadOnlyCollection<IAbility> Abilities => throw new System.NotImplementedException();

        public IUnit Revert()
        {
            throw new System.NotImplementedException();
        }

        public ConstructedUnit(GameObject parent, uint hp)
        {
            creator = new Creator("Warrior", parent)
                .Background()
                .LeftTitle()
                .MaskedImage("Artwork", new Rect(-1, 0.4f, 4, 5), "Artwork", "addroran", FSColor.White);
            abilities = new Creator.SlotDrawer(creator, "Abilities", 3, true, new Vector2(-2, -3.25f));
            upgrades = new Creator.SlotDrawer(creator, "Upgrades", 2, false, new Vector2(2, -1));
            this.hp = new Creator.Badge(creator, hp, FSColor.Red);
            HP = 25;
        }
    
    }

    class ConstructedUpgrade : IUpgrade
    {
        private readonly Creator creator;
        private readonly Creator.Icon icon;

        public IEffect Effect => throw new System.NotImplementedException();

        public ConstructedUpgrade(GameObject parent)
        {
            creator = new Creator("Poison", parent)
                .Background()
                .MiddleTitle()
                .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Artwork", "Potion Making", FSColor.White)
                .Description("Creature you hit gets poisoned. It takes 8 damage each round.", FSFont.Dumbledor);
            icon = new Creator.Icon(creator, "Hit", "Poison", "erlenmeyer", FSColor.Blue);
        }
    }

    class ConstructedAbility : IAbility
    {
        private readonly Creator creator;
        private readonly Creator.Icon icon;

        public uint Percentage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public uint Low { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public uint High { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public AbilityType Type => throw new System.NotImplementedException();

        public IAbility Revert() => throw new System.NotImplementedException();
        
        public ConstructedAbility(GameObject parent)
        {
            creator = new Creator("Elven Sword", parent)
                .Background()
                .MiddleTitle()
                .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Icons", "broadsword", FSColor.Yellow)
                .Description("80% 6-9\nLIGHT DMG", FSFont.DeadRevolution);
            icon = new Creator.Icon(creator, "80%", "6-9", "broadsword", FSColor.Yellow);
            icon.Hide();
        }
    }

    public void CreateExampleCard()
    {
        //var unit = new ConstructedUnit(gameObject, 30);
        //var upgrade = new ConstructedUpgrade(gameObject);
        var ability = new ConstructedAbility(gameObject);        
    }

    [EditorCools.Button]
    private void Generate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        CreateExampleCard();
    }
}
