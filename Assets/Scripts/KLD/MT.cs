using UnityEngine;
using game4automation;
using TMPro;

public class MT : MonoBehaviour
{
    public Transform partA;
    public Transform partB;
    public Transform partC;
    public TMP_Text uiFeedbackTMP;

    public float stopThreshold = 5f;

    //private bool bStopped = false;
    //private Vector3 lastBPosition; // 保存B的最后位置


    [Header("Factory Machine")]
    public string factoryMachineID;
    public OPCUA_Interface Interface;


    [Header("OPCUA Reader")]
    public string nodeBeingMonitored;
    public string nodeID;

    public string dataFromOPCUANode;
    public float fixedData;


    void Start()
    {
        Interface.EventOnConnected.AddListener(OnInterfaceConnected);
        Interface.EventOnConnected.AddListener(OnInterfaceDisconnected);
        Interface.EventOnConnected.AddListener(OnInterfaceReconnect);
        InvokeRepeating("UpdateData", 0f, 0.1f);



    }


    private void OnInterfaceConnected()
    {
        Debug.LogWarning("Connected to Factory Machine " + factoryMachineID);
        var subscription = Interface.Subscribe(nodeID, NodeChanged);
        dataFromOPCUANode = subscription.ToString();
        Debug.LogError(dataFromOPCUANode);
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

    public void NodeChanged(OPCUANodeSubscription sub, object value)
    {
        dataFromOPCUANode = value.ToString();
        Debug.Log("Factory machine " + factoryMachineID + " just registered " + nodeBeingMonitored + " as " + dataFromOPCUANode);
    }


    void UpdateData()
    { 
        uiFeedbackTMP.text = factoryMachineID + ":" + dataFromOPCUANode;
        if (float.TryParse(dataFromOPCUANode, out float parsedData))
        {
            fixedData = parsedData / 100f * -1;
            UpdatePosition(fixedData);
        }
        else
        {
            //Debug.LogWarning("Failed to parse data from OPC UA node.");
        }
    }
    void UpdatePosition(float displacement)
    {
        // 通过位移值更新B的位置
        partB.localPosition = new Vector3(partB.localPosition.x, partB.localPosition.y, displacement);

    }


    //void UpdateData()
    //{
    //    if (float.TryParse(dataFromOPCUANode, out float parsedData))
    //    {
    //        fixedData = parsedData / 100f;
    //        UpdatePosition(fixedData);
    //    }
    //    else
    //    {
    //        //Debug.LogWarning("Failed to parse data from OPC UA node.");
    //    }
    //}

    //public void UpdatePosition(float displacement)
    //{
    //    // 通过位移值更新B的位置
    //    partB.localPosition = new Vector3(partB.localPosition.x, partB.localPosition.y, displacement);


    //    // 检查是否需要停止B的移动
    //    if (!bStopped && Mathf.Abs(fixedData) >= stopThreshold)
    //    {
    //        // 如果B移动的距离超过阈值，停止B的移动
    //        bStopped = true;
    //        Debug.Log("B停止移动");

    //        // 可以添加其他需要执行的逻辑
    //        lastBPosition = partB.position;
    //        // 启动C的移动
    //        StartMovingPartC();
    //    }

    //    // 如果B已经停止，移动C
    //    if (bStopped)
    //    {
    //        MovePartC(displacement);
    //        // 重置bStopped状态，使得B能够在fixedData再次小于5的时候继续移动
    //        if (Mathf.Abs(fixedData) < stopThreshold)
    //        {
    //            bStopped = false;
    //            Debug.Log("B继续移动");
    //        }
    //    }
    //}

    //void StartMovingPartC()
    //{
    //    partB.localPosition = lastBPosition;

    //    Debug.Log("C开始移动");
    //}

    //void MovePartC(float displacement)
    //{
    //    // 通过位移值更新C的位置
    //    partC.localPosition = new Vector3(partC.localPosition.x, partC.localPosition.y, displacement);
    //    partB.position = lastBPosition;
    //}


}

