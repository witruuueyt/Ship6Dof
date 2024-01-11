using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Connect : MonoBehaviour
{
    
    public TMP_Text ip;
    public TMP_Text port;

    public void CIP()
    {
        ip.text = "Good Connection";
    }

    public void DIP()
    {
        ip.text = "No Connection";
    }

    public void CPort()
    {
        port.text = "Good Connection";
    }

    public void DPort()
    {
        port.text = "No Connection";
    }
}
