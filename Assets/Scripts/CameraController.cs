using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; private set; }
    [SerializeField] public Camera BattlefieldCamera;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField] public GameObject Tutorial;
    [SerializeField] public GameObject Islands;
    [SerializeField] public GameObject TraderCam;
    [SerializeField] public GameObject Battlefield;
    [SerializeField] public GameObject BattleUI;
    [SerializeField] public GameObject IslandsUI;
    [SerializeField] private Generator generator;
    int cnt = 0;

    void Start()
    {
        Battlefield.SetActive(false);
        TraderCam.SetActive(false);
        Islands.SetActive(true);
        IslandsUI.SetActive(true);
        BattleUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CameraIslands();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Rewards.instance.GiveReward(Difficulty.Extreme);
        }
    }

    public void CameraIslands()
    {
        if(cnt >= 3)
        {
            Islands.SetActive(true);
            Tutorial.SetActive(false);
        }
        else
        {
            Islands.SetActive(false);
            Tutorial.SetActive(true);
            ++cnt;
        }
        
        Trader.instance.DisableTrader();
        Battlefield.SetActive(false);
        TraderCam.SetActive(false);
        
        IslandsUI.SetActive(true);
        BattleUI.SetActive(false);
        generator.gameObject.SetActive(false);
    }

    public void CameraTrader()
    {
        Trader.instance.EnableTrader();
        Tutorial.SetActive(false);
        Battlefield.SetActive(false);
        TraderCam.SetActive(true);
        Islands.SetActive(false);
        IslandsUI.SetActive(false);
        BattleUI.SetActive(false);
        generator.gameObject.SetActive(false);
    }
    public void CameraBattlefield()
    {
        Trader.instance.DisableTrader();
        Tutorial.SetActive(false);
        Battlefield.SetActive(true);
        TraderCam.SetActive(false);
        Islands.SetActive(false);
        IslandsUI.SetActive(false);
        BattleUI.SetActive(true);
        generator.gameObject.SetActive(true);
    }
}
