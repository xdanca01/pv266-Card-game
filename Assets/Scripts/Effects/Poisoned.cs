using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poisoned : IEffect
{
    public EffectApplication Condition => EffectApplication.RoundStart;

    public void Affect(IUnit host)
    {
        host.HP -= 3;
    }
}
