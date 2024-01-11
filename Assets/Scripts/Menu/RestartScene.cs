using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    void Start()
    {
        // ��ȡ��ť���
        Button button = GetComponent<Button>();

        // ��Ӱ�ť����¼�������
        button.onClick.AddListener(RestartThisScene);
    }

    public void RestartThisScene()
    {
        // ��ȡ��ǰ����������
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���¼��ص�ǰ����
        SceneManager.LoadScene(currentSceneIndex);
    }
}
