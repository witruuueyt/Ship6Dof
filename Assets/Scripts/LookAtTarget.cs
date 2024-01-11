using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target;  // 指定的目标点

    void Update()
    {
        if (target != null)
        {
            Vector3 lookDirection = target.position - transform.position;

            // 计算朝向目标点的旋转
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            // 应用旋转，使X轴朝向目标点
            transform.rotation = targetRotation;
        }
    }
}
