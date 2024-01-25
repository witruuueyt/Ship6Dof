using UnityEngine;
using game4automation;
using TMPro;

public class MT : MonoBehaviour
{
    public Transform partA;
    public Transform partB;
    public Transform partC;
    private float maxLength = 7.5f; // 部件最大长度
    public TMP_Text uiFeedbackTMP;

    //public float stopThreshold = 5f;

    public float maxTranslationSpeed = 100f;
    public float accelerationRate = 200f;
    public float decelerationRate = 200f;
    public float maxDistance;
    private Transform myTransform;
    float zPosition;
    [SerializeField]
    private float currentTranslationSpeed = 0f;
    private Vector3 initialPositionB;

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
    public string moveNodeID;
    public string moveData;


    void Start()
    {
        Interface.EventOnConnected.AddListener(OnInterfaceConnected);
        Interface.EventOnConnected.AddListener(OnInterfaceDisconnected);
        Interface.EventOnConnected.AddListener(OnInterfaceReconnect);
        //InvokeRepeating("UpdateData", 0f, 0.1f);
        myTransform = GetComponent<Transform>();
        initialPositionB = partB.transform.position;
    }


    private void OnInterfaceConnected()
    {
        Debug.LogWarning("Connected to Factory Machine " + factoryMachineID);
        var subscription = Interface.Subscribe(nodeID, NodeChanged);
        dataFromOPCUANode = subscription.ToString();
        var subscriptionMove = Interface.Subscribe(moveNodeID, MoveNodeChanged);
        moveData = subscription.ToString();
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

    public void MoveNodeChanged(OPCUANodeSubscription sub, object value)
    {
        moveData = value.ToString();
        Debug.Log("Factory machine " + factoryMachineID + " just registered " + nodeBeingMonitored + " as " + dataFromOPCUANode);
    }

    void Update()
    {
        //Move();
        //WriteValue();
        uiFeedbackTMP.text = factoryMachineID + ":" + dataFromOPCUANode;
        if (float.TryParse(dataFromOPCUANode, out float parsedData))
        {
            fixedData = parsedData / 100f;
            UpdatePosition();
        }
        else
        {
            //Debug.LogWarning("Failed to parse data from OPC UA node.");
        }
    }
    void UpdatePosition()
    {

        if (moveData.Equals("1"))
        {

        }

        else
        {
            // 根据 fixedData 计算每个部件的位置
            float normalizedPosition = Mathf.Clamp01(Mathf.Abs(fixedData) / maxLength);

            // 更新部件A的位置（始终不变）
            partA.localPosition = Vector3.zero;

            // 更新部件B的位置
            float lengthB = Mathf.Min(Mathf.Abs(fixedData), maxLength) * Mathf.Sign(fixedData);
            partB.localPosition = new Vector3(0, 0, lengthB);

            // 更新部件C的位置
            float lengthC = Mathf.Max(Mathf.Abs(fixedData) - maxLength, 0) * Mathf.Sign(fixedData);
            partC.localPosition = new Vector3(0, 0, lengthC);
        }
    }

    public void WriteValue()
    {
        zPosition = transform.position.z * -1;

        Interface.WriteNodeValue(nodeID, zPosition);
        //Debug.Log(nodeID + dataFromOPCUANode);
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
    //        if (Mathf.Abs((partB.transform.position.z - initialPositionB.z)*-1) >= maxDistance)
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

}
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




