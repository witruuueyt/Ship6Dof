using UnityEngine;
using game4automation;
using TMPro;

public class TT : MonoBehaviour
{
    public Transform partA;
    public Transform partB;
    public TMP_Text uiFeedbackTMP;


    public float maxTranslationSpeed = 100f;
    public float accelerationRate = 200f;
    public float decelerationRate = 200f;
    public float maxDistance;
    private Transform myTransform;
    float zPosition;
    [SerializeField]
    private float currentTranslationSpeed = 0f;
    private Vector3 initialPositionB;

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
        Move();
        WriteValue();
        uiFeedbackTMP.text = factoryMachineID + ":" + dataFromOPCUANode;
        if (float.TryParse(dataFromOPCUANode, out float parsedData))
        {
            fixedData = parsedData / 100f * -1;
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

        }

        else
        {
            // ͨ��λ��ֵ����B��λ��
            partB.localPosition = new Vector3(partB.localPosition.x, partB.localPosition.y, displacement);
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
        //���յ���moveData��ֵΪ1ʱ����ʼ�ƶ�
        if (moveData.Equals("1"))
        {
            //���ٶ�
            currentTranslationSpeed = Mathf.MoveTowards(currentTranslationSpeed, maxTranslationSpeed, accelerationRate * Time.deltaTime);

            // ��Z��ƽ��
            partB.transform.Translate(Vector3.forward * currentTranslationSpeed * Time.deltaTime * -1);

            // ����Ƿ�ﵽ�����룬����ﵽ��������ֹͣ
            if (Mathf.Abs((partB.transform.position.z - initialPositionB.z) * -1) >= maxDistance)
            {
                currentTranslationSpeed = 0f;
                accelerationRate = 0f;
            }
        }
        else
        {
            // ����
            currentTranslationSpeed = Mathf.MoveTowards(currentTranslationSpeed, 0f, decelerationRate * Time.deltaTime);

            // ��Z��ƽ��
            partB.transform.Translate(Vector3.forward * currentTranslationSpeed * Time.deltaTime * -1);
        }
    }
}

