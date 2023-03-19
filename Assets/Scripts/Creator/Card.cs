using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;

public partial class Card : MonoBehaviour
{
    public void CreateAbilities()
    {
        var text = File.ReadAllText("Assets/Data/Abilities.csv");
        Debug.Log(text);
    }

    public void CreateExampleCard()
    {
        var upgrade = new Upgrade(gameObject, "Poison", "Unit you hit gets poisoned. It takes 3 damage each round.", "Potion Making", "erlenmeyer", new Poison());
        upgrade.Card.SetPosition(new Vector2(7, 0));
        var ability = new Ability(gameObject, "Elven Sword", AbilityType.FastAttack, 70, 9, 6, "broadsword");
        ability.Card.SetPosition(new Vector2(14, 0));

        for(int row = 0; row < 3; row++)
        {
            for(int column = 0; column < 4; column++)
            {
                var unit = new Unit(gameObject, "Cavalier " + row + " " + column, 30, ability, null, ability, upgrade, null, "addroran");
                unit.creator.SetPosition(new Vector2(column * 7 - 21, row * 10 - 10));
            }
        }
    }

    [EditorCools.Button]
    private void Generate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        CreateAbilities();
    }
}
