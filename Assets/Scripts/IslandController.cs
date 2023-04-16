using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum IslandType
{
    Trader,
    Fight,
    Boss
}

public class IslandController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler ,IPointerClickHandler
{
    public bool ActiveIsland { get; set; }
    public bool IslandCanBeNext { get; set; }
    [SerializeField] public IslandType typeOfIsland;
    [SerializeField] public String IslandName;
    [SerializeField] private bool _generateRandomVisual;
    [SerializeField] private Sprite _islandShape;
    [SerializeField] private Sprite _islandTexture;

    [SerializeField] private Image _islandType;

    [SerializeField] private Sprite[] _allShapes;
    [SerializeField] private Sprite[] _allTextures;

    [SerializeField] private LineRenderer _lineRendererPrefab;
    
    [SerializeField] private IslandController[] _nextIslands;

    [SerializeField] private GameObject _isladInfo;
    [SerializeField] private GameObject _activeCircle;

    [SerializeField] private StringDataSO _islandNames;

    [SerializeField] private TextMeshProUGUI _islandNameText;

    [SerializeField] private PopupInfoControll _popupInfo;

    private GameObject _goButton;

    private Battlefield _battlefield;
    
    private LineRenderer[] _nextIsladsLineRenderer;

    private SpriteMask _spriteMask;
    private SpriteRenderer _islandShapeImage;
    private SpriteRenderer _islandTextureImage;

    private void Awake()
    {
        _islandShapeImage = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _islandTextureImage = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();

        _spriteMask = transform.GetChild(0).GetComponent<SpriteMask>();
        var GoButton = this.transform.GetComponentInChildren<Button>(true);
        _goButton = GoButton.gameObject;
    }
    private void OnEnable()
    {
        MapController.OnGenerateIslands += GenerateIsland;
        //_battlefield = new Generator().CreateBattlefield(IslandName);
    }

    private void OnDisable()
    {
        MapController.OnGenerateIslands -= GenerateIsland;
    }

    private void Update()
    {
        if (ActiveIsland)
        {
            showButtonInChild();
            _activeCircle.SetActive(true);
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
            _activeCircle.SetActive(false);
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

        if(IslandName != "")
        {
            _islandNameText.text = IslandName;
        }
        else
        {
            var islandName = _islandNames.GetRandomName();
            _islandNameText.text = islandName;
        }
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
        _islandType.color = Color.red;
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _islandType.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject[] Islands = GameObject.FindGameObjectsWithTag("Island");
        foreach(var i in Islands)
        {
            i.GetComponent<IslandController>().HidePopup();
        }
        _isladInfo.SetActive(true);
    }

    public void HidePopup()
    {
        _isladInfo.SetActive(false);
    }

    public void StartBattle()
    {
        GameObject map = GameObject.FindGameObjectsWithTag("MapController")[0];
        MapController controller = map.GetComponent<MapController>();
        controller.StartBattle(this);
        //TODO create battlefield
    }

    public bool IsPreviousFor(IslandController nextIsland)
    {
        foreach(var island in _nextIslands)
        {
            if(island == nextIsland)
            {
                return true;
            }
        }
        return false;
    }

    public void showButton()
    {
        //_goButton.SetActive(true);
        _goButton.GetComponent<Button>().interactable = true;
    }
    public void hideButton()
    {
        //_goButton.SetActive(false);
        _goButton.GetComponent<Button>().interactable = false;
    }

    public void showButtonInChild()
    {
        GameObject[] Islands = GameObject.FindGameObjectsWithTag("Island");
        foreach (var i in Islands)
        {
            i.GetComponent<IslandController>().hideButton();
        }
        foreach(var next in _nextIslands)
        {
            next.showButton();
        }
    }

    public void UpdatePopupInfo()
    {
        //TODO get battlefield from somewhere
        int row = _battlefield.EnemySlots.GetLength(0);
        int col = _battlefield.EnemySlots.GetLength(1);
        string difficulty = _battlefield.GetDifficulty();
        int enemies = _battlefield.CountEnemies();
        _popupInfo.UpdateInfo(row, col, difficulty, "?", enemies);
    }
}
