using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using Chronos;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

/// <summary>
/// 时间管理
/// </summary>
public class TimeManager : Singleton<TimeManager>
{
    //时钟
    public GlobalClock RootGlobalClock, InterFaceGlobalClock, PlayGlobalClock, EnemyGlobalClock;

    //UI界面
    [SerializeField] private GameObject _RepairStationUI;
    [SerializeField] private GameObject _EscUI;
    [SerializeField] private GameObject MainMenuUI;
    [SerializeField] private GameObject HelpUI;

    /// <summary>
    /// 是否暂停游戏中
    /// </summary>
    public bool IsStopGame;

    /// <summary>
    /// 是否在维修站附近
    /// </summary>
    public bool InRepairStation;

    void Start()
    {
        InRepairStation = false;
        IsStopGame = false;

        StartCoroutine(StartOpenUI());
    }

    IEnumerator StartOpenUI()
    {
        yield return new WaitForSeconds(0.5f);
        _RepairStationUI.SetActive(true);
        _EscUI.SetActive(true);
        MainMenuUI.SetActive(true);
        HelpUI.SetActive(true);
    }

    void Update()
    {
        //按下ESC
        //在维修站旁边
        //不在维修站旁边
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //停止游戏
            if (!IsStopGame)
            {
                StopGame();
            }
            //放回游戏
            else
            {
                BackToGame();
            }
        }
    }


    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void StopGame()
    {
        IsStopGame = true;

        if (InRepairStation)
        {
            _RepairStationUI.transform.GetChild(0).gameObject.SetActive(true);
            _RepairStationUI.GetComponent<RepairStationUI>().GetInfo();
        }
        else
        {
            _EscUI.transform.GetChild(0).gameObject.SetActive(true);
            _EscUI.transform.GetComponent<RepairStationUI>().GetInfo();
        }

        //关闭武器系统
        WeaponManager.Instance.ChangeWeaponManagerState(false);
        //游戏状态暂停
        GameManager.Instance.ChangeGameStatu(GameManager.GameStatu.Pause);
        //相机运动暂停
        Camera.main.GetComponent<ProCamera2D>().enabled = false;

        //时间暂停
        RootGlobalClock.localTimeScale = 0;
    }

    /// <summary>
    /// 放回游戏
    /// </summary>
    public void BackToGame()
    {
        print("fan回游戏");
        IsStopGame = false;
        //开启武器系统
        WeaponManager.Instance.ChangeWeaponManagerState(true);
        //游戏状态开启
        GameManager.Instance.ChangeGameStatu(GameManager.GameStatu.Normal);
        //相机运动暂停
        Camera.main.GetComponent<ProCamera2D>().enabled = true;
        //UI内容保存
        if (InRepairStation)
        {
            _RepairStationUI.GetComponent<RepairStationUI>().BackToGame();
            //_EscUI.GetComponent<RepairStationUI>().BackToGame();
        }


        //时间恢复
        RootGlobalClock.localTimeScale = 1;

        _EscUI.transform.GetChild(0).gameObject.SetActive(false);
        _RepairStationUI.transform.GetChild(0).gameObject.SetActive(false);
    }



    #region 时间延缓(左轮专用)

    /// <summary>
    /// 时间延缓(左轮专用)
    /// </summary>
    /// <param name="DelayTime">延缓间隔</param>
    public void DelayTime(float DelayTime)
    {
        PlayGlobalClock.localTimeScale = 0.8f;
        EnemyGlobalClock.localTimeScale = 0.5f;
        StartCoroutine(DelayTimine(DelayTime));
    }

    /// <summary>
    /// 左轮时间延缓持续时间
    /// </summary>
    /// <returns></returns>
    IEnumerator DelayTimine(float delayTime)
    {
        float timeing = 0;
        while (true)
        {
            timeing += Time.deltaTime;
            yield return null;
            if (timeing >= delayTime)
            {
                break;
            }
        }

        PlayGlobalClock.localTimeScale = 0.8f;
        EnemyGlobalClock.localTimeScale = 0.5f;
    }

    #endregion


    #region 接口
    /// <summary>
    /// 玩家状态
    /// </summary>
    /// <param name="state">true为复活，false死亡</param>
    public void PlayerState(bool state)
    {
        //关闭武器系统
        WeaponManager.Instance.ChangeWeaponManagerState(state);
        //相机运动暂停
        Camera.main.GetComponent<ProCamera2D>().enabled = state;

        //复活
        if (state)
        {
            GameManager.Instance.ChangeGameStatu(GameManager.GameStatu.Normal);
            RootGlobalClock.localTimeScale = 1;
        }
        else
        {
            GameManager.Instance.ChangeGameStatu(GameManager.GameStatu.GameOver);
            RootGlobalClock.localTimeScale = 0;
        }
    }
    
    
    #endregion

}
