using System;
using System.Collections;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private GameObject instantiated;
    public GameObject Empty;

    public void Spawn(GameObject go)
    {
        // for some reason there is too much objects
        for (int i = transform.childCount - 1; i >= 2; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // we do not need to spawn anything, just reasign the reference
        if (transform.childCount > 1)
        {
            instantiated = transform.GetChild(transform.childCount - 1).gameObject;
        }
        // there is no child except for empty slot that we could assign
        else
        {
            instantiated = Instantiate(go, transform);
            instantiated.transform.position = transform.position;
            instantiated.SetActive(true);
            Empty.SetActive(false);
        }
    }

    public void Destroy()
    {
        // destroy all objects that are not an empty slot
        for (int i = transform.childCount - 1; i >= 1; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        Empty.SetActive(true);
        instantiated = null;
    }
}
    