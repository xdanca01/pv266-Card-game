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
        if(CurrentIsland.IsPreviousFor(newIsland) == false)
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
        GameObject Generator = GameObject.FindGameObjectWithTag("Generator");
        Generator.GetComponent<Generator>().CreateBattlefield(CurrentIsland.IslandName);
        battlefieldCamera.gameObject.SetActive(true);
    }

    private void ChangeLoop()
    {
        ++loop;
        _loopText.SetText("TURN " + loop.ToString());
    }
}
