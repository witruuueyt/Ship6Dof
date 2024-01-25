using UnityEngine;

public class Test2 : MonoBehaviour
{
    void Start()
    {
        // 获取物体的Mesh Renderer组件
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            // 获取物体在X轴上的长度
            float objectLengthX = meshRenderer.bounds.size.x;
            float objectLengthY = meshRenderer.bounds.size.y;
            float objectLengthZ = meshRenderer.bounds.size.z;
            // 输出长度
            Debug.Log("物体在X轴上的长度是：" + objectLengthX + "," + objectLengthY + "," + objectLengthZ);
        }
        else
        {
            Debug.LogError("物体没有Mesh Renderer组件，无法获取长度。");
        }
    }
}