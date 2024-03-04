using UnityEngine;
using game4automation;
using TMPro;
using System.Net;

public class MT : MonoBehaviour
{
    public Transform partA;
    public Transform partB;
    public Transform partC;
    private float maxLength = 7.5f; // 部件最大长度
    public TMP_Text uiFeedbackTMP;
    private string previousMoveData = "0";


    public float maxTranslationSpeed = 10f;

    public float accelerationRate = 10f;
    public float decelerationRate = 10f;
    public float maxDistance;
    float zPosition;
    [SerializeField]
    private float currentTranslationSpeed = 0f;
    private Vector3 initialPositionB;
    public float minXPosition = -5.87f;
    public float maxXPosition = -0.37f;


    //private bool bStopped = false;
    //private Vector3 lastBPosition; // 保存B的最后位置


    [Header("Factory Machine")]
    public string factoryMachineID;
    public OPCUA_Interface Interface;


    [Header("OPCUA Reader")]
    public string nodeBeingMonitored;
    public string nodeID;
    public string dataFromOPCUANode;
    public float fixedData ;
    public string moveNodeID;
    public string moveData;


    void Start()
    {
        Interface.EventOnConnected.AddListener(OnInterfaceConnected);
        Interface.EventOnConnected.AddListener(OnInterfaceDisconnected);
        Interface.EventOnConnected.AddListener(OnInterfaceReconnect);
        initialPositionB = partB.transform.position;
    }


    private void OnInterfaceConnected()
    {
        Debug.LogWarning("Connected to Factory Machine " + factoryMachineID);
        var subscription = Interface.Subscribe(nodeID, NodeChanged);
        dataFromOPCUANode = subscription.ToString();
        //var subscriptionMove = Interface.Subscribe(moveNodeID, MoveNodeChanged);
        //moveData = subscription.ToString();
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

    //public void MoveNodeChanged(OPCUANodeSubscription sub, object value)
    //{
    //    moveData = value.ToString();
    //    Debug.Log("Factory machine " + factoryMachineID + " just registered " + nodeBeingMonitored + " as " + dataFromOPCUANode);
    //}

    public void MoveForward()
    {
        moveData = "1";
    }


    public void MoveBackward()
    {
        moveData = "2";
    }


    public void StopMovement()
    {
        moveData = "0";
    }

    void Update()
    {
        uiFeedbackTMP.text = factoryMachineID + ":" + dataFromOPCUANode;
        if (float.TryParse(dataFromOPCUANode, out float parsedData))
        {
            if (moveData.Equals("1") && parsedData < 1500f)
            {


                parsedData += 10f;
                WriteValue();

            }
            else if (moveData.Equals("2") && parsedData > 50f)
            {
                parsedData -= 10f;
                WriteValue();
            }
            fixedData = parsedData / 100f * -1;
            UpdatePosition();
        }
        
    }

    void UpdatePosition()
    {
        float lengthB = Mathf.Min(Mathf.Abs(fixedData), maxLength) * Mathf.Sign(fixedData);
        partB.localPosition = new Vector3(0, 0, lengthB);

        float lengthC = Mathf.Clamp(Mathf.Max(Mathf.Abs(fixedData) - maxLength, 0), 0, maxLength) * Mathf.Sign(fixedData);
        partC.localPosition = new Vector3(-0.636806f, 0, lengthC);
    }


    public void WriteValue()
    {
        if (moveData.Equals("1") || moveData.Equals("2"))
        {
            Interface.WriteNodeValue(nodeID, (fixedData * 100 * -1));

        }
        //Debug.Log(nodeID + dataFromOPCUANode);
    }


}   

    //public void Move()
    //{
    //    //接收到的moveData数值为1时，开始移动
    //    if (moveData.Equals("1"))
    //    {
    //        //加速度
    //        currentTranslationSpeed = Mathf.MoveTowards(currentTranslationSpeed, maxTranslationSpeed, accelerationRate * Time.deltaTime);

    //        // 沿Z轴平移
    //        partB.transform.Translate(Vector3.forward * currentTranslationSpeed * Time.deltaTime * -1);

    //        // 检查是否达到最大距离，如果达到，则立即停止
    //        if (Mathf.Abs((partB.transform.position.z - initialPositionB.z)*-1) >= maxLength)
    //        {
    //            currentTranslationSpeed = 0f;
    //            accelerationRate = 0f;
    //        }
    //    }
    //    else
    //    {
    //        // 减速
    //        currentTranslationSpeed = Mathf.MoveTowards(currentTranslationSpeed, 0f, decelerationRate * Time.deltaTime);

    //        // 沿Z轴平移
    //        partB.transform.Translate(Vector3.forward * currentTranslationSpeed * Time.deltaTime * -1);
    //    }
    //}


//----------------------------------------------------------
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




