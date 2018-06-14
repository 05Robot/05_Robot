using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneCanvas : MonoBehaviour {

    public void LoadingNextLevel(string nextLevelName)//加载loading场景
    {
        SceneManager.LoadScene(nextLevelName);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="closeGameObject">窗口</param>
    public void Close(GameObject closeGameObject)
    {
        closeGameObject.SetActive(false);
    }

    /// <summary>
    /// 开启窗口
    /// </summary>
    /// <param name="closeGameObject">窗口</param>
    public void Open(GameObject openGameObject)
    {
        openGameObject.SetActive(true);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitAllGame()
    {
        Application.Quit();
    }
}
