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
    White = 0xFFFFFF
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
        private readonly GameObject gameobject;
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

        private Creator Text(string purpose, GameObject parent, string text, Rect rect, FSFont font)
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
            return this;
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
            gameobject = new GameObject();
            gameobject.name = name;
            gameobject.transform.parent = parent.transform;
            FindComponent<Canvas>(gameobject);
            SetRect(gameobject, new Rect(0, 0, cardWidth, cardHeight));
            FindComponent<GraphicRaycaster>(gameobject);
        }

        public Creator Background()
        {
            var background = FindGameObject("Background");
            var spriteRenderer = FindComponent<SpriteRenderer>(background);
            spriteRenderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/9-SlicedWithBorder.png");
            spriteRenderer.drawMode = SpriteDrawMode.Sliced;
            spriteRenderer.size = new Vector2(cardWidth + 0.4f, cardHeight + 0.4f);
            spriteRenderer.color = new Color(0f, 0f, 0f, 0.5f);
            spriteRenderer.sortingOrder = -1;
            return this;
        }

        public Creator LeftTitle()
        {
            return Text("Title", gameobject, gameobject.name, new Rect(-1f, 3.5f, 4f, 1.2f), FSFont.Geizer);
        }
        public Creator MiddleTitle()
        {
            return Text("Title", gameobject, gameobject.name, new Rect(0f, 3.5f, 5f, 2f), FSFont.Geizer);
        }

        public Creator Description(string text, FSFont font)
        {
            return Text("Description", gameobject, text, new Rect(0f, -3f, 5f, 2.75f), font);
        }

        public Creator MaskedImage(string reason, Rect rect, string spriteFolder, string spriteName, FSColor color)
        {
            // mask
            var mask = FindGameObject(reason + " Mask");
            var spriteMask = FindComponent<SpriteMask>(mask);
            spriteMask.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Packages/com.unity.2d.sprite/Editor/ObjectMenuCreation/DefaultAssets/Textures/v2/Square.png");
            var rectTransform = mask.GetComponent<RectTransform>();
            rectTransform.transform.position = new Vector3(rect.x, rect.y, mask.transform.position.z);
            rectTransform.localScale = new Vector3(rect.width, rect.height, 1);
            // inner sprite
            var image = FindGameObject(reason, mask);
            SetRect(image, new Rect(0, 0, 1, 1));
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/" + spriteFolder + "/" + spriteName + ".png");
            bool fitToSmallerSize = true;
            var spriteRenderer = FindComponent<SpriteRenderer>(image);
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = color.ToColor();
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            Vector3 spriteSize = spriteRenderer.sprite.bounds.size;
            Transform transform = image.GetComponent<RectTransform>().transform;
            float dimension = fitToSmallerSize ? Mathf.Max(spriteSize.x, spriteSize.y) : Mathf.Min(spriteSize.x, spriteSize.y);
            float maskScaleX = rect.height > rect.width ? rect.width / rect.height : 1;
            float maskScaleY = rect.width > rect.height ? rect.height / rect.width : 1;
            float spriteScaleX = spriteSize.y > spriteSize.x ? spriteSize.y / spriteSize.x : spriteSize.x / spriteSize.y;
            float spriteScaleY = spriteSize.x > spriteSize.y ? spriteSize.x / spriteSize.y : spriteSize.y / spriteSize.x;
            transform.localScale = new Vector3(spriteScaleX / (dimension * maskScaleX), spriteScaleY / (dimension * maskScaleY), 1);
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
                set
                {
                    color = value;
                    creator.Hexagon("Hexagon", gameobject, color, true);
                }
            }
            private uint count;
            public uint Count
            {
                get => count;
                set
                {
                    count = value;
                    creator.Text("Text", gameobject, count.ToString(), new Rect(0f, 0f, 1.5f, 1.1f), FSFont.DeadRevolution);
                }
            }
        }

        public class Slot
        {
            private readonly GameObject gameobject;
            private readonly Creator creator;
            public Slot(Creator creator, string reason, GameObject parent, bool pointedUp, Vector2 position)
            {
                this.creator = creator;
                gameobject = creator.Hexagon(reason, parent, FSColor.DarkGray, pointedUp);
                gameobject.transform.position = new Vector3(position.x, position.y, gameobject.transform.position.z);
                var child = creator.Hexagon("Child", gameobject, FSColor.LightGray, pointedUp);
                child.GetComponent<RectTransform>().localScale = new Vector3(1f / 3f, 1f / 3f, 1f);                
            }
        }

        public class Icon
        {
            private readonly GameObject gameobject;
            private readonly Creator creator;
            public Icon(Creator creator, GameObject parent, string title, string description, string spriteName, FSColor color)
            {
                var icon = creator.MaskedImage("Icon", new Rect(0, 0, 2, 2), "Icons", spriteName, color).gameobject;
                this.creator = creator
                    .Text("Title", icon, title, new Rect(0f, 0.5f, 2f, 1f), FSFont.DeadRevolution)
                    .Text("Description", icon, description, new Rect(0f, -0.5f, 2f, 1f), FSFont.DeadRevolution);
                gameobject = creator.gameobject;
            }
        }

        public class SlotDrawer
        {
            private readonly List<Slot> list;
            private readonly Creator creator;
            
            public SlotDrawer(Creator creator, string reason, int count, bool horizontal, Vector2 position)
            {
                this.creator = creator;
                var parent = creator.FindGameObject(reason);
                this.list = new List<Slot>();
                for (int i = 0; i < count; i++)
                {
                    list.Add(new Slot(creator, reason + " " + i, parent, horizontal, 
                        horizontal ? new Vector2(position.x + 2 * i, position.y) : new Vector2(position.x, position.y + 2 * i)));
                }
            }
        }

        public GameObject Build()
        {
            return gameobject;
        }
    }

    class ConstructedUnit : IUnit
    {
        private readonly Creator creator;
        private readonly Creator.Badge hp;
        private readonly Creator.SlotDrawer abilities;
        private readonly Creator.SlotDrawer upgrades;

        public uint HP { get => hp.Count; set => hp.Count = value; }

        public ReadOnlyCollection<IAbility> Abilities => throw new System.NotImplementedException();

        public IUnit Revert()
        {
            throw new System.NotImplementedException();
        }

        public ConstructedUnit(GameObject parent, uint hp)
        {
            creator = new Creator("Warrior", parent)
                .Background()
                .LeftTitle()
                .MaskedImage("Artwork", new Rect(-1, 0.4f, 4, 5), "Artwork", "addroran", FSColor.White);
            abilities = new Creator.SlotDrawer(creator, "Abilities", 3, true, new Vector2(-2, -3.25f));
            upgrades = new Creator.SlotDrawer(creator, "Upgrades", 2, false, new Vector2(2, -1));
            this.hp = new Creator.Badge(creator, hp, FSColor.Red);
            HP = 25;
        }
    
    }

    class ConstructedUpgrade : IUpgrade
    {
        private readonly Creator creator;
        private readonly Creator.Icon icon;

        public IEffect Effect => throw new System.NotImplementedException();

        public ConstructedUpgrade(GameObject parent)
        {
            creator = new Creator("Poison", parent)
                .Background()
                .MiddleTitle()
                .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Artwork", "Potion Making", FSColor.White)
                .Description("Creature you hit gets poisoned. It takes 8 damage each round.", FSFont.Dumbledor);
            icon = new Creator.Icon(creator, parent, "Hit", "Poison", "erlenmeyer", FSColor.Blue);
        }
    }

    class ConstructedAbility : IAbility
    {
        private readonly Creator creator;
        private readonly Creator.Icon icon;

        public uint Percentage { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public uint Low { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public uint High { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public AbilityType Type => throw new System.NotImplementedException();

        public IAbility Revert() => throw new System.NotImplementedException();
        
        public ConstructedAbility(GameObject parent)
        {
            creator = new Creator("Elven Sword", parent)
                .Background()
                .MiddleTitle()
                .MaskedImage("Artwork", new Rect(0, 0.4f, 4, 4), "Icons", "broadsword", FSColor.Yellow)
                .Description("80% 6-9\nLIGHT DMG", FSFont.DeadRevolution);
            //icon = new Creator.Icon(creator, parent, "80%", "6-9", "broadsword", FSColor.Yellow);
        }
    }

    public void CreateExampleCard()
    {
        //var unit = new ConstructedUnit(gameObject, 30);
        //var upgrade = new ConstructedUpgrade(gameObject);
        var ability = new ConstructedAbility(gameObject);        
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
