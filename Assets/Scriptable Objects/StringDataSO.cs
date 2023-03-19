using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/String Data")]
public class StringDataSO : ScriptableObject
{
    public List<string> data;

    private List<string> _dataLotery;

    public string GetRandomName()
    {
        if (_dataLotery == null || _dataLotery.Count == 0)
            _dataLotery = new List<string>(data);

        var randomIndex = Random.Range(0, _dataLotery.Count);
        var result = _dataLotery[randomIndex];
        _dataLotery.RemoveAt(randomIndex);

        return result;
    }
}
