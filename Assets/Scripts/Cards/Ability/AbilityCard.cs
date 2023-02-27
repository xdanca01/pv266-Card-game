using TMPro;
using UnityEngine;

public class AbilityCard : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Description;
    public SpriteRenderer Artwork;
    public Ability ability;

    public void OnValidate()
    {
        Name.text = ability.Name;
        Description.text = ability.GetPercentageString() + " " + ability.GetRangeString() + " " + ability.Type switch
        {
            AbilityType.FastAttack => "Light DMG",
            AbilityType.SlowAttack => "Heavy DMG",
            AbilityType.Heal => "Heal",
            _ => throw new System.NotImplementedException(),
        };
        Artwork.sprite = ability.Artwork;
    }
}
