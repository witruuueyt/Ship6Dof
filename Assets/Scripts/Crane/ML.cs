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

    float yRotation;
    public float targetAngleMin = -30f;
    public float targetAngleMax = 40f;
    private bool shouldStop = false;

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
            angleInDegrees = (parsedData / 10f);

            RotateObjectOnY(angleInDegrees);
        }
        else
        {
            //Debug.LogWarning("Failed to parse data from OPC UA node.");
        }
        

    }
    void RotateObjectOnY(float angle)
    {
        if (moveData.Equals("1") || moveData.Equals("2"))
        {
            WriteValue();

        }

        else
        {
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);

            //// ��ȡ��ǰ�����ŷ����
            //Vector3 currentRotation = transform.eulerAngles;

            //// �����µ�Z��Ƕ�
            //currentRotation.z = angle;

            //// Ӧ���µ�ŷ���ǣ���ת����
            //transform.eulerAngles = currentRotation;
        }
    }

    public void WriteValue()
    {

        if (moveData.Equals("1") || moveData.Equals("2"))
        {

            yRotation = transform.eulerAngles.y;

            Interface.WriteNodeValue(nodeID, yRotation);
            //Debug.Log(nodeID + dataFromOPCUANode);
            //}
        }
    }
    public void Spin()
    {
        Vector3 rotationDirection = Vector3.zero; // ��ת����������ʼ��Ϊ������
        float targetRotationSpeed; // Ŀ����ת�ٶ�

        if (moveData.Equals("1"))
        {
            rotationDirection = Vector3.up;
            targetRotationSpeed = maxRotationSpeed; // ����Ŀ����ת�ٶ�Ϊ�����ת�ٶ�
            previousMoveData = "1";
        }
        else if (moveData.Equals("2"))
        {
            rotationDirection = Vector3.down;
            targetRotationSpeed = maxRotationSpeed;
            previousMoveData = "2";
        }
        else
        {
            targetRotationSpeed = 0f; // û���ƶ�����ʱ��Ŀ����ת�ٶ�Ϊ��

            // ����previousMoveDataȷ����ת����
            if (previousMoveData.Equals("1"))
            {
                rotationDirection = Vector3.up;
            }
            else if (previousMoveData.Equals("2"))
            {
                rotationDirection = Vector3.down;
            }
        }

        // ���ݼ��ٶȵ�����ǰ��ת�ٶ�
        currentRotationSpeed = Mathf.MoveTowards(currentRotationSpeed, targetRotationSpeed, accelerationRate * Time.deltaTime);

        // ���ݵ�ǰ��ת�ٶȺͷ��������ת
        transform.Rotate(rotationDirection, currentRotationSpeed * Time.deltaTime);

        //Debug.LogWarning(GetEffectiveRotationX()); 

        // ����Ƿ񳬳�ָ���Ƕȷ�Χ���������ֹͣ��ת���̶��ڷ�Χ�߽���
        float effectiveRotationY = GetEffectiveRotationY();
        if (effectiveRotationY > targetAngleMax)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, targetAngleMax, transform.localEulerAngles.z);
            currentRotationSpeed = 0f;
            moveData = "0";
        }
        else if (effectiveRotationY < targetAngleMin)
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, targetAngleMin, transform.localEulerAngles.z);
            currentRotationSpeed = 0f;
            moveData = "0";
        }

        // ��ȡ��Ч�� x ����ת�Ƕȷ���
        float GetEffectiveRotationY()
        {
            float effectiveRotation = transform.localEulerAngles.y; // ��ȡ x ����ת�Ƕ�
            if (transform.localRotation.eulerAngles.y > 180) // ����Ƕȴ���180��
            {
                effectiveRotation -= 360; // ������180�ȵĽǶ�ת��Ϊ����
            }
            return effectiveRotation; // ������Ч�� x ����ת�Ƕ�
        }
    }
}
