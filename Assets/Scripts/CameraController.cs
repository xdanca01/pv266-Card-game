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

    [SerializeField] public Camera Islands;
    [SerializeField] public Camera Trader;
    [SerializeField] public Camera Battlefield;
    void Start()
    {
        Battlefield.enabled = false;
        Trader.enabled = false;
        Islands.enabled = true;
    }

    public void CameraIslands()
    {
        Battlefield.enabled = false;
        Trader.enabled = false;
        Islands.enabled = true;
    }

    public void CameraTrader()
    {
        Battlefield.enabled = false;
        Trader.enabled = true;
        Islands.enabled = false;
    }
    public void CameraBattlefield()
    {
        Battlefield.enabled = true;
        Trader.enabled = false;
        Islands.enabled = false;
    }
}
