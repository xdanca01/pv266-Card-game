using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class IslandController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool ActiveIsland { get; set; }
    public bool IslandCanBeNext { get; set; }
    [SerializeField] private bool _generateRandomVisual;
    [SerializeField] private Sprite _islandShape;
    [SerializeField] private Sprite _islandTexture;

    [SerializeField] private Sprite[] _allShapes;
    [SerializeField] private Sprite[] _allTextures;

    [SerializeField] private LineRenderer _lineRendererPrefab;
    
    [SerializeField] private IslandController[] _nextIslands;

    [SerializeField] private GameObject _isladInfo;

    [SerializeField] private StringDataSO _islandNames;

    [SerializeField] private TextMeshProUGUI _islandNameText;
    
    private LineRenderer[] _nextIsladsLineRenderer;

    private SpriteMask _spriteMask;
    private SpriteRenderer _islandShapeImage;
    private SpriteRenderer _islandTextureImage;

    private void Awake()
    {
          
        _islandShapeImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _islandTextureImage = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();

        _spriteMask = transform.GetChild(0).GetComponent<SpriteMask>();
    }
    private void OnEnable()
    {
        MapController.OnGenerateIslands += GenerateIsland;
    }

    private void OnDisable()
    {
        MapController.OnGenerateIslands -= GenerateIsland;
    }

    private void Update()
    {
        if (ActiveIsland)
        {
            foreach (var lineRenderer in _nextIsladsLineRenderer)
            {
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.green;
            }
            foreach (var nextIsland in _nextIslands)
            {
                nextIsland.IslandCanBeNext = true;
            }
        }
        else
        {
            foreach (var lineRenderer in _nextIsladsLineRenderer)
            {
                lineRenderer.startColor = Color.white;
                lineRenderer.endColor = Color.white;
            }
            foreach (var nextIsland in _nextIslands)
            {
                nextIsland.IslandCanBeNext = false;
            }
        }
    }
    private void GenerateIsland(int seed)
    {
        Random.InitState(seed + gameObject.GetInstanceID());
        ChangeIslandLook();
        GenerateLines();

        var islandName = _islandNames.GetRandomName();
        _islandNameText.text = islandName;
    }

    private void GenerateLines()
    {
        var lineList = new List<LineRenderer>();
        foreach (var nextIsland in _nextIslands)
        {
            var line = Instantiate(_lineRendererPrefab);
            line.transform.parent = transform;
            line.widthMultiplier = 0.1f;
            line.SetPositions(new Vector3[] { transform.position, nextIsland.transform.position });
            lineList.Add(line);
            //line.gameObject.SetActive(false);
        }

        _nextIsladsLineRenderer = lineList.ToArray();
    }

    public void ChangeIslandLook()
    {
        var shape = _islandShape;
        var texture = _islandTexture;

        if (_generateRandomVisual)
        {
            shape = _allShapes[Random.Range(0, _allShapes.Length)];
            texture = _allTextures[Random.Range(0, _allTextures.Length)];
        }
        _islandShapeImage.sprite = shape;
        _islandTextureImage.sprite = texture;

        _spriteMask.sprite = shape;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isladInfo.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _isladInfo.SetActive(false);
    }
}
