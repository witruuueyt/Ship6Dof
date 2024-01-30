using UnityEngine;
using game4automation;
using TMPro;

public class TS : MonoBehaviour
{

    public float maxRotationSpeed = 10f;
    public float accelerationRate = 10f;
    public float decelerationRate = 10f;

    private Transform myTransform;
    float xRotation;
    private string previousMoveData = "0";

    [SerializeField]
    private float currentRotationSpeed = 0f;



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

        }

        else
        {
            Quaternion currentRotation = transform.localRotation;

            // ʹ��Quaternion.Euler�����µı�����ת
            Quaternion newRotation = Quaternion.Euler(angle, currentRotation.eulerAngles.y, currentRotation.eulerAngles.z);

            // Ӧ���µı�����ת
            transform.localRotation = newRotation;
        }
    }

    //public void WriteValue()
    //{
    //    xRotation = transform.eulerAngles.x;

    //    Interface.WriteNodeValue(nodeID, xRotation);
    //    //Debug.Log(nodeID + dataFromOPCUANode);
    //}

    public void Spin()
    {
        // ��ʼ��rotationDirectionΪĬ��ֵ������Vector3.zero
        Vector3 rotationDirection = Vector3.zero;
        float targetRotationSpeed;

        if (moveData.Equals("1"))
        {
            // ��moveDataΪ"1"ʱ����ǰ����
            rotationDirection = Vector3.forward;
            targetRotationSpeed = maxRotationSpeed;
            previousMoveData = moveData;

        }
        else if (moveData.Equals("2"))
        {
            // ��moveDataΪ"2"ʱ��������
            rotationDirection = Vector3.back;
            targetRotationSpeed = maxRotationSpeed;
            previousMoveData = moveData;

        }
        else
        {
            // ��moveDataΪ����ֵʱ������
            targetRotationSpeed = 0f;

            //Debug.Log(previousMoveData);
            if (previousMoveData.Equals("1"))
            {
                rotationDirection = Vector3.forward;

            }

            else if (previousMoveData.Equals("2"))
            {
                rotationDirection = Vector3.back;

            }

        }

        // ���ݵ�ǰ�ٶȺ�Ŀ���ٶȼ����µ���ת�ٶ�
        currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, targetRotationSpeed, accelerationRate * Time.deltaTime);

        // ������ת����͵�ǰ�ٶ�ִ����ת
        transform.Rotate(rotationDirection, currentRotationSpeed * Time.deltaTime);

    }
}

