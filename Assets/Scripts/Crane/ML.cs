using UnityEngine;
using game4automation;
using TMPro;

public class ML : MonoBehaviour
{
    public float maxRotationSpeed = 10f;
    public float accelerationRate = 10f;
    public float decelerationRate = 10f;
    [SerializeField]
    private float currentRotationSpeed = 0f;
    private Transform myTransform;

    float zRotation;

    private string previousMoveData = "0"; // �� previousMoveData �����ƶ�����ķ�Χ�ڣ��Ա��ڷ���֮�䱣��״̬��
    //private float refreshInterval = 0.1f;
    //private float timeSinceLastRefresh = 0f;
    //Transform objectTransform;

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
            angleInDegrees = (parsedData / 10f) - 90;

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
            // ��moveData��ֵ����"1"��"2"ʱ��ִ������Ĵ���
        }
       
        else
        {

            // ��ȡ��ǰ�����ŷ����
            Vector3 currentRotation = transform.eulerAngles;

            // �����µ�Z��Ƕ�
            currentRotation.z = angle;

            // Ӧ���µ�ŷ���ǣ���ת����
            transform.eulerAngles = currentRotation;
        }
    }

    //public void WriteValue()
    //{

    //if (moveData.Equals("1"))
    //{

    //    zRotation = transform.eulerAngles.z;

    //    Interface.WriteNodeValue(nodeID, zRotation);
    //    //Debug.Log(nodeID + dataFromOPCUANode);
    ////}
    //}
    //    }
    public void Spin()
    {
        // ��ʼ��rotationDirectionΪĬ��ֵ������Vector3.zero
        Vector3 rotationDirection = Vector3.zero;
        float targetRotationSpeed;

        if (moveData.Equals("1"))
        {
            rotationDirection = Vector3.forward;
            targetRotationSpeed = maxRotationSpeed;
            previousMoveData = moveData;
        }
        else if (moveData.Equals("2"))
        {
            rotationDirection = Vector3.back;
            targetRotationSpeed = maxRotationSpeed;
            previousMoveData = moveData;
        }
        else
        {
            targetRotationSpeed = 0f;

            if (previousMoveData.Equals("1"))
            {
                rotationDirection = Vector3.forward;   
            }

            else if (previousMoveData.Equals("2"))
            {
                rotationDirection = Vector3.back;
            }
        }

        currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, targetRotationSpeed, accelerationRate * Time.deltaTime);

        transform.Rotate(rotationDirection, currentRotationSpeed * Time.deltaTime);

    }
}
