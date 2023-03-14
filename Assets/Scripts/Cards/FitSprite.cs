using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitSprite : MonoBehaviour
{
    public bool fitToMaxSize;

    public void OnValidate()
    {
        Vector3 spriteSize = GetComponent<SpriteRenderer>().sprite.bounds.size;
        Transform transform = GetComponent<RectTransform>().transform;
        float maxDimension = fitToMaxSize ? Mathf.Min(spriteSize.x, spriteSize.y) : Mathf.Max(spriteSize.x, spriteSize.y);
        transform.localScale = new Vector3(1/maxDimension, 1/maxDimension, 1);
    }
}
