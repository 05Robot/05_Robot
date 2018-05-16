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
    [Rename("消耗MP/发")] [SerializeField]public float m_SpecialComsumeMP;
    //特殊消耗的HP
    [Rename("消耗HP/发")] [SerializeField] private float m_SpecialComsumeHP;
    //特殊伤害数值
    [Rename("伤害/发")] [SerializeField] private float m_SpecialDemageNums;
    //特殊硬直系数
    [Rename("硬直系数")] [SerializeField] private float m_SpecialHardStraight;
    //特殊击退系数
    [Rename("击退系数")] [SerializeField] private float m_SpecialBeatBack;
    //特殊攻击频率CD
    [Rename("射击频率(s)（攻击速度）")] [SerializeField] private float m_SpecialAttackCD;
    //特殊最大蓄能时间
    [Rename("最大蓄能时间")] [SerializeField] private float m_SpecialMaxEnergyTime;
    //特殊攻击中：和子弹 或者其他 预设有关的
    //子弹散射度数
    [Rename("散射度数")] [SerializeField] private int m_SpecialScatter;
    //子弹速度
    [Rename("子弹速度（单位/s）")] [SerializeField] private uint m_SpecialButtleSpeed;
    //特殊攻击距离
    [Rename("子弹距离")] [SerializeField] private float m_SpecialAttackDistance;
    //子弹预设
    [Rename("子弹预设")] [SerializeField] private GameObject m_SpecialButtle;
    //特殊攻击是否可用/开启
    [Rename("特殊攻击是否可用")] [SerializeField] private bool m_SpecialEnable;
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


    //------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 左键普通蓄能过程
    /// </summary>
    protected override void LeftEnergying()
    {
        base.LeftEnergying();
        //瞄准Target数值变换
        if (AWMCanShotNext)//如果可以射击
        {
            //【是否为普通攻击  && 已经开启可以用】
            if (!(Gun_Data.GunState == GunState.NormalState && Gun_Data.Enable))
            {
                CanSpecialShotNext = false;
                return;
            }
            Target.GetComponent<Target>().ChangeTargetSlider(Gun_Data.MaxEnergyTime, base.LeftEnergyTime);
        }
        else//如果不能射击
        {
            base.LeftEnergyTime = 0;
        }
    }
    /// <summary>
    /// 左键普通蓄能攻击
    /// 1、消耗MP：20+60t
    /// 2、消耗HP：16+48t
    /// 3、伤害加成：200+200t
    /// </summary>
    protected override void LeftEnergyShot()
    {
        base.LeftEnergyShot();
        // 如果可以射击【达到CD时间】
        if (AWMCanShotNext) //如果可以射击
            Target.GetComponent<Target>().Init(); //瞄准Target数值归0
        else
            return;
        //-------------------------------------------------------
        //开始计时
        StartCoroutine(ShotCD());
        //-------------------------------------------------------
        //生成子弹（调整位置与角度）
        WeaponManager.Instance.GenerateNormalButton(Gun_Data.Buttle.name,
            Gun_Data.MuzzlePos.transform.position, Gun_Data.MuzzlePos.transform.rotation.eulerAngles, Gun_Data.Scatter,
            Gun_Data.ButtleSpeed, Gun_Data.AttackDistance, Gun_Data.DemageNums + base.LeftEnergyTime * 200f);

        //角色MPHP减少
        PlayerMPHPChange(Gun_Data.ComsumeMP + base.LeftEnergyTime * 60.0f,
            Gun_Data.ComsumeHP + base.LeftEnergyTime * 48.0f);
    }

    private float m_AWMCurrent = 0; //当前射击的CD
    private bool AWMCanShotNext = true; //达到CD时间，可以射击下一回合
    /// <summary>
    /// 普通攻击CD检测函数
    /// </summary>
    /// <returns></returns>
    IEnumerator ShotCD()
    {
        AWMCanShotNext = false;
        while (m_AWMCurrent < Gun_Data.AttackCD)
        {
            m_AWMCurrent += Time.deltaTime;
            yield return null;
        }
        m_AWMCurrent = 0;
        AWMCanShotNext = true;
    }


    //-----------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 右键点射(特殊攻击)
    /// </summary>
    protected override void RightNormalShot()
    {
        base.RightNormalShot();
        //还没达到CD时间
        if (!CanSpecialShotNext) return;
        StartCoroutine(SpecialShotCD());
        //生成子弹（调整位置与角度）
        GameObject SpecialButtleGameObject = ObjectPool.Instance.Spawn(Gun_Data.SpecialButtle.name);
        SpecialButtleGameObject.transform.position = Gun_Data.MuzzlePos.transform.position;
        SpecialButtleGameObject.transform.rotation = Gun_Data.MuzzlePos.transform.rotation;
        //子弹数据填充
        Buttle buttle = SpecialButtleGameObject.GetComponent<Buttle>();
        buttle.BulletStart(Gun_Data.SpecialButtleSpeed, Gun_Data.SpecialAttackDistance, Gun_Data.SpecialDemageNums);
        //角色MPHP减少
        PlayerMPHPChange(Gun_Data.SpecialComsumeMP, Gun_Data.ComsumeHP);
    }
    private float m_SpecialCurrent = 0;//当前射击的CD
    private bool CanSpecialShotNext = true;//达到CD时间，可以射击下一回合
    /// <summary>
    /// 特殊攻击CD检测函数
    /// </summary>
    /// <returns></returns>
    IEnumerator SpecialShotCD()
    {
        CanSpecialShotNext = false;
        while (m_SpecialCurrent < Gun_Data.SpecialAttackCD)
        {
            m_SpecialCurrent += Time.deltaTime;
            yield return null;
        }
        m_SpecialCurrent = 0;
        CanSpecialShotNext = true;
    }
    #region 枪械特殊信息（保存）
    /// <summary>
    /// 枪械特殊信息（保存）
    /// </summary>
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
        //子弹散射度数
        Gun_Data.SpecialScatter = m_SpecialScatter;
        //子弹速度
        Gun_Data.SpecialButtleSpeed = m_SpecialButtleSpeed;
        //特殊攻击距离
        Gun_Data.SpecialAttackDistance = m_SpecialAttackDistance;
        //子弹预设
        Gun_Data.SpecialButtle = m_SpecialButtle;
        //普通攻击是否可用/开启
        Gun_Data.SpecialEnable = m_SpecialEnable;
    }

    #endregion
}
