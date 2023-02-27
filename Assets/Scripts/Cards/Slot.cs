using System;
using System.Collections;
using UnityEngine;

public class Slot : MonoBehaviour
{
    private GameObject instantiated;
    public GameObject Empty;
    
    public void Spawn(GameObject go)
    {
        if (instantiated == null)
        {
            instantiated = Instantiate(go, transform);
            instantiated.transform.position = transform.position;
            instantiated.SetActive(true);
            Empty.SetActive(false);
        }
    }

    public void Destroy()
    {
        if (instantiated != null) 
        {
            Empty.SetActive(true);
            DestroyImmediate(instantiated);
        }
    }
}
