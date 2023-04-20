using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;


public abstract class SlotDrawer<I, S, D> : MonoBehaviour where I: Interactable where S: ItemSlot<I, S> where D: SlotDrawer<I, S, D>
{
    protected List<S> list;
    private Creator creator;

    protected static D New(Creator creator, string reason, uint horizontal, uint vertical, Vector2 position)
    {
        var parent = creator.FindGameObject(reason);
        creator.FindComponent<RectTransform>(parent).sizeDelta = new Vector2(Creator.cardWidth, Creator.cardHeight);
        var slotDrawer = parent.AddComponent<D>();
        slotDrawer.creator = creator;
        slotDrawer.list = new List<S>();
        for (int row = 0; row < vertical; row++)
        {
            for (int col = 0; col < horizontal; col++)
            {
                slotDrawer.list.Add(ItemSlot<I, S>.New(creator, reason + " " + row + " " + col, parent, horizontal > vertical, 
                    new Vector2(position.x + Creator.hexagonWidth * col, position.y + Creator.hexagonHeight * row)));
            }
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
        return list.Where(i=>i.Get() != null).Select(i => i.Get()).ToList();
    }
}
