using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class IslandVisual : MonoBehaviour
{
    [SerializeField] private bool _generateRandomVisual;
    [SerializeField] private Color _outlineColor;
    [SerializeField] private Sprite _islandShape;
    [SerializeField] private Sprite _islandTexture;
    [SerializeField, Range(1f, 5f)] private float _islandSize;
    [SerializeField, Range(0.01f, 1f)] private float _outlineThicnes;


    [SerializeField] private Sprite[] _allShapes;
    [SerializeField] private Sprite[] _allTextures;
    
    private Image _islandShapeImage;
    private Image _islandTextureImage;
    private Image _outlineShapeImage;
    private Image _outlineTextureImage;

    private void OnValidate()
    {
        _outlineShapeImage = transform.GetChild(0).GetComponent<Image>();
        _outlineTextureImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        
        _islandShapeImage = transform.GetChild(1).GetComponent<Image>();
        _islandTextureImage = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        
        _outlineShapeImage.transform.localScale = (Vector3.one * _islandSize) + (_outlineThicnes * Vector3.one);
        _islandShapeImage.transform.localScale = (Vector3.one * _islandSize);

        ChangeIslandLook();
    }
    private void ChangeIslandLook()
    {
        var shape = _islandShape;
        var texture = _islandTexture;

        if (_generateRandomVisual)
        {
            shape = _allShapes[Random.Range(0, _allShapes.Length)];
            texture = _allTextures[Random.Range(0, _allTextures.Length)];
        }
        
        _outlineShapeImage.sprite = shape;
        _islandShapeImage.sprite = shape;
        _islandTextureImage.sprite = texture;
        
        _outlineTextureImage.color = _outlineColor;
    }
}
