using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum FSColor
{
    Yellow = 0xBF9F00,
    Orange = 0xBF6000,
    Red = 0xBF0000,
    Blue = 0x00A0FF,
    Violet = 0xAE00FF,
    Black = 0x0,
    DarkGray = 0x525252,
    LightGray = 0x808080,
}

static class FSColorMethods
{
    public static Color ToColor(this FSColor color) => ToColorAlpha(color, 1.0f);
    public static Color ToColorAlpha(this FSColor color, float alpha) => new(
        (((int)color & 0xFF0000) >> 16) / 255.0f, 
        (((int)color & 0x00FF00) >> 8) / 255.0f, 
        ((int)color & 0x0000FF) / 255.0f, 
        alpha
    );
}

public enum FSFont
{
    DeadRevolution,
    Dumbledor,
    Geizer
}
static class FSFontMethods
{
    public static TMP_FontAsset ToAsset(this FSFont font) => AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(font switch
    {
        FSFont.DeadRevolution => "Assets/Fonts/Dead Revolution/Dead Revolution SDF.asset",
        FSFont.Dumbledor => "Assets/Fonts/Dumbledor/dum1 SDF.asset",
        FSFont.Geizer => "Assets/Fonts/Geizer/Geizer SDF.asset",
        _ => throw new System.Exception("Unknown Font!"),
    });
}

public class CardCreator : MonoBehaviour
{
    private class Creator
    {
        private readonly GameObject card;
        private static readonly int cardWidth = 6;
        private static readonly int cardHeight = 9;
        private readonly Dictionary<string, GameObject> hiearchy = new Dictionary<string, GameObject>();

        private void SetRect(GameObject gameobject, Rect rect)
        {
            var transform = gameobject.GetComponent<RectTransform>();
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
            transform.anchoredPosition = new Vector2(rect.x, rect.y);
        }

        private string GetFullName(GameObject gameobject)
        {
            if (gameobject.transform.parent != null)
            {
                return GetFullName(gameobject.transform.parent.gameObject) + "/" + gameobject.name;
            }
            return gameobject.name;
        }

        private GameObject FindGameObject(string name, GameObject parent)
        {
            var parentName = GetFullName(parent);
            var fullName = parentName + "/" + name;
            if (!hiearchy.TryGetValue(fullName, out GameObject gameobject))
            {
                gameobject = new GameObject(name, typeof(RectTransform));
                gameobject.GetComponent<RectTransform>().SetParent(parent.transform, false);
                hiearchy.Add(fullName, gameobject);
            }
            return gameobject;
        }

        private GameObject FindGameObject(string name)
        {
            return FindGameObject(name, card);
        }
        
        public T FindComponent<T>(GameObject gameobject) where T : Component
        {
            if (!gameobject.TryGetComponent<T>(out T component))
            {
                component = gameobject.AddComponent<T>();
            }
            return component;
        }

        private void Background()
        {
            var background = FindGameObject("Background");
            var spriteRenderer = FindComponent<SpriteRenderer>(background);
            spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/9-SlicedWithBorder.png");
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            spriteRenderer.size = new Vector2(cardWidth + 0.4f, cardHeight + 0.4f);
            spriteRenderer.color = new Color(0f, 0f, 0f, 0.5f);
            spriteRenderer.sortingOrder = -1;
        }

        private GameObject Text(string purpose, GameObject parent, string text, Rect rect, FSFont font)
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
            TMPro.horizontalAlignment = HorizontalAlignmentOptions.Center;
            TMPro.verticalAlignment = VerticalAlignmentOptions.Middle;
            return textGO;
        }

        private GameObject Hexagon(string reason, GameObject parent, FSColor color, bool pointedUp)
        {
            var hexagon = FindGameObject(reason, parent);
            SetRect(hexagon, new Rect(0f, 0f, 1f, 1f));
            var spriteRenderer = FindComponent<SpriteRenderer>(hexagon);
            spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                pointedUp ? "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/HexagonPointed-TopWithBorder.png"
                          : "Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/HexagonFlat-TopWithBorder.png");
            spriteRenderer.color = color.ToColor();
            hexagon.GetComponent<RectTransform>().localScale = new Vector3(2f, 2f, 1f);
            return hexagon;
        }

        public Creator(string name, GameObject parent)
        {
            card = new GameObject();
            card.name = name;
            card.transform.parent = parent.transform;
            FindComponent<Canvas>(card);
            SetRect(card, new Rect(0, 0, cardWidth, cardHeight));
            FindComponent<GraphicRaycaster>(card);
            Background();
        }

        public Creator Title()
        {
            Text("Title", card, card.name, new Rect(-1f, 3.5f, 4f, 1.2f), FSFont.Geizer);
            return this;
        }

        public Creator Slot(string reason, bool pointedUp)
        {
            var hexagon = Hexagon(reason, card, FSColor.DarkGray, pointedUp);
            var child = Hexagon("Child", hexagon, FSColor.LightGray, pointedUp);
            child.GetComponent<RectTransform>().localScale = new Vector3(1f / 3f, 1f / 3f, 1f);
            return this;
        }

        public class Badge
        {
            private readonly GameObject gameobject;
            private readonly Creator creator;

            public Badge(Creator creator, uint count, FSColor color)
            {
                this.creator = creator;
                gameobject = creator.FindGameObject("Badge");
                creator.SetRect(gameobject, new Rect(2f, 3.5f, 2f, 2f));
                Color = color;
                Count = count;
            }
            private FSColor color;
            public FSColor Color
            {
                get => color;
                set {
                    color = value;
                    creator.Hexagon("Hexagon", gameobject, color, true);
                }
            }
            private uint count;
            public uint Count
            {
                get => count;
                set {
                    count = value;
                    creator.Text("Text", gameobject, count.ToString(), new Rect(0f, 0f, 1.5f, 1.1f), FSFont.DeadRevolution);
                }
            }
        }

        public GameObject Build()
        {
            return card;
        }
    }

    class ConstructedUnit : IUnit
    {
        private readonly Creator creator;
        private readonly Creator.Badge hp;

        public uint HP { get => hp.Count; set => hp.Count = value; }

        public ReadOnlyCollection<IAbility> Abilities => throw new System.NotImplementedException();

        public IUnit Revert()
        {
            throw new System.NotImplementedException();
        }

        public ConstructedUnit(GameObject parent, uint hp)
        {
            creator = new Creator("Warrior", parent)
                .Title()
                .Slot("Upgrade 1", false);
            this.hp = new Creator.Badge(creator, hp, FSColor.Red);
            HP = 25;
        }
    }

    public void CreateExampleCard()
    {
        var unit = new ConstructedUnit(gameObject, 30);

    }

    [EditorCools.Button]
    private void Generate()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        CreateExampleCard();
    }
}
