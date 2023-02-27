using System.Collections;
using UnityEngine;

public class UnitUpgrades : MonoBehaviour
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
            if (unit.Upgrades.Length > i && unit.Upgrades[i] != null /*&& unit.Upgrades[i].Icon != null*/)
            {
                slots[i].Spawn(unit.Upgrades[i]);             
            }
            else
            {
                slots[i].Destroy();
            }
        }
    }
}
