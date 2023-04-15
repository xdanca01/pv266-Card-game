using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    public Icon Icon { get; }
    public Creator Card { get; }
    public Background Background { get; }
}
