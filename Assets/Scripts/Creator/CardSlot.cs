using UnityEngine;
using UnityEngine.UIElements;

public partial class Card
{
    public partial class Creator
    {
        public class CardSlot
        {
            private readonly GameObject gameobject;
            private GameObject unit;
            private GameObject upgrade;
            private readonly Creator creator;

            public CardSlot(string reason, GameObject parent, Vector2 position)
            {
                this.creator = new Card.Creator(reason, parent).Background();
                this.gameobject = this.creator.gameobject;
                this.gameobject.transform.position = position;
            }

            // if icon = null then slot will become empty
            private void SetUnit(Unit unit)
            {
                if (this.unit != null)
                {
                    DestroyImmediate(this.unit);
                }
                if (unit == null)
                {
                    gameobject.SetActive(upgrade == null);
                    this.unit = null;
                }
                else
                {
                    gameobject.SetActive(false);
                    this.unit = unit.Card.gameobject;
                    this.unit.transform.position = this.gameobject.transform.position;
                }
            }

            private void SetUpgrade(Upgrade upgrade)
            {
                if (this.upgrade != null)
                {
                    DestroyImmediate(this.upgrade);
                }
                if (upgrade == null)
                {
                    gameobject.SetActive(unit == null);
                    this.upgrade = null;
                }
                else
                {
                    gameobject.SetActive(false);
                    this.upgrade = upgrade.Card.gameobject;
                    this.upgrade.transform.position = this.gameobject.transform.position;
                }
            }

        }
    }
}
