using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ItemSlot<I, S> : MonoBehaviour where I: Interactable where S: ItemSlot<I, S>
{
    private GameObject empty;
    private Creator creator;
    protected Icon icon;
    protected I interactible;

    public static S New(Creator creator, string reason, GameObject parent, bool pointedUp, Vector2 position)
    {
        var gameobject = creator.FindGameObject(reason, parent);
        var itemSlot = gameobject.AddComponent<S>();
        itemSlot.creator = creator;
        creator.SetRect(gameobject, new Rect(position.x, position.y, 2, 2));
        itemSlot.empty = creator.Hexagon("Empty", gameobject, FSColor.DarkGray, pointedUp);
        itemSlot.empty.transform.position = new Vector3(position.x, position.y, itemSlot.empty.transform.position.z);
        var child = creator.Hexagon("Child", itemSlot.empty, FSColor.LightGray, pointedUp);
        child.GetComponent<RectTransform>().localScale = new Vector3(1f / 3f, 1f / 3f, 1f);
        return itemSlot;
    }

    // if icon = null then slot will become empty
    public void Set(I interactible)
    {
        if (icon != null)
        {
            Destroy(icon.gameObject);
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
            icon = interactible.Icon.FreshCopy(gameObject);
            this.interactible = interactible;
        }
    }
    public I Get()
    {
        return interactible;
    }
}