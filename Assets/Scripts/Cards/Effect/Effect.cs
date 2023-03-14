using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour, IEffect
{
    public abstract EffectApplication Condition { get; }

    public abstract void Affect(IUnit host);
}
