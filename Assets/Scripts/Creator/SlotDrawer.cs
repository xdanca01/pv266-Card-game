using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public abstract class SlotDrawer<I, S, D> : MonoBehaviour where I: Interactable where S: ItemSlot<I, S> where D: SlotDrawer<I, S, D>
{
    private List<S> list;
    private Creator creator;
    public static D New(Creator creator, string reason, uint count, bool horizontal, Vector2 position)
    {
        var parent = creator.FindGameObject(reason);
        creator.FindComponent<RectTransform>(parent).sizeDelta = new Vector2(Creator.cardWidth, Creator.cardHeight);
        var slotDrawer = parent.AddComponent<D>();
        slotDrawer.creator = creator;
        slotDrawer.list = new List<S>();
        for (int i = 0; i < count; i++)
        {
            slotDrawer.list.Add(ItemSlot<I, S>.New(creator, reason + " " + i, parent, horizontal,
                horizontal ? new Vector2(position.x + 2 * i, position.y) : new Vector2(position.x, position.y + 2 * i)));
        }
        return slotDrawer;
    }

    public void Set(uint nth, I interactible)
    {
        if (nth >= list.Count)
        {
            throw new System.Exception("Slot drawers does not have " + nth + " slots but only " + list.Count);
        }
        list[(int)nth].Set(interactible);
    }

    public I Get(uint nth)
    {
        if (nth >= list.Count)
        {
            throw new System.Exception("Slot drawers does not have " + nth + " slots but only " + list.Count);
        }
        return list[(int)nth].Get();
    }

    public List<I> GetAll()
    {
        return list.Select(i => i.Get()).ToList();
    }
}
    