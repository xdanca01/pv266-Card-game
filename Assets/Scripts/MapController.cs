using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class MapController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _loopText;
    [SerializeField] private bool _randomSeed;
    [SerializeField] private int _seed;
    [SerializeField] private Camera battlefieldCamera;
    public static event Action<int> OnGenerateIslands;
    int loop = 1;

    [field: SerializeField] public IslandController CurrentIsland { get; private set; }
    private IslandController _startIsland;
    void Start()
    {
        if(_randomSeed)
            OnGenerateIslands?.Invoke(Random.Range(0,99999));
        else
            OnGenerateIslands?.Invoke(_seed);
        CurrentIsland.ActiveIsland = true;
        _startIsland = CurrentIsland;
    }

    public void StartBattle(IslandController newIsland)
    {
        //Cant go to this island
        if (CurrentIsland.IsPreviousFor(newIsland) == false)
        {
            return;
        }
        newIsland.ActiveIsland = true;
        CurrentIsland.ActiveIsland = false;
        CurrentIsland = newIsland;
        if (newIsland == _startIsland)
        {
            ChangeLoop();
        }
        if (CurrentIsland.typeOfIsland == IslandType.Trader)
        {
            Trader.instance.Generate();
            CameraController.instance.CameraTrader();
        }
        else
        {
            GameObject generator = GameObject.FindGameObjectWithTag("Generator");
            var battlefield = generator.GetComponent<Generator>().CreateBattlefield(CurrentIsland.IslandName);
            var rows = Math.Max(battlefield.AllySlots.GetLength(0), battlefield.EnemySlots.GetLength(0));
            var cols = battlefield.AllySlots.GetLength(1) + battlefield.EnemySlots.GetLength(1);
            var midX = (cols / 2 + 0.75f) * Generator.ColumnSize;
            var midY = (-rows / 2 - 2f) * Generator.RowSize;
            var height = rows * Generator.RowSize;
            var width = (cols + 0.5f) * Generator.ColumnSize / battlefieldCamera.aspect;
            var borderSize = 0.5f;
            battlefieldCamera.enabled = true;
            battlefieldCamera.orthographicSize = Mathf.Max(width, height) / 2 + borderSize;
            battlefieldCamera.transform.position = new Vector3(midX, midY, battlefieldCamera.transform.position.z);
        }   
    }

    public void ChangeLoop()
    {
        GameObject.FindGameObjectWithTag("Generator").GetComponent<Generator>().battlefield.NextRound();
        ++loop;
        _loopText.SetText("TURN " + loop.ToString());
    }
}
