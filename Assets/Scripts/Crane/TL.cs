using UnityEngine;
using game4automation;
using TMPro;

public class TL : MonoBehaviour
{

    public float maxRotationSpeed = 10f;
    public float accelerationRate = 10f;
    public float decelerationRate = 10f;
    public float targetAngleMin = -50f;
    public float targetAngleMax = 20f;

    float xRotation;
    private float currentRotationSpeed = 0f;
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

    public void Rotate1()
    {
        moveData = "1";
    }

    public void Rotate2()
    {
        moveData = "2";
    }

    public void StopRotation()
    {
        moveData = "0";
    }

    void Update()
    {
        Spin();
        uiFeedbackTMP.text = factoryMachineID + ":" + dataFromOPCUANode;
        if (float.TryParse(dataFromOPCUANode, out float parsedData))
        {
            angleInDegrees = parsedData / 10f;

            RotateObjectOnX(angleInDegrees);
        }
        else
        {
            //Debug.LogWarning("Failed to parse data from OPC UA node.");
        }
    }
    void RotateObjectOnX(float angle)
    {
        if (moveData.Equals("1") || moveData.Equals("2"))
        {
            WriteValue();

        }

        else
        {
            //Quaternion currentRotation = transform.localRotation;

            //// 使用Quaternion.Euler构建新的本地旋转
            //Quaternion newRotation = Quaternion.Euler(angle, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);

            //// 应用新的本地旋转
            //transform.localRotation = newRotation;

            transform.localRotation = Quaternion.Euler(angle, 0f, 0f);

        }
    }

    public void WriteValue()
    {
        xRotation = transform.eulerAngles.x;

        Interface.WriteNodeValue(nodeID, xRotation);
        //Debug.Log(nodeID + dataFromOPCUANode);
    }

    public void Spin()
    {
        Vector3 rotationDirection = Vector3.zero; // 旋转方向向量初始化为零向量
        float targetRotationSpeed; // 目标旋转速度

        if (moveData.Equals("1"))
        {
            rotationDirection = Vector3.left;
            targetRotationSpeed = maxRotationSpeed; // 设置目标旋转速度为最大旋转速度
            previousMoveData = "1";
        }
        else if (moveData.Equals("2"))
        {
            rotationDirection = Vector3.right;
            targetRotationSpeed = maxRotationSpeed;
            previousMoveData = "2";
        }
        else
        {
            targetRotationSpeed = 0f; // 没有移动数据时，目标旋转速度为零

            // 根据上一个移动数据确定旋转方向
            if (previousMoveData.Equals("1"))
            {
                rotationDirection = Vector3.left;
            }
            else if (previousMoveData.Equals("2"))
            {
                rotationDirection = Vector3.right;
            }
        }

        // 根据加速度调整当前旋转速度
        currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, targetRotationSpeed, accelerationRate * Time.deltaTime);

        // 根据当前旋转速度和方向进行旋转
        transform.Rotate(rotationDirection, currentRotationSpeed * Time.deltaTime);

        //Debug.LogWarning(GetEffectiveRotationX()); 

        // 检查是否超出指定角度范围，如果是则停止旋转并固定在范围边界上
        float effectiveRotationX = GetEffectiveRotationX();
        if (effectiveRotationX > targetAngleMax)
        {
            transform.localEulerAngles = new Vector3(targetAngleMax, transform.localEulerAngles.y, transform.localEulerAngles.z);
            currentRotationSpeed = 0f;
            moveData = "0";
        }
        else if (effectiveRotationX < targetAngleMin)
        {
            transform.localEulerAngles = new Vector3(targetAngleMin, transform.localEulerAngles.y, transform.localEulerAngles.z);
            currentRotationSpeed = 0f;
            moveData = "0";
        }
    }

    // 获取有效的 x 轴旋转角度方法
    float GetEffectiveRotationX()
    {
        float effectiveRotation = transform.localEulerAngles.x; // 获取 x 轴旋转角度
        if (transform.localRotation.eulerAngles.x > 180) // 如果角度大于180度
        {
            effectiveRotation -= 360; // 将大于180度的角度转换为负数
        }
        return effectiveRotation; // 返回有效的 x 轴旋转角度
    }
}
