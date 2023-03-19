using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public partial class Card
{
    public partial class Creator
    {
        public class SlotDrawer<T> where T : Interactable
        {
            private readonly List<Slot<T>> list;
            private readonly Creator creator;

            public SlotDrawer(Creator creator, string reason, uint count, bool horizontal, Vector2 position)
            {
                this.creator = creator;
                var parent = creator.FindGameObject(reason);
                this.list = new List<Slot<T>>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(new Slot<T>(creator, reason + " " + i, parent, horizontal,
                        horizontal ? new Vector2(position.x + 2 * i, position.y) : new Vector2(position.x, position.y + 2 * i)));
                }
            }

            public void Set(uint nth, T interactible)
            {
                if (nth >= list.Count)
                {
                    throw new System.Exception("Slot drawers does not have " + nth + " slots but only " + list.Count);
                }
                list[(int)nth].Set(interactible);
            }

            public T Get(uint nth)
            {
                if (nth >= list.Count)
                {
                    throw new System.Exception("Slot drawers does not have " + nth + " slots but only " + list.Count);
                }
                return list[(int)nth].Get();
            }

            public List<T> GetAll()
            {
                return list.Select(i => i.Get()).ToList();
            }
        }
    }
}
