using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; private set; }

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

    [SerializeField] public GameObject Islands;
    [SerializeField] public GameObject Trader;
    [SerializeField] public GameObject Battlefield;
    [SerializeField] public GameObject BattleUI;
    [SerializeField] public GameObject IslandsUI;
    void Start()
    {
        Battlefield.SetActive(false);
        Trader.SetActive(false);
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
    }

    public void CameraIslands()
    {
        Battlefield.SetActive(false);
        Trader.SetActive(false);
        Islands.SetActive(true);
        IslandsUI.SetActive(true);
        BattleUI.SetActive(false);
    }

    public void CameraTrader()
    {
        Battlefield.SetActive(false);
        Trader.SetActive(true);
        Islands.SetActive(false);
        IslandsUI.SetActive(false);
        BattleUI.SetActive(false);
    }
    public void CameraBattlefield()
    {
        Battlefield.SetActive(true);
        Trader.SetActive(false);
        Islands.SetActive(false);
        IslandsUI.SetActive(false);
        BattleUI.SetActive(true);
    }
}
