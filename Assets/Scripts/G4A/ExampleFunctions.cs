using UnityEngine;
using game4automation;
using TMPro;



public class ExampleFunctions : MonoBehaviour
{
    [Header("Factory Interfaces")]
    public OPCUA_Interface[] opcuaInterfaces;
    public TMP_Text test;

    string string1 = "";

    private void Start()
    {
        CheckConnectionWithMachines();
    }
    //runs through all of the factory interfaces checking for connection
    public void CheckConnectionWithMachines()
    {
        test.SetText(string1); //rewrite message to nothing


        for (int i = 0; i < opcuaInterfaces.Length; i++)             
        {
            if (opcuaInterfaces[i].IsConnected)               
            {
                test.text = "Good connection to interface " + i + ".";
            }
            else                                     
            {
                test.text = "No connection to interface " + i + ".";
            }
        }
    }
}
