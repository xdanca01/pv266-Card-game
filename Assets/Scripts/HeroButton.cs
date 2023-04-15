using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeroButton : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI Name;
    [SerializeField] public TextMeshProUGUI HP;
    public void SetPrefabData(string name, string HP)
    {
        this.Name.text = name;
        this.HP.text = HP;
    }
}
