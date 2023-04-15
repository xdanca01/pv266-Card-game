using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    public static Background NewWithDimensions(Creator creator, GameObject parent, Rect rect)
    {
        var gameObject = creator.FindGameObject("Background", parent);
        var background = creator.FindComponent<Background>(gameObject);
        var image = creator.FindComponent<Image>(gameObject);
        image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/9-Sliced.png");
        image.type = Image.Type.Sliced;
        image.pixelsPerUnitMultiplier = 100;
        creator.FindComponent<RectTransform>(gameObject).sizeDelta = new Vector2(rect.width, rect.height);
        image.color = new Color(0f, 0f, 0f, 0.5f);
        background.transform.SetAsFirstSibling();
        gameObject.transform.position = rect.position;
        return background;
    }

    public static Background New(Creator creator)
    {
        return NewWithDimensions(creator, creator.gameobject, new Rect(0, 0, Creator.cardWidthWithBorder, Creator.cardHeightWithBorder));
    }

    public void SetColor(FSColor color)
    {
        gameObject.GetComponent<Image>().color = color.ToColor(0.5f);
    }

    public void SetAlpha(float alpha)
    {
        var img = gameObject.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, alpha);
    }
}
