using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;

public partial class Card
{
    public partial class Creator
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

        private GameObject MaskedImageGameObject(string reason, GameObject parent, Rect rect, string spriteFolder, string spriteName, FSColor color, float alpha)
        {
            // outer mask
            var mask = FindGameObject(reason + " Mask", parent);
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
            spriteRenderer.color = color.ToColor(alpha);
            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            Vector3 spriteSize = spriteRenderer.sprite.bounds.size;
            Transform transform = image.GetComponent<RectTransform>().transform;
            float dimension = fitToSmallerSize ? Mathf.Max(spriteSize.x, spriteSize.y) : Mathf.Min(spriteSize.x, spriteSize.y);
            float maskScaleX = rect.height > rect.width ? rect.width / rect.height : 1;
            float maskScaleY = rect.width > rect.height ? rect.height / rect.width : 1;
            float spriteScaleX = spriteSize.y > spriteSize.x ? spriteSize.y / spriteSize.x : spriteSize.x / spriteSize.y;
            float spriteScaleY = spriteSize.x > spriteSize.y ? spriteSize.x / spriteSize.y : spriteSize.y / spriteSize.x;
            transform.localScale = new Vector3(spriteScaleX / (dimension * maskScaleX), spriteScaleY / (dimension * maskScaleY), 1);
            return mask;
        }

        public Creator MaskedImage(string reason, Rect rect, string spriteFolder, string spriteName, FSColor color, float alpha)
        {
            MaskedImageGameObject(reason, gameobject, rect, spriteFolder, spriteName, color, alpha);
            return this;
        }
    }
}
