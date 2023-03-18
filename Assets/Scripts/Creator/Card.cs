using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public partial class Card : MonoBehaviour
{
    public void CreateExampleCard()
    {
        //var unit = new ConstructedUnit(gameObject, 30);
        //var upgrade = new ConstructedUpgrade(gameObject);
        var ability = new Ability(gameObject, "Elven Sword", AbilityType.FastAttack, 70, 9, 6, "broadsword");
        var unit = new Unit(gameObject, 30, ability);

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
