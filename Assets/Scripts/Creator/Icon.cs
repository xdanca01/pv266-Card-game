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
            public GameObject gameobject;
            private string title;
            public string Title { get => title; set {
                    title = value;
                }
            }
            private string description;
            public string Description { get => description; set {
                    description = value;
                }
            }
            private string spriteName;
            public string SpriteName { get => spriteName; set {
                    spriteName = value;
                }
            }
            private FSColor color;
            public FSColor Color { get => color; set {
                    color = value;
                }
            }

            public Icon(Creator creator, string fullTitle, string title, string description, string spriteName, FSColor color)
            {
                this.creator = creator;
                this.fullTitle = fullTitle;
                this.title = title;
                this.description = description;
                this.spriteName = spriteName;
                this.color = color;
            }

            public GameObject Create(GameObject parent)
            {
                gameobject = creator.FindGameObject(fullTitle + " Icon", parent);                
                creator.SetRect(gameobject, parent.GetComponent<RectTransform>().rect);
                creator.MaskedImageGameObject("Image", gameobject, new Rect(0, 0, 2, 2), "Icons", spriteName, color, 0.5f).transform.position = gameobject.transform.position;
                creator.Text("Title", gameobject, title, new Rect(0f, 0.5f, 2f, 1f), FSFont.DeadRevolution);
                creator.Text("Description", gameobject, description, new Rect(0f, -0.5f, 2f, 1f), FSFont.DeadRevolution);
                gameobject.transform.position = parent.transform.position;
                return gameobject;
            }
        }
    }
}
