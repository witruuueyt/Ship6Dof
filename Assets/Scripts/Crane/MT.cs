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
    //private Vector3 lastBPosition; // ����B�����λ��


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
        // ͨ��λ��ֵ����B��λ��
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
    //    // ͨ��λ��ֵ����B��λ��
    //    partB.localPosition = new Vector3(partB.localPosition.x, partB.localPosition.y, displacement);


    //    // ����Ƿ���ҪֹͣB���ƶ�
    //    if (!bStopped && Mathf.Abs(fixedData) >= stopThreshold)
    //    {
    //        // ���B�ƶ��ľ��볬����ֵ��ֹͣB���ƶ�
    //        bStopped = true;
    //        Debug.Log("Bֹͣ�ƶ�");

    //        // �������������Ҫִ�е��߼�
    //        lastBPosition = partB.position;
    //        // ����C���ƶ�
    //        StartMovingPartC();
    //    }

    //    // ���B�Ѿ�ֹͣ���ƶ�C
    //    if (bStopped)
    //    {
    //        MovePartC(displacement);
    //        // ����bStopped״̬��ʹ��B�ܹ���fixedData�ٴ�С��5��ʱ������ƶ�
    //        if (Mathf.Abs(fixedData) < stopThreshold)
    //        {
    //            bStopped = false;
    //            Debug.Log("B�����ƶ�");
    //        }
    //    }
    //}

    //void StartMovingPartC()
    //{
    //    partB.localPosition = lastBPosition;

    //    Debug.Log("C��ʼ�ƶ�");
    //}

    //void MovePartC(float displacement)
    //{
    //    // ͨ��λ��ֵ����C��λ��
    //    partC.localPosition = new Vector3(partC.localPosition.x, partC.localPosition.y, displacement);
    //    partB.position = lastBPosition;
    //}


}

