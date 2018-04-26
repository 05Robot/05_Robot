using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/15
****	描述 AWM狙击枪控制类
**********************************************************************/
public class AWMGunC : GunC
{
    [Header("--特殊攻击信息--")]
    #region 特殊攻击内容

    //特殊消耗的MP
    [SerializeField]
    private float m_SpecialComsumeMP;

    //特殊消耗的HP
    [SerializeField] private float m_SpecialComsumeHP;

    //特殊伤害数值
    [SerializeField] private float m_SpecialDemageNums;

    //特殊硬直系数
    [SerializeField] private float m_SpecialHardStraight;

    //特殊击退系数
    [SerializeField] private float m_SpecialBeatBack;

    //特殊攻击频率CD
    [SerializeField] private float m_SpecialAttackCD;

    //特殊最大蓄能时间
    [SerializeField] private float m_SpecialMaxEnergyTime;

    //特殊攻击中：和子弹 或者其他 预设有关的
    //子弹速度
    [SerializeField] private uint m_SpecialButtleSpeed;

    //特殊攻击距离
    [SerializeField] private float m_SpecialAttackDistance;

    //子弹预设
    [SerializeField] private GameObject m_SpecialButtle;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        SaveGunSpecialData();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void LeftNormalShot()
    {
        base.LeftNormalShot();
    }

    //右键蓄能过程
    protected override void RightEnergying()
    {
        base.RightEnergying();
    }

    //右键蓄能攻击
    protected override void RightEnergyShot()
    {
        base.RightEnergyShot();
        //还没达到CD时间
        if (!CanSpecialShotNext) return;
        StartCoroutine(SpecialShotCD());
        //-------------------------------------------------------
        //生成子弹（调整位置与角度）
        GameObject SpecialButtleGameObject = ObjectPool.Instance.Spawn(Gun_Data.SpecialButtle.name);
        SpecialButtleGameObject.transform.position = Gun_Data.MuzzlePos.transform.position;
        SpecialButtleGameObject.transform.rotation = Gun_Data.MuzzlePos.transform.rotation;
        //蓄能效果(base.RightEnergyTime : 蓄能时间)
        // Gun_Data.SpecialComsumeMP + base.RightEnergyTime * 60.0f;
        // Gun_Data.SpecialComsumeHP + base.RightEnergyTime * 48.0f;
        //子弹数据填充
        Buttle buttle = SpecialButtleGameObject.GetComponent<Buttle>();
        buttle.BulletStart(
            Gun_Data.SpecialButtleSpeed,
            Gun_Data.SpecialAttackDistance,
            Gun_Data.SpecialDemageNums + base.RightEnergyTime * 200.0f);
        print(Gun_Data.SpecialDemageNums + base.RightEnergyTime * 200.0f);
    }

    private float m_SpecialCurrent = 0; //当前射击的CD
    private bool CanSpecialShotNext = true; //达到CD时间，可以射击下一回合
    IEnumerator SpecialShotCD()
    {
        CanSpecialShotNext = false;
        while (m_SpecialCurrent < Gun_Data.SpecialAttackCD)
        {
            m_SpecialCurrent += Time.fixedDeltaTime;
            yield return null;
        }

        m_SpecialCurrent = 0;
        CanSpecialShotNext = true;
    }

    #region 枪械特殊信息（保存）

    private void SaveGunSpecialData()
    {
        //特殊消耗的MP
        Gun_Data.SpecialComsumeMP = m_SpecialComsumeMP;
        //特殊消耗的HP
        Gun_Data.SpecialComsumeHP = m_SpecialComsumeHP;
        //特殊伤害数值
        Gun_Data.SpecialDemageNums = m_SpecialDemageNums;
        //特殊硬直系数
        Gun_Data.SpecialHardStraight = m_SpecialHardStraight;
        //特殊击退系数
        Gun_Data.SpecialBeatBack = m_SpecialBeatBack;
        //特殊攻击频率CD
        Gun_Data.SpecialAttackCD = m_SpecialAttackCD;
        //特殊最大蓄能时间
        Gun_Data.SpecialMaxEnergyTime = m_SpecialMaxEnergyTime;
        //特殊攻击中：和子弹 或者其他 预设有关的
        //子弹速度
        Gun_Data.SpecialButtleSpeed = m_SpecialButtleSpeed;
        //特殊攻击距离
        Gun_Data.SpecialAttackDistance = m_SpecialAttackDistance;
        //子弹预设
        Gun_Data.SpecialButtle = m_SpecialButtle;
    }

    #endregion
}
