using UnityEngine;

public class Test2 : MonoBehaviour
{
    void Start()
    {
        // ��ȡ�����Mesh Renderer���
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            // ��ȡ������X���ϵĳ���
            float objectLengthX = meshRenderer.bounds.size.x;
            float objectLengthY = meshRenderer.bounds.size.y;
            float objectLengthZ = meshRenderer.bounds.size.z;
            // �������
            Debug.Log("������X���ϵĳ����ǣ�" + objectLengthX + "," + objectLengthY + "," + objectLengthZ);
        }
        else
        {
            Debug.LogError("����û��Mesh Renderer������޷���ȡ���ȡ�");
        }
    }
}