using UnityEngine;
using game4automation;
using TMPro;

public class MS : MonoBehaviour
{
    public float maxRotationSpeed = 10f;
    public float accelerationRate = 10f;
    public float decelerationRate = 10f;
    private float currentRotationSpeed = 0f;
    private Transform myTransform;

    float zRotation;
    private string previousMoveData = "0";

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
        //WriteValue();
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
        if (moveData.Equals("1") || moveData.Equals("2"))
        {

        }

        else
        {
            Quaternion currentRotation = transform.localRotation;

            // 使用Quaternion.Euler构建新的本地旋转
            Quaternion newRotation = Quaternion.Euler(angle, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);

            // 应用新的本地旋转
            transform.localRotation = newRotation;
        }
        

    }

    //public void WriteValue()
    //{

    //    zRotation = transform.eulerAngles.z;

    //    Interface.WriteNodeValue(nodeID, zRotation);
    //    //Debug.Log(nodeID + dataFromOPCUANode);
    //}

    public void Spin()
    {
        Vector3 rotationDirection = Vector3.zero;
        float targetRotationSpeed;

        if (moveData.Equals("1") || Input.GetKey(KeyCode.LeftArrow))
        {
            rotationDirection = Vector3.right;
            targetRotationSpeed = maxRotationSpeed;
            previousMoveData = "1";

        }
        else if (moveData.Equals("2") || Input.GetKey(KeyCode.RightArrow))
        {
            rotationDirection = Vector3.left;
            targetRotationSpeed = maxRotationSpeed;
            previousMoveData = "2";

        }
        else
        {
            targetRotationSpeed = 0f;

            if (previousMoveData.Equals("1"))
            {
                rotationDirection = Vector3.right;
            }

            else if (previousMoveData.Equals("2"))
            {
                rotationDirection = Vector3.left;
            }

        }

        currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, targetRotationSpeed, accelerationRate * Time.deltaTime);

        transform.Rotate(rotationDirection, currentRotationSpeed * Time.deltaTime);

    }
}
