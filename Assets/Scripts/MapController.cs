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

    public void SetBattlefieldCamera()
    {
        var camera = CameraController.instance.BattlefieldCamera;
        camera.orthographicSize = Generator.WorldHeight / 2f;
        camera.transform.position = new Vector3(Generator.WorldWidth / 2f, Generator.WorldHeight / 2f, camera.transform.position.z);
        CameraController.instance.CameraBattlefield();
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
            generator.GetComponent<Generator>().CreateOnlyBattlefield(CurrentIsland.IslandName);
            SetBattlefieldCamera();
        }
    }

    public void ChangeLoop()
    {
        GameObject.FindGameObjectWithTag("Generator").GetComponent<Generator>().battlefield.NextRound();
        ++loop;
        _loopText.SetText("TURN " + loop.ToString());
    }
}
