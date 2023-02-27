using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAbilities : MonoBehaviour
{
    public Unit unit;
    public Slot[] slots;

    public void OnValidate()
    {
        StartCoroutine(Instantiator());
    }

    IEnumerator Instantiator()
    {
        yield return null;
        for (int i = 0; i < slots.Length; i++)
        {
            if (unit.Abilities.Length > i && unit.Abilities[i] != null && unit.Abilities[i].Icon != null)
            {
                slots[i].Spawn(unit.Abilities[i].Icon.gameObject);
            }
            else
            {
                slots[i].Destroy();
            }
        }
    }
}
