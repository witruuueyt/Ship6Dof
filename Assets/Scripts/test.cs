using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    public Vector3 localAxisDof = new Vector3(0, 0, 1);  // ��ת��
    public float rotationSpeed = 45f;  // ��ת�ٶ�

    bool rotateEnabled = false;  // ����Ƿ�������ת

    void Update()
    {
        // ���ո���Ƿ���
        if (Input.GetKey(KeyCode.Space))
        {
            rotateEnabled = true;
        }

        // ��������ת�������ִ����ת
        if (rotateEnabled)
        {
            // �������ľֲ���ת
            Quaternion additionalRotation = Quaternion.Euler(localAxisDof * rotationSpeed * Time.deltaTime);

            // ������ľֲ���ת��ԭʼ�ľֲ���ת���
            transform.localRotation *= additionalRotation;
        }
    }
}
