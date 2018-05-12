using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevolverGunC : GunC {
    [Header("--特殊攻击信息--")]
    #region 特殊攻击内容【策划用】
    //特殊消耗的MP
    [SerializeField] private float m_SpecialComsumeMP;
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
    //子弹散射度数
    [SerializeField] private int m_SpecialScatter;
    //子弹预设
    [SerializeField] private GameObject m_SpecialButtle;
    //当前特殊攻击是否可用
    [SerializeField] private bool m_SpecialEnable;
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

    /// <summary>
    /// 左键普通攻击
    /// </summary>
    protected override void LeftNormalShot()
    {
        base.LeftNormalShot();
    }


    /// <summary>
    /// 右键点射 特殊攻击【2s的子弹时间，这段时间内除玩家以外的东西（敌人）速度都变成原来的0.5倍，会有一个20%的伤害提升】
    /// </summary>
    protected override void RightNormalShot()
    {
        base.RightNormalShot();

        //todo 时间控制

        //
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
        //子弹速度
        Gun_Data.SpecialButtleSpeed = m_SpecialButtleSpeed;
        //特殊攻击距离
        Gun_Data.SpecialAttackDistance = m_SpecialAttackDistance;
        //子弹散射度数
        Gun_Data.SpecialScatter = m_SpecialScatter;
        //子弹预设
        Gun_Data.SpecialButtle = m_SpecialButtle;
        //当前特殊攻击是否可用
        Gun_Data.SpecialEnable = m_SpecialEnable;
    }
    #endregion
}
