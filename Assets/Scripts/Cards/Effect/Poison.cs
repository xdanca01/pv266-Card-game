using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : Effect
{
    public override EffectApplication Condition => EffectApplication.AttackSuccess;

    public override void Affect(IUnit host)
    {
        host.HP -= 3; // TODO!
    }
}
