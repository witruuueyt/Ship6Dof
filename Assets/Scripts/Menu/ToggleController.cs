using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleController : MonoBehaviour
{
    public GameObject object1;

    void Start()
    {
       
        if (object1 != null)
        {
            object1.SetActive(false);
        }
    }

    
    public void Toggle()
    {
       
        if (object1 != null)
        {
            object1.SetActive(!object1.activeSelf);
        }
    }
}
