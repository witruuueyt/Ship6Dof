using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartScene : MonoBehaviour
{
    void Start()
    {
        // 获取按钮组件
        Button button = GetComponent<Button>();

        // 添加按钮点击事件监听器
        button.onClick.AddListener(RestartThisScene);
    }

    public void RestartThisScene()
    {
        // 获取当前场景的索引
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 重新加载当前场景
        SceneManager.LoadScene(currentSceneIndex);
    }
}
