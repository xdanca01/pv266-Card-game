using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopupInfoControll : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _difficulty; 
    [SerializeField] private TextMeshProUGUI _prize;
    [SerializeField] private TextMeshProUGUI _enemies;
    [SerializeField] private TextMeshProUGUI _size;
    public void UpdateInfo(int row, int col, string difficulty, string prize, int enemies)
    {
        _difficulty.SetText("Difficulty: " + difficulty);
        _prize.SetText("Prize: " + prize);
        _enemies.SetText("Enemies: " + enemies.ToString());
        _size.SetText("Size: " + row.ToString() + " x " + col.ToString());
    }
}
