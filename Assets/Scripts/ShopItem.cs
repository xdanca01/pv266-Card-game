using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI Name;
    [SerializeField] public GameObject HP;
    [SerializeField] public TextMeshProUGUI Price;
    
    public void InitItem(string name, string? hp, string price)
    {
        Name.text = name;
        int cost = 5;
        HP.SetActive(false);
        if (hp != null)
        {
            cost = 10;
            HP.SetActive(true);
            TextMeshProUGUI life = HP.GetComponentInChildren<TextMeshProUGUI>();
            life.text = hp;
        }
        Price.text = price;
    }
}
