using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : IEffect
{
    public EffectApplication Condition => EffectApplication.AttackSuccess;

    public void Affect(IUnit host)
    {
        //new Poisoned().Affect(host.Target)
        throw new NotImplementedException();
    }

}
