using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapController : MonoBehaviour
{
    [SerializeField] private bool _randomSeed;
    [SerializeField] private int _seed;
    public static event Action<int> OnGenerateIslands;

    [field: SerializeField] public IslandController CurrentIsland { get; private set; }
    void Start()
    {
        if(_randomSeed)
            OnGenerateIslands?.Invoke(Random.Range(0,99999));
        else
            OnGenerateIslands?.Invoke(_seed);
        CurrentIsland.ActiveIsland = true;
    }
}
