using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public GameObject camera1;
    public GameObject camera2;
    public bool kld;
    public GameObject dofText;
    public GameObject kldText;
    public TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void modeSwitch()
    {
        if (kld == true)
        {
            camera1.SetActive(false);
            camera2.SetActive(true);
            kldText.SetActive(false);
            dofText.SetActive(true);
            kld = false;
            text.text = "¥¨";
        }
        else
        {
            camera2.SetActive(false);
            camera1.SetActive(true);
            kldText.SetActive(true);
            dofText.SetActive(false);
            kld = true;
            text.text = "øÀ¡Óµı";
        }
    }
}