using UnityEngine;
using game4automation;
using TMPro;
using System.Threading;
using System.Collections;
using System.Collections.Generic;



public class Connection : MonoBehaviour
{
    [Header("Factory Interfaces")]
    public OPCUA_Interface[] opcuaInterfaces;
    public TMP_Text connectionText;

    string string1 = "";

    public void CheckConnectionWithMachines()
    {
        connectionText.SetText(string1); //rewrite message to nothing

        for (int i = 0; i < opcuaInterfaces.Length; i++)
        {
            if (opcuaInterfaces[i].IsConnected)
            {
                StartCoroutine(CCoroutine());
            }
            else
            {
                StartCoroutine(DCoroutine());
            }
        }   
    }
    public IEnumerator CCoroutine()
    {
        connectionText.enabled=true;
        connectionText.text = "Connection successful!!";
        yield return new WaitForSeconds(3f);
        connectionText.enabled=false;

    }

    public IEnumerator DCoroutine()
    {
        connectionText.enabled = true;
        connectionText.text = "No connection to OPC Server"; 
        yield return new WaitForSeconds(3f);
        connectionText.enabled = false;

    }
}

