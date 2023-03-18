using UnityEngine;

public partial class Card
{
    public partial class Creator
    {
        public class Slot<T> where T: Interactable
        {
            private readonly GameObject gameobject;
            private readonly GameObject empty;
            private readonly Creator creator;
            private GameObject icon;
            private T interactible;

            public Slot(Creator creator, string reason, GameObject parent, bool pointedUp, Vector2 position)
            {
                this.creator = creator;
                gameobject = creator.FindGameObject(reason, parent);
                creator.SetRect(gameobject, new Rect(position.x, position.y, 2, 2));
                empty = creator.Hexagon("Empty", gameobject, FSColor.DarkGray, pointedUp);
                empty.transform.position = new Vector3(position.x, position.y, empty.transform.position.z);
                var child = creator.Hexagon("Child", empty, FSColor.LightGray, pointedUp);
                child.GetComponent<RectTransform>().localScale = new Vector3(1f / 3f, 1f / 3f, 1f);
                child.GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
            // if icon = null then slot will become empty
            public void Set(T interactible)
            {
                if (this.icon != null)
                {
                    DestroyImmediate(this.icon);
                }
                if (interactible == null)
                {
                    empty.SetActive(true);
                    icon = null;
                    this.interactible = default;
                }
                else
                {
                    empty.SetActive(false);
                    icon = interactible.Icon.Create(gameobject);
                    this.interactible = interactible;
                }
            }
            public T Get()
            {
                return interactible;
            }
        }
    }
}
