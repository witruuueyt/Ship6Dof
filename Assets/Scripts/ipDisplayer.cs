using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using game4automation;
using TMPro;
using UnityEngine.UIElements;


public class ipDisplayer : MonoBehaviour
{
    public TMP_Text text;
    public TMP_Text text2;
    string serverIP;
    int serverPort;
    public OPCUA_Interface Interface;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateData", 0f, 1f);
        if (Interface != null)
        {
            serverIP = Interface.ServerIP;
            serverPort = Interface.ServerPort;
        }
        else
        {
            Debug.LogError("OPCUA_Interface component is not assigned in the Inspector.");
        }
    }

    // Update is called once per frame
    void UpdateData()
    {
        if (Interface != null)
        {
            serverIP = Interface.ServerIP;
            serverPort = Interface.ServerPort;
            text.text = "IP: " + serverIP;
            text2.text = "Port: " + serverPort.ToString();
        }
        else
        {
            Debug.LogError("No Connection to OPCUA Server.");
        }
        

    }
}
