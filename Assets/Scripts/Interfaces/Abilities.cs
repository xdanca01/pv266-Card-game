using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Abilities : MonoBehaviour
{
    public string ArtworkFolder;
    public string IconFolder;

    private Dictionary<string, Sprite> artworks;
    private Dictionary<string, Sprite> icons;

    public GameObject AbilityPrefab;

    [TextArea(10, 20)]
    public string data;

    private static string GetColumn(string columnName, string[] columns, string[] columnNames)
    {
        foreach ((string name, string value) in columnNames.Zip(columns, (a,b)=>(a,b)))
        {
            if (name.Equals(columnName))
            {
                return value;
            }
        }
        throw new System.Exception("Can't find column " + columnName);
    }

    [EditorCools.Button]
    private void RegenerateCards()
    {
        Debug.Log("Generating...");
        StartCoroutine(RegenerateAllCards());
    }

    IEnumerator RegenerateAllCards()
    {
        yield return null;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        string[] array = data.Split('\n');
        string[] columnNames = array[0].Trim().Split('\t');
        for (int i = 1; i < array.Length; i++)
        {
            string line = array[i].Trim();
            if (line.Length == 0)
            {
                continue;
            }
            string[] columns = line.Split('\t');
            GameObject prefab = Instantiate(AbilityPrefab);
            prefab.name = GetColumn("Title", columns, columnNames);
            prefab.transform.parent = transform;
            prefab.transform.position = new Vector3(transform.position.x + 7 * (i - 1), transform.position.y, transform.position.z);
            
           /* Ability ability = prefab.GetComponentInChildren<Ability>();

            ability.Percentage = uint.Parse(GetColumn("Percentage", columns, columnNames)[..^1]);
            ability.Low = uint.Parse(GetColumn("Low", columns, columnNames));
            ability.High = uint.Parse(GetColumn("High", columns, columnNames));

            ability.visual.title = prefab.name;
            ability.visual.value = uint.Parse(GetColumn("Value", columns, columnNames));
            string iconName = GetColumn("Icon", columns, columnNames);
            Sprite icon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/"+IconFolder+"/"+ iconName + ".png");
            ability.visual.artwork = icon;
            ability.icon.badge = icon;

            ability.OnValidate();*/
        }
        Debug.Log("Done!");
    }
}