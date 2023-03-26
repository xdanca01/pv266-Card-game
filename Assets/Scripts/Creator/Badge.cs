using UnityEngine;

public class Badge : MonoBehaviour
{
    private Creator creator;

    public static Badge New(Creator creator, uint count, FSColor color)
    {
        var gameobject = creator.FindGameObject("Badge");
        var badge = gameobject.AddComponent<Badge>();
        badge.creator = creator;
        badge.creator.SetRect(gameobject, new Rect(2f, 3.5f, 2f, 2f));
        badge.Color = color;
        badge.Count = count;
        return badge;
    }
    private FSColor color;
    public FSColor Color
    {
        get => color;
        set
        {
            color = value;
            creator.Hexagon("Hexagon", gameObject, color, true);
        }
    }
    private uint count;
    public uint Count
    {
        get => count;
        set
        {
            count = value;
            creator.Text("Text", gameObject, count.ToString(), new Rect(0f, 0f, 1.5f, 1.1f), FSFont.DeadRevolution);
        }
    }
}