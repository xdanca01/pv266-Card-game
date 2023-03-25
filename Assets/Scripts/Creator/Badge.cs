using UnityEngine;

public class Badge : MonoBehaviour
{
    private GameObject gameobject;
    private Creator creator;

    public static Badge New(Creator creator, uint count, FSColor color)
    {
        var gameobject = creator.FindGameObject("Badge");
        var badge = gameobject.AddComponent<Badge>();
        badge.gameobject = gameobject;
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