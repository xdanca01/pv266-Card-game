using System.Collections;
using UnityEngine;

public abstract class SlotDrawer : MonoBehaviour
{
    public Unit unit;
    public Slot[] slots;

    protected abstract Card[] Elements { get; }

    public void OnValidate()
    {
        StartCoroutine(Instantiator());
    }

    IEnumerator Instantiator()
    {
        yield return null;
        for (int i = 0; i < slots.Length; i++)
        {
            if (Elements.Length > i && Elements[i] != null && Elements[i].IconVisual != null)
            {
                slots[i].Spawn(Elements[i].IconVisual.gameObject);
            }
            else
            {
                slots[i].Destroy();
            }
        }
    }
}
