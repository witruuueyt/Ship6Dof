using UnityEngine;
using game4automation;
using TMPro;

public class TT : MonoBehaviour
{
    
    public TMP_Text uiFeedbackTMP;


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
        if (moveData.Equals("1"))
        {
            //WriteValue();

        }

        else
        {
            float length = Mathf.Min(Mathf.Abs(displacement), maxLength) * Mathf.Sign(displacement);
            transform.localPosition = new Vector3(length, -0.004704595f, 0.7641368f);

        }
    }

    //public void WriteValue()
    //{
    //    zPosition = transform.position.z * -1;

    //    Interface.WriteNodeValue(nodeID, zPosition);
    //    //Debug.Log(nodeID + dataFromOPCUANode);
    //}

    public void Move()
    {
        // 初始化rotationDirection为默认值，例如Vector3.zero
        Vector3 translationDirection = Vector3.zero;
        float targetTranslationSpeed;

        if (moveData.Equals("1"))
        {
            // 当moveData为"1"时，向前加速
            translationDirection = Vector3.forward;
            targetTranslationSpeed = maxTranslationSpeed;
            previousMoveData = moveData;

        }
        else if (moveData.Equals("2"))
        {
            // 当moveData为"2"时，向后加速
            translationDirection = Vector3.back;
            targetTranslationSpeed = maxTranslationSpeed;
            previousMoveData = moveData;

        }
        else
        {
            // 当moveData为其他值时，减速
            targetTranslationSpeed = 0f;

            //Debug.Log(previousMoveData);
            if (previousMoveData.Equals("1"))
            {
                translationDirection = Vector3.forward;

            }

            else if (previousMoveData.Equals("2"))
            {
                translationDirection = Vector3.back;

            }

        }

        // 根据当前速度和目标速度计算新的旋转速度
        currentTranslationSpeed = Mathf.MoveTowards(currentTranslationSpeed, targetTranslationSpeed, accelerationRate * Time.deltaTime);

        // 根据旋转方向和当前速度执行旋转
        transform.Translate(translationDirection * currentTranslationSpeed * Time.deltaTime);

    }
}

