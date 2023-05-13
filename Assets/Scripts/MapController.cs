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
    [SerializeField] private Generator generator;
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
        foreach (var island in _startIsland._nextIslands)
        {
            island.IslandCanBeNext = true;
        }
        StartCoroutine(LateStart(0.1f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Your Function You Want to Call
        StartBattle(CurrentIsland);
    }

    public void SetBattlefieldCamera(Battlefield battlefield, bool hasPlacementSlots)
    {
        var camera = CameraController.instance.BattlefieldCamera;
        var rows = Math.Max(battlefield.AllySlots.GetLength(0), battlefield.EnemySlots.GetLength(0)) + (hasPlacementSlots ? 1 : 0);
        var cols = battlefield.AllySlots.GetLength(1) + battlefield.EnemySlots.GetLength(1);
        var midX = (cols / 2.0f + 0.75f) * Generator.ColumnSize;
        var midY = (-rows / 2.0f - 1.5f) * Generator.RowSize;
        var height = rows * Generator.RowSize;
        var width = (cols + 0.5f) * Generator.ColumnSize / camera.aspect;
        var border = 1.2f; // 20% on the sides
        camera.orthographicSize = border * (Mathf.Max(width, height) / 2.0f);
        camera.transform.position = new Vector3(midX, midY, camera.transform.position.z);
        CameraController.instance.CameraBattlefield();
    }

    public void StartBattle(IslandController newIsland)
    {
        CurrentIsland.ActiveIsland = false;
        newIsland.ActiveIsland = true;
        foreach(var island in CurrentIsland._nextIslands)
        {
            island.IslandCanBeNext = false;
        }
        foreach (var island in newIsland._nextIslands)
        {
            island.IslandCanBeNext = true;
        }
        CurrentIsland = newIsland;
        if (newIsland.name == "Aban")
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
            var battlefield = generator.CreateOnlyBattlefield(CurrentIsland.IslandName);
            SetBattlefieldCamera(battlefield, true);
        }
    }

    public void ChangeLoop()
    {
        generator.gameObject.SetActive(true);
        generator.battlefield.NextRound();
        ++loop;
        _loopText.SetText("TURN " + loop.ToString());
    }

    public void SetActiveAndNext(IslandController newIsland)
    {

    }
}
