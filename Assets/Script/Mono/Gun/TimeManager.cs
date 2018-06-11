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
            _RepairStationUI.SetActive(true);
        }
        else
        {
            _EscUI.SetActive(false);
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
        IsStopGame = false;
        //开启武器系统
        WeaponManager.Instance.ChangeWeaponManagerState(true);
        //游戏状态开启
        GameManager.Instance.ChangeGameStatu(GameManager.GameStatu.Normal);
        //相机运动暂停
        Camera.main.GetComponent<ProCamera2D>().enabled = true;
        //UI内容保存
        RepairStationUI.Instance.BackToGame();

        //时间恢复
        RootGlobalClock.localTimeScale = 1;

        _EscUI.SetActive(false);
        _RepairStationUI.SetActive(false);
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

}
