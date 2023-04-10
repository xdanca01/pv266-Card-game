using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI Name;
    [SerializeField] public GameObject Icon;
    public void SetPrefabData(string name)
    {
        Name.text = name;
        //Image tmp = Icon.GetComponent<Image>();
        //tmp = icon;
    }
}
