using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public Transform targetObject;

    void Update()
    {
        if (targetObject != null)
        {
            // ģ��Ŀ�������λ��
            transform.position = targetObject.position;

            // ģ��Ŀ���������ת
            transform.rotation = targetObject.rotation;
        }
    }
}
