using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour {
    /// <summary>
    /// 放回游戏主菜单
    /// </summary>
    public void BackToGameMenu(GameObject check)
    {
        check.SetActive(true);
    }
    /// <summary>
    /// 退出游戏放回桌面
    /// </summary>
    public void BackToDesk(GameObject check)
    {
        check.SetActive(true);
    }

    public void Yes(int index)
    {
        switch (index)
        {
            case 1:
                BackToGameMenuFunction();
                break;
            case 2:
                BackToDeskFunction();
                break;
        }
    }


    public void No(GameObject check)
    {
        check.SetActive(false);
    }


    /// <summary>
    /// 返回游戏主菜单功能
    /// </summary>
    private void BackToGameMenuFunction()
    {
        SceneManager.LoadScene(0);
    }
    /// <summary>
    /// 退出游戏放回桌面功能
    /// </summary>
    private void BackToDeskFunction()
    {
        Application.Quit();
    }

}
