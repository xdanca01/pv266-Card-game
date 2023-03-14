using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public abstract class CardVisual : MonoBehaviour
{
    public abstract string Title { get; }
    public abstract uint Value { get; }
    public abstract string Description { get; }
    public abstract Sprite Artwork { get; }

    [Serializable]
    public struct Visual
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Value;
        public TextMeshProUGUI Description;
        public SpriteRenderer Artwork;
    }

    [SerializeField]
    private Visual visual;

    public void OnValidate()
    {
        if (!isActiveAndEnabled)
        {
            return;
        }
        visual.Title.text = Title;
        visual.Value.text = Value.ToString();
        visual.Description.text = Description;
        // will throw warning if set directly
        StartCoroutine(SetArtwork());
    }

    private IEnumerator SetArtwork()
    {
        yield return null;
        visual.Artwork.sprite = Artwork;
    }
}
