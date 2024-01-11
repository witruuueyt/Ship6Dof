using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target;  // ָ����Ŀ���

    void Update()
    {
        if (target != null)
        {
            Vector3 lookDirection = target.position - transform.position;

            // ���㳯��Ŀ������ת
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            // Ӧ����ת��ʹX�ᳯ��Ŀ���
            transform.rotation = targetRotation;
        }
    }
}
