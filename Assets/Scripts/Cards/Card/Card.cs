using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    public CardVisual CardVisual;
    public CardIcon IconVisual;

    public void OnValidate()
    {
        if (CardVisual != null)
        {
            CardVisual.OnValidate();
        }
        if (IconVisual != null)
        {
            IconVisual.OnValidate();
        }
    }
}
