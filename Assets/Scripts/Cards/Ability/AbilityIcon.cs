using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AbilityIcon : MonoBehaviour
{
    private static readonly Color FastAttackColor = new(0.75f, 0.625f, 0, 0.5f);
    private static readonly Color SlowAttackColor = new(0.75f, 0.375f, 0, 0.5f);
    private static readonly Color HealColor = new(0.75f, 0, 0, 0.5f);
    
    public TextMeshProUGUI percentage;
    public TextMeshProUGUI range;
    public SpriteRenderer badge;
    public Ability ability;

    public void OnValidate()
    {
        percentage.text = ability.GetPercentageString();
        range.text = ability.GetRangeString();
        badge.sprite = ability.Badge;
        badge.color = ability.Type switch
        {
            AbilityType.FastAttack => FastAttackColor,
            AbilityType.SlowAttack => SlowAttackColor,
            AbilityType.Heal => HealColor,
            _ => throw new System.NotImplementedException(),
        };
    }
}
