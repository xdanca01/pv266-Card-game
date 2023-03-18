using UnityEngine;

public partial class Card
{
    public partial class Creator
    {
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
    }
}
