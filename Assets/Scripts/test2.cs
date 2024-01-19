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
            // 模仿目标物体的位置
            transform.position = targetObject.position;

            // 模仿目标物体的旋转
            transform.rotation = targetObject.rotation;
        }
    }
}
