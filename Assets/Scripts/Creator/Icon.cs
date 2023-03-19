using System.Collections.Generic;
using UnityEngine;
using static Card.Creator;

public partial class Card
{
    public partial class Creator
    {
        public class Icon
        {
            private readonly Creator creator;
            private readonly string fullTitle;
            private string title;
            public string Title { get => title; set {
                    title = value;
                    Update();
                }
            }
            private string description;
            public string Description { get => description; set {
                    description = value;
                    Update();
                }
            }
            private string spriteName;
            public string SpriteName { get => spriteName; set {
                    spriteName = value;
                    Update();
                }
            }
            private FSColor color;
            public FSColor Color { get => color; set {
                    color = value;
                    Update();
                }
            }
            private List<GameObject> updatees;

            public Icon(Creator creator, string fullTitle, string title, string description, string spriteName, FSColor color)
            {
                this.creator = creator;
                this.fullTitle = fullTitle;
                this.title = title;
                this.description = description;
                this.spriteName = spriteName;
                this.color = color;
                this.updatees = new List<GameObject>();
            }

            private void Update()
            {
                foreach (var icon in updatees)
                {
                    creator.MaskedImageGameObject("Image", icon, new Rect(0, 0, 2, 2), "Icons", spriteName, color, 0.5f).transform.position = icon.transform.position;
                    creator.Text("Title", icon, title, new Rect(0f, 0.5f, 2f, 1f), FSFont.DeadRevolution);
                    creator.Text("Description", icon, description, new Rect(0f, -0.5f, 2f, 1f), FSFont.DeadRevolution);
                }
            }

            public GameObject Create(GameObject parent)
            {
                var gameobject = creator.FindGameObject(fullTitle + " Icon", parent);
                updatees.RemoveAll(x => x == null);
                updatees.Add(gameobject);
                creator.SetRect(gameobject, parent.GetComponent<RectTransform>().rect);
                Update();
                gameobject.transform.position = parent.transform.position;
                return gameobject;
            }
        }
    }
}
