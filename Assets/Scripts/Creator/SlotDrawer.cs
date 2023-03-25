using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public class SlotDrawer : MonoBehaviour
{
    private List<ItemSlot> list;
    private Creator creator;

    public static SlotDrawer New(Creator creator, string reason, uint count, bool horizontal, Vector2 position)
    {
        var parent = creator.FindGameObject(reason);
        var slotDrawer = parent.AddComponent<SlotDrawer>();
        slotDrawer.creator = creator;
        slotDrawer.list = new List<ItemSlot>();
        for (int i = 0; i < count; i++)
        {
            slotDrawer.list.Add(ItemSlot.New(creator, reason + " " + i, parent, horizontal,
                horizontal ? new Vector2(position.x + 2 * i, position.y) : new Vector2(position.x, position.y + 2 * i)));
        }
        return slotDrawer;
    }

    public void Set(uint nth, Interactable interactible)
    {
        if (nth >= list.Count)
        {
            throw new System.Exception("Slot drawers does not have " + nth + " slots but only " + list.Count);
        }
        list[(int)nth].Set(interactible);
    }

    public T Get<T>(uint nth) where T : Interactable
    {
        if (nth >= list.Count)
        {
            throw new System.Exception("Slot drawers does not have " + nth + " slots but only " + list.Count);
        }
        return (T)list[(int)nth].Get();
    }

    public List<T> GetAll<T>() where T: Interactable
    {
        return list.Select(i => (T) i.Get()).ToList();
    }
}
    