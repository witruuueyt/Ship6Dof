using UnityEngine;
using game4automation;
using TMPro;
using UnityEngine.UIElements;
//using static UnityEditor.FilePathAttribute;

public class Ship6DoF : MonoBehaviour
{
    private string xPosition;
    private string yPosition;
    private string zPosition;

    private string xRotation;
    private string yRotation;
    private string zRotation;

    public Transform msXRotation;
    //public TMP_Text text;
    //public TMP_Text text2;
    //public Transform targetObjectTransform;

    private float refreshInterval = 0.1f;
    private float timeSinceLastRefresh = 0f;

    //string serverIP;
    //int serverPort;

    [Header("OPCUA")]
    public string factoryMachineID;
    public OPCUA_Interface Interface;
    public TMP_Text  uiFeedbackTMP;

    [Header("RX")]
    public string RXnodeID;
    public string RXValue;

    [Header("RY")]
    public string RYnodeID;
    public string RYValue;

    [Header("RZ")]
    public string RZnodeID;
    public string RZValue;

    [Header("X")]
    public string XnodeID;
    public string XValue;

    [Header("Y")]
    public string YnodeID;
    public string YValue;

    [Header("Z")]
    public string ZnodeID;
    public string ZValue;

    

    void Start()
    {
        Interface.EventOnConnected.AddListener(OnInterfaceConnected);
        Interface.EventOnConnected.AddListener(OnInterfaceDisconnected);
        Interface.EventOnConnected.AddListener(OnInterfaceReconnect);

        //if (Interface != null)
        //{
        //    serverIP = Interface.ServerIP;
        //    serverPort = Interface.ServerPort;
        //}
        //else
        //{
        //    Debug.LogError("OPCUA_Interface component is not assigned in the Inspector.");
        //}
    }


    private void OnInterfaceConnected()
    {


        //Debug.LogWarning("Connected to Factory Machine " + factoryMachineID);
        var RXsubscription = Interface.Subscribe(RXnodeID, RXNodeChanged);
        RXValue = RXsubscription.ToString();

        var RYsubscription = Interface.Subscribe(RYnodeID, RYNodeChanged);
        RYValue = RYsubscription.ToString();

        var RZsubscription = Interface.Subscribe(RZnodeID, RZNodeChanged);
        RZValue = RZsubscription.ToString();

        var Xsubscription = Interface.Subscribe(XnodeID, XNodeChanged);
        XValue = Xsubscription.ToString();

        var Ysubscription = Interface.Subscribe(YnodeID, YNodeChanged);
        YValue = Ysubscription.ToString();

        var Zsubscription = Interface.Subscribe(ZnodeID, ZNodeChanged);
        ZValue = Zsubscription.ToString();
        //Debug.LogError(dataFromOPCUANode);
        //digitalTwinRFIDFeedbackTMP.text = RFIDInfo;
        //uiRFIDFeedbackTMP.text = RFIDInfo;        
    }

    private void OnInterfaceDisconnected()
    {
        Debug.LogWarning("Factory Machine " + factoryMachineID + " has disconnected");
    }

    private void OnInterfaceReconnect()
    {
        Debug.LogWarning("Factory Machine " + factoryMachineID + " has reconnected");
    }


    
        

    public void RXNodeChanged(OPCUANodeSubscription sub, object value)
    {
        RXValue = value.ToString();   
    }

    public void RYNodeChanged(OPCUANodeSubscription sub, object value)
    {
        RYValue = value.ToString();
    }

    public void RZNodeChanged(OPCUANodeSubscription sub, object value)
    {
        RZValue = value.ToString();
    }

    public void XNodeChanged(OPCUANodeSubscription sub, object value)
    {
        XValue = value.ToString();
    }

    public void YNodeChanged(OPCUANodeSubscription sub, object value)
    {
        YValue = value.ToString();
    }

    public void ZNodeChanged(OPCUANodeSubscription sub, object value)
    {
        ZValue = value.ToString();
    }

    private void Update()
    {
        // 累加时间
        timeSinceLastRefresh += Time.deltaTime;

        // 检查是否到达刷新间隔
        if (timeSinceLastRefresh >= refreshInterval)
        {
            // 重置计时器
            timeSinceLastRefresh = 0f;
            xRotation = RXValue;
            yRotation = RYValue;
            zRotation = RZValue;
            xPosition = XValue; 
            yPosition = YValue;
            zPosition = ZValue;

            uiFeedbackTMP.text = "X" + " : " + XValue + "\n" + "Y" + " : " + YValue + "\n" + "Z" + " : " + ZValue + "\n" + "RX" + " : " + RXValue + "\n" + "RY" + " : " + RYValue + "\n" + "RZ" + " : " + RZValue;
            
            //text.text = "IP: " + serverIP;
            //text2.text = "Port: " + serverPort.ToString();

            Vector3 position = new(ParseStringToFloat(xPosition), ParseStringToFloat(yPosition), ParseStringToFloat(zPosition));
            Vector3 rotation = new(ParseStringToFloat(xRotation), ParseStringToFloat(yRotation), ParseStringToFloat(zRotation));

            // 将物体的 X 轴旋转设置为 0
            SetXRotationToZero(msXRotation.transform);

            //if (targetObjectTransform != null)
            //{
            //    //targetObjectTransform.position = position;
            //    targetObjectTransform.rotation = Quaternion.Euler(rotation);
            //}
            //else
            //{
            //    Debug.LogError("Target object transform is not assigned!");
            //}

            transform.SetPositionAndRotation(position, Quaternion.Euler(rotation));
        }
    }

    private float ParseStringToFloat(string value)
    {
        float result = 0f;
        float.TryParse(value, out result);
        return result;
    }

    void SetXRotationToZero(Transform targetTransform)
    {
        // 获取当前的旋转
        Quaternion currentRotation = targetTransform.rotation;

        // 将 X 轴旋转设置为 0
        Quaternion newRotation = Quaternion.Euler(-90f, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);

        // 应用新的旋转
        targetTransform.rotation = newRotation;
    }
}
