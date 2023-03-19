using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public Card.Creator.Icon Icon { get; }
    public Card.Creator Card { get; }
}
