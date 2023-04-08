using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Creator
{
    public readonly GameObject gameobject;
    public static readonly float cardWidth = 6f;
    public static readonly float cardHeight = 9f;
    public static readonly float cardWidthWithBorder = cardWidth + 0.4f;
    public static readonly float cardHeightWithBorder = cardHeight + 0.4f;
    public void SetRect(GameObject gameobject, Rect rect)
    {
        var transform = gameobject.GetComponent<RectTransform>();
        transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
        transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
        transform.anchoredPosition = new Vector2(rect.x, rect.y);
    }

    public GameObject FindGameObject(string name, GameObject parent)
    {
        var transform = parent.transform.Find(name);
        if (transform == null)
        {
            var gameobject = new GameObject(name, typeof(RectTransform));
            gameobject.GetComponent<RectTransform>().SetParent(parent.transform, false);
            return gameobject;
        }
        else
        {
            return transform.gameObject;
        }
    }

    public GameObject FindGameObject(string name)
    {
        return FindGameObject(name, gameobject);
    }

    public T FindComponent<T>(GameObject gameobject) where T : Component
    {
        if (!gameobject.TryGetComponent<T>(out T component))
        {
            component = gameobject.AddComponent<T>();
        }
        return component;
    }

    public Creator Text(string purpose, GameObject parent, string text, Rect rect, FSFont font, FSColor color)
    {
        var textGO = FindGameObject(purpose, parent);
        SetRect(textGO, rect);
        textGO.name = purpose;
        var TMPro = FindComponent<TextMeshProUGUI>(textGO);
        TMPro.text = text;
        TMPro.enableAutoSizing = true;
        TMPro.fontSizeMax = 100;
        TMPro.fontSizeMin = 0;
        TMPro.font = font.ToAsset();
        TMPro.color = color.ToColor();
        TMPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
        TMPro.verticalAlignment = VerticalAlignmentOptions.Middle;
        return this;
    }

    public Creator Text(string purpose, GameObject parent, string text, Rect rect, FSFont font)
    {
        return Text(purpose, parent, text, rect, font, FSColor.White);
    }

    public GameObject Hexagon(string reason, GameObject parent, FSColor color, bool pointedUp)
    {
        var hexagon = FindGameObject(reason, parent);
        SetRect(hexagon, new Rect(0f, 0f, 1f, 1f));
        var image = FindComponent<Image>(hexagon);
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
            pointedUp ? "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/HexagonPointed-TopWithBorder.png"
                        : "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/HexagonFlat-TopWithBorder.png");
        image.color = color.ToColor();
        image.preserveAspect = true; 
        hexagon.GetComponent<RectTransform>().localScale = new Vector3(2f, 2f, 1f);        
        return hexagon;
    }

    public Creator(string name, GameObject parent)
    {
        gameobject = new GameObject();
        gameobject.name = name;
        gameobject.transform.parent = parent.transform;
        var canvas = FindComponent<Canvas>(gameobject);
        canvas.worldCamera = Camera.main;
        SetRect(gameobject, new Rect(0, 0, cardWidth, cardHeight));
        FindComponent<GraphicRaycaster>(gameobject);
    }

    public Creator Background()
    {
        var background = FindGameObject("Background");
        var image = FindComponent<Image>(background);
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/9-Sliced.png");
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 100;
        FindComponent<RectTransform>(background).sizeDelta = new Vector2(cardWidthWithBorder, cardHeightWithBorder);
        image.color = new Color(0f, 0f, 0f, 0.5f);
        return this;
    }

    public Creator LeftTitle()
    {
        return Text("Title", gameobject, gameobject.name, new Rect(-1f, 3.5f, 4f, 1.2f), FSFont.Geizer);
    }
    public Creator MiddleTitle()
    {
        return Text("Title", gameobject, gameobject.name, new Rect(0f, 3.5f, 5f, 1.2f), FSFont.Geizer);
    }

    public Creator Description(string text, FSFont font)
    {
        return Text("Description", gameobject, text, new Rect(0f, -3f, 5f, 2.75f), font);
    }

    public GameObject MaskedImageGameObject(string reason, GameObject parent, Rect rect, string spriteFolder, string spriteName, FSColor color, float alpha)
    {
        // outer mask
        var mask = FindGameObject(reason + " Mask", parent);
        var maskMask = FindComponent<RectMask2D>(mask);
        var maskRectTransform = mask.GetComponent<RectTransform>();
        maskRectTransform.transform.position = new Vector3(rect.x, rect.y, mask.transform.position.z);
        maskRectTransform.sizeDelta = new Vector2(rect.width, rect.height);
        // inner image
        var image = FindGameObject(reason, mask);
        SetRect(image, new Rect(0, 0, 1, 1));
        Sprite imageSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/" + spriteFolder + "/" + spriteName + ".png");
        var imageRenderer = FindComponent<Image>(image);
        imageRenderer.sprite = imageSprite;
        imageRenderer.color = color.ToColor(alpha);
        imageRenderer.preserveAspect = true;
        RectTransform imageRect = image.GetComponent<RectTransform>();
        Vector3 spriteSize = imageSprite.bounds.size;
        imageRect.sizeDelta = spriteSize.x > spriteSize.y ? new Vector2(spriteSize.x, rect.height) : new Vector2(rect.width, spriteSize.y);
        return mask;
    }

    public Creator MaskedImage(string reason, Rect rect, string spriteFolder, string spriteName, FSColor color)
    {
        MaskedImageGameObject(reason, gameobject, rect, spriteFolder, spriteName, color, 1.0f);
        return this;
    }

    public void SetPosition(Vector2 position)
    {
        gameobject.transform.position = new Vector3(position.x, position.y, gameobject.transform.position.z);
    }

    public LineRenderer Line(string reason, Vector3 from, Vector3 to, FSColor color)
    {
        GameObject go = FindGameObject(reason);
        LineRenderer line = FindComponent<LineRenderer>(go);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.startColor = FSColor.DarkGray.ToColor();
        line.endColor = color.ToColor();        
        var dir = to - from;
        var normal = new Vector3(-dir.y, dir.x, dir.z).normalized;
        var quarter1 = (-dir.normalized + normal + normal / 4).normalized / 2;
        var quarter2 = (-dir.normalized - normal - normal / 4).normalized / 2;
        var points = new Vector3[]{from, from + dir / 4 + normal / 2.5f,
            from + dir / 2 + normal / 2, from + 3 * dir/4 + normal / 2.5f,
            to, to + quarter1, to, to + quarter2};
        line.positionCount = points.Length;
        line.SetPositions(points);
        line.useWorldSpace = true;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingOrder = 1;
        return line;
    }
}
