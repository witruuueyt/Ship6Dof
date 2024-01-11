using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    public Vector3 localAxisDof = new Vector3(0, 0, 1);  // 旋转轴
    public float rotationSpeed = 45f;  // 旋转速度

    bool rotateEnabled = false;  // 标记是否允许旋转

    void Update()
    {
        // 检测空格键是否按下
        if (Input.GetKey(KeyCode.Space))
        {
            rotateEnabled = true;
        }

        // 在允许旋转的情况下执行旋转
        if (rotateEnabled)
        {
            // 计算额外的局部旋转
            Quaternion additionalRotation = Quaternion.Euler(localAxisDof * rotationSpeed * Time.deltaTime);

            // 将额外的局部旋转与原始的局部旋转相乘
            transform.localRotation *= additionalRotation;
        }
    }
}
