using UnityEngine;
using game4automation;
using TMPro;

public class TT : MonoBehaviour
{
    
    public TMP_Text uiFeedbackTMP;

    public float minXPosition = -5.87f;
    public float maxXPosition = -0.37f;
    public float maxTranslationSpeed = 10f;

    public float accelerationRate = 10f;
    public float decelerationRate = 10f;
    public float maxLength = 6.24f;//5.87f + 0.37f
    private Transform myTransform;
    float zPosition;
    [SerializeField]
    private float currentTranslationSpeed = 0f;
    private Vector3 initialPositionB;
    private string previousMoveData = "0";

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
        moveData = "3";
    }
    void Update()
    {
        Move();
        uiFeedbackTMP.text = factoryMachineID + ":" + dataFromOPCUANode;


        if (float.TryParse(dataFromOPCUANode, out float parsedData))
        {
            fixedData = (parsedData / 100f * -1) - 0.37f;
            UpdatePosition(fixedData);
        }
        else
        {
            //Debug.LogWarning("Failed to parse data from OPC UA node.");
        }
    }
    void UpdatePosition(float displacement)
    {
        if (moveData.Equals("1") || moveData.Equals("2") || moveData.Equals("3"))
        {
            //WriteValue();

        }

        else
        {
            float length = Mathf.Min(Mathf.Abs(displacement), maxLength) * Mathf.Sign(displacement);
            transform.localPosition = new Vector3(length, -0.004704595f, 0.7641368f);

        }
    }

    public void WriteValue()
    {
        zPosition = transform.position.z * -1;

        Interface.WriteNodeValue(nodeID, zPosition);
        //Debug.Log(nodeID + dataFromOPCUANode);
    }

    public void Move()
    {
        Vector3 translationDirection = Vector3.zero;
        float targetTranslationSpeed;

        // 根据移动数据确定移动方向和目标移动速度
        if (moveData.Equals("1"))
        {
            translationDirection = Vector3.forward; //因为父级物体的角度以及自己的角度，反正是沿着x轴移动的
            targetTranslationSpeed = maxTranslationSpeed;
            previousMoveData = "1";
        }
        else if (moveData.Equals("2"))
        {
            translationDirection = Vector3.back; //因为父级物体的角度以及自己的角度，反正是沿着x轴移动的
            targetTranslationSpeed = maxTranslationSpeed;
            previousMoveData = "2";
        }
        else
        {
            targetTranslationSpeed = 0f;

            if (previousMoveData.Equals("1"))
            {
                translationDirection = Vector3.forward;
            }
            else if (previousMoveData.Equals("2"))
            {
                translationDirection = Vector3.back;
            }
        }

        // 根据加速度调整当前移动速度
        currentTranslationSpeed = Mathf.MoveTowards(currentTranslationSpeed, targetTranslationSpeed, accelerationRate * Time.deltaTime);

        // 根据当前移动速度和方向进行移动
        transform.Translate(translationDirection * currentTranslationSpeed * Time.deltaTime);

        // 检查是否超出指定位置范围，如果是则停止移动并固定在范围边界上
        float effectiveXPosition = transform.localPosition.x;
        if (effectiveXPosition > maxXPosition)
        {
            transform.localPosition = new Vector3(maxXPosition, transform.localPosition.y, transform.localPosition.z);
            currentTranslationSpeed = 0f;
            moveData = "0";
        }
        else if (effectiveXPosition < minXPosition)
        {
            transform.localPosition = new Vector3(minXPosition, transform.localPosition.y, transform.localPosition.z);
            currentTranslationSpeed = 0f;
            moveData = "0";
        }

        if (currentTranslationSpeed == 0f)
        {
            moveData = "0";
        }
    }
}

