using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public interface IEffect
{
    // applied only once, after it is applied
    void Once(IUnit self) { }
    // applied at the start of each round
    void RoundStart(IUnit self) { }
    // applied at the end of each round
    void RoundEnd(IUnit self) { }
    // applied after successfully using some ability 
    void AbilitySuccess(IUnit self, IUnit target) { }
    // applied after successfully using some attack 
    void AttackSuccess(IUnit self, IUnit target) { }
    // applied after successfully using fast attack
    void LightAttackSuccess(IUnit self, IUnit target) { }
    // applied after successfully using slow attack
    void HeavyAttackSuccess(IUnit self, IUnit target) { }
    // applied after successfully using heal
    void HealSuccess(IUnit self, IUnit target) { }
    // applied after failing using any ability 
    void AbilityFail(IUnit self, IUnit target) { }
    // applied after failing using any attack 
    void AttackFail(IUnit self, IUnit target) { }
    // applied after failing using fast attack
    void LightAttackFail(IUnit self, IUnit target) { }
    // applied after failing using slow attack
    void HeavyAttackFail(IUnit self, IUnit target) { }
    // applied after failing using heal
    void HealFail(IUnit self, IUnit target) { }
    // applied after sucessful move
    void Move(IUnit self) { }
}

public class NoEffect : IEffect
{

}

public class DoubleAttack : IEffect
{
    public void RoundStart(IUnit self)
    {
        foreach (IAbility ability in self.Abilities)
        {
            ability.High *= 2;
            ability.Low *= 2;
        }
    }
    public void RoundEnd(IUnit self)
    {
        foreach (IAbility ability in self.Abilities)
        {
            ability.High /= 2;
            ability.Low /= 2;
        }
    }
    public void Once(IUnit self)
    {
        RoundStart(self);
    }
}

public class HealingSpring : IEffect
{
    public void RoundStart(IUnit self)
    {
        self.HP = Math.Min(self.HP + 5, self.MAX_HP);
        
    }
}

public class Poisoned : IEffect
{
    public void RoundStart(IUnit self)
    {
        self.HP -= Math.Max(self.HP - 5, 0);
    }
}

public class Poison : IEffect
{
    public void AbilitySuccess(IUnit self, IUnit target)
    {
        target.ApplyEffect(Deck.instance.upgrades["Poison"]); // TODO
    }
}