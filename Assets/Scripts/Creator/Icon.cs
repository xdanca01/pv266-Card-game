using System;
using System.Collections.Generic;
using UnityEngine;

public class Icon : MonoBehaviour
{
    private Creator creator;
    private string title;
    public string Title { get => title; set {
            title = value;
            UpdateText();
        }
    }
    private string description;
    public string Description { get => description; set {
            description = value;
            UpdateText();
        }
    }
    private string spriteName;
    public string SpriteName { get => spriteName; set {
            spriteName = value;
            UpdateImage();
        }
    }
    private FSColor color;
    public FSColor Color { get => color; set {
            color = value;
            UpdateImage();
        }
    }

    private FSColor textColor;
    public FSColor TextColor { get => textColor; set {
            textColor = value;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        creator.Text("Title", gameObject, title, new Rect(0f, 0.5f, 2f, 1f), FSFont.DeadRevolution, textColor);
        creator.Text("Description", gameObject, description, new Rect(0f, -0.5f, 2f, 1f), FSFont.DeadRevolution, textColor);
    }

    private GameObject UpdateImage()
    {
        return creator.MaskedImageGameObject("Image", gameObject, new Rect(0, 0, 2, 2), "Icons", spriteName, color, 0.5f);
    }

    public Func<GameObject, Icon> FreshCopy;

    public static Icon New(Creator creator, GameObject parent, string title, string description, string spriteName, FSColor color)
    {
        var gameobject = creator.FindGameObject("Icon", parent);
        var icon = gameobject.AddComponent<Icon>();
        icon.creator = creator;
        creator.SetRect(gameobject, parent.GetComponent<RectTransform>().rect);
        icon.spriteName = spriteName;
        icon.color = color;
        icon.UpdateImage().transform.position = gameobject.transform.position; 
        icon.title = title;
        icon.description = description;
        icon.UpdateText();      
        icon.TextColor = FSColor.White;
        gameobject.transform.position = parent.transform.position;
        icon.FreshCopy = (GameObject parent) => Icon.New(icon.creator, parent, icon.Title, icon.Description, icon.spriteName, icon.color);
        return icon;
    }
}
