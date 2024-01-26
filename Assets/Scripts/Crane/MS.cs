using UnityEngine;
using game4automation;
using TMPro;

public class MS : MonoBehaviour
{
    public float maxRotationSpeed = 30f;
    public float accelerationRate = 10f;
    public float decelerationRate = 10f;
    private float currentRotationSpeed = 0f;
    private Transform myTransform;

    float zRotation;


    [Header("Factory Machine")]
    public string factoryMachineID;
    public OPCUA_Interface Interface;


    [Header("OPCUA Reader")]
    public string nodeBeingMonitored;
    public string nodeID;

    //public TMP_Text digitalTwinFeedbackTMP;
    public TMP_Text uiFeedbackTMP;
    public string dataFromOPCUANode;
    public float angleInDegrees;
    public string moveNodeID;
    public string moveData;

    void Start()
    {
        myTransform = GetComponent<Transform>();

        //InvokeRepeating("UpdateData", 0f, 0.1f);
        Interface.EventOnConnected.AddListener(OnInterfaceConnected);
        Interface.EventOnConnected.AddListener(OnInterfaceDisconnected);
        Interface.EventOnConnected.AddListener(OnInterfaceReconnect);
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
        Spin();
        WriteValue();
        uiFeedbackTMP.text = factoryMachineID + ":" + dataFromOPCUANode;
        if (float.TryParse(dataFromOPCUANode, out float parsedData))
        {
            angleInDegrees = parsedData / 10f;

            RotateObjectOnZ(angleInDegrees);
        }
        else
        {
            //Debug.LogWarning("Failed to parse data from OPC UA node.");
        }
    }
    void RotateObjectOnZ(float angle)
    {
        if (moveData.Equals("1"))
        {

        }

        else
        {
            // 获取当前物体的旋转
            Quaternion currentRotation = transform.rotation;

            // 使用Quaternion.Euler构建新的旋转
            Quaternion newRotation = Quaternion.Euler(currentRotation.eulerAngles.x, 0f, angle);

            // 应用新的旋转
            transform.rotation = newRotation;
        }
        

    }

    public void WriteValue()
    {

        zRotation = transform.eulerAngles.z;

        Interface.WriteNodeValue(nodeID, zRotation);
        //Debug.Log(nodeID + dataFromOPCUANode);
    }

    public void Spin()
    {
        //接收到的moveData数值为1时，开始旋转
        if (moveData.Equals("1"))
        {
            //加速度
            currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, maxRotationSpeed, accelerationRate * Time.deltaTime);

            
            transform.Rotate(Vector3.right, currentRotationSpeed * Time.deltaTime);

        }

        else
        {
            // 减速
            currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, 0f, decelerationRate * Time.deltaTime);

            
            transform.Rotate(Vector3.right, currentRotationSpeed * Time.deltaTime);
        }
    }
}
