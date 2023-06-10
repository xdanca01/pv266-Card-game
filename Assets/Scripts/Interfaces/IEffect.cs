using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public interface IEffect
{
    EffectType Type { get; }
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

public enum EffectType
{
    NoEffect,
    DoubleAttack,
    HealingSpring,
    Poisoned,
    Poison,
    LavaFloor,
    IceFloor,
    Immovable
}

public static class Effects
{
    private class NoEffect : IEffect
    {
        public EffectType Type => EffectType.NoEffect;
    }
    private class DoubleAttack : IEffect
    {
        public EffectType Type => EffectType.DoubleAttack;

        public void Once(IUnit self)
        {
            foreach (IAbility ability in self.Abilities)
            {
                if (ability.Type != AbilityType.Heal)
                {
                    ability.High *= 2;
                    ability.Low *= 2;
                }
            }
        }
    }

    private class IceFloor : IEffect
    { 
        public EffectType Type => EffectType.IceFloor;

        public void Once(IUnit self)
        {
            foreach (IAbility ability in self.Abilities)
            {
                ability.Percentage /= 2;
            }
        }
    }
    
    private class HealingSpring : IEffect
    {
        public EffectType Type => EffectType.HealingSpring;

        public void RoundStart(IUnit self)
        {
            self.HP = Math.Min(self.HP + 5, self.MAX_HP);
        }
    }

    private class Poisoned : IEffect
    {
        public EffectType Type => EffectType.Poisoned;

        public void RoundStart(IUnit self)
        {
            self.HP = (uint)Math.Max((int)self.HP - 3, 0);
        }
    }

    private class LavaFloor : IEffect
    {
        public EffectType Type => EffectType.Poisoned;

        public void RoundStart(IUnit self)
        {
            self.HP = (uint)Math.Max((int)self.HP - 3, 0);
        }
    }   

    private class Poison : IEffect
    {
        public EffectType Type => EffectType.Poison;

        public void AbilitySuccess(IUnit self, IUnit target)
        {
            target.ApplyEffect(Deck.instance.upgrades["poisoned"]); // TODO
        }
    }

    private class Immuvable : IEffect
    {
        public EffectType Type => EffectType.Immovable;
    }

    public static IEffect GetEffect(this EffectType effect) => effect switch
    {
        EffectType.NoEffect => new NoEffect(),
        EffectType.DoubleAttack => new DoubleAttack(),
        EffectType.HealingSpring => new HealingSpring(),
        EffectType.Poisoned => new Poisoned(),
        EffectType.Poison => new Poison(),
        EffectType.Immovable => new Immuvable(),
        EffectType.LavaFloor => new LavaFloor(),
        EffectType.IceFloor => new IceFloor(),
        _ => throw new NotImplementedException(effect.ToString()),
    };
}