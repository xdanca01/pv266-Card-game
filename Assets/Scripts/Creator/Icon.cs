using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Icon : MonoBehaviour
{
    private Creator creator;
    private string title;
    public string Title { get => title; set {
            title = value;
            UpdateText();
            foreach (var icon in linkedCopies)
            {
                icon.Title = title;
            }
        }
    }
    private string description;
    public string Description { get => description; set {
            description = value;
            UpdateText();
            foreach (var icon in linkedCopies)
            {
                icon.Description = description;
            }
        }
    }
    private string spriteName;
    public string SpriteName { get => spriteName; set {
            spriteName = value;
            UpdateImage();
            foreach (var icon in linkedCopies)
            {
                icon.SpriteName = spriteName;
            }
        }
    }
    private FSColor color;
    public FSColor Color { get => color; set {
            color = value;
            UpdateImage();
            foreach (var icon in linkedCopies)
            {
                icon.Color = color;
            }
        }
    }

    private FSColor textColor;
    public FSColor TextColor { get => textColor; set {
            textColor = value;
            UpdateText();
            foreach (var icon in linkedCopies)
            {
                icon.TextColor = color;
            }
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

    public Sprite GetSprite()
    {
        return Resources.Load<Sprite>("Icons/"+spriteName);
    }

    private List<Icon> linkedCopies;

    public Func<GameObject, Icon> FreshCopy;

    public Func<GameObject, Icon> LinkedCopy;

    public static Icon New(Creator creator, GameObject parent, string title, string description, string spriteName, FSColor color)
    {
        var gameobject = creator.FindGameObject("Icon", parent);
        var icon = gameobject.AddComponent<Icon>();
        icon.linkedCopies = new();
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
        icon.LinkedCopy = (GameObject parent) => {
            var ic = Icon.New(icon.creator, parent, icon.Title, icon.Description, icon.spriteName, icon.color);
            icon.linkedCopies.Add(ic);
            return ic;
        };
        return icon;
    }

}
