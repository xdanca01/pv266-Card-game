using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public abstract class CardIcon : MonoBehaviour
{
    public abstract string MainText { get; }
    public abstract string AdditionalText { get; }
    public abstract Sprite Badge { get; }
    public abstract Color BadgeColor { get; }

    [Serializable]
    public struct VisualData
    {
        public TextMeshProUGUI MainText;
        public TextMeshProUGUI AdditionalText;
        public SpriteRenderer Badge;
    }

    [SerializeField]
    private VisualData visual;

    public void OnValidate()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        visual.MainText.text = MainText;
        visual.AdditionalText.text = AdditionalText;
        visual.Badge.color = BadgeColor;
        // will throw warning if set directly
        StartCoroutine(SetBadge());
    }

    private IEnumerator SetBadge()
    {
        yield return null;
        visual.Badge.sprite = Badge;
    }
}
