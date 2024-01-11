using UnityEngine;
using UnityEngine.UI;
using game4automation;
using TMPro;

public class OPCUA_IPPortChanger : MonoBehaviour
{
    public OPCUA_Interface Interface;
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;

    void Start()
    {
        if (Interface == null)
        {
            Debug.LogError("OPCUA_Interface component is not assigned in the Inspector.");
        }
    }

    public void ChangeIP()
    {
        if (Interface != null && ipInputField != null)
        {
            Interface.ServerIP = ipInputField.text;
        }
    }

    public void ChangePort()
    {
        if (Interface != null && portInputField != null)
        {
            int newPort;
            if (int.TryParse(portInputField.text, out newPort))
            {
                Interface.ServerPort = newPort;
            }
            else
            {
                Debug.LogError("Invalid port number input.");
            }
        }
    }

    public void ConnectToServer()
    {
        ChangeIP();
        ChangePort();
        Interface.AttemptConnection();
    }
}
