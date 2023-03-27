using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler
{
    private GameObject empty;
    private Creator creator;
    private GameObject icon;
    private Interactable interactible;

    public static ItemSlot New(Creator creator, string reason, GameObject parent, bool pointedUp, Vector2 position)
    {
        var gameobject = creator.FindGameObject(reason, parent);
        var itemSlot = gameobject.AddComponent<ItemSlot>();
        itemSlot.creator = creator;
        creator.SetRect(gameobject, new Rect(position.x, position.y, 2, 2));
        var box2D = creator.FindComponent<BoxCollider2D>(gameobject);
        box2D.size = new Vector2(2, 2);
        box2D.isTrigger = true;
        itemSlot.empty = creator.Hexagon("Empty", gameobject, FSColor.DarkGray, pointedUp);
        itemSlot.empty.transform.position = new Vector3(position.x, position.y, itemSlot.empty.transform.position.z);
        var child = creator.Hexagon("Child", itemSlot.empty, FSColor.LightGray, pointedUp);
        child.GetComponent<RectTransform>().localScale = new Vector3(1f / 3f, 1f / 3f, 1f);
        child.GetComponent<SpriteRenderer>().sortingOrder = 1;
        return itemSlot;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter: " + gameObject.name);
    }

    // if icon = null then slot will become empty
    public void Set(Interactable interactible)
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
            icon = interactible.Icon.Create(gameObject);
            this.interactible = interactible;
        }
    }
    public Interactable Get()
    {
        return interactible;
    }
}