using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/14
****	描述 枪械基类
**********************************************************************/
public enum ShotType//射击类型
{
    //左边
    LeftFixedFire = 0,//点射
    LeftContinueFire = 1,//按住连续射
    LeftEnergyFire = 2,//蓄能射击
    LeftCloseFire = 3,//近战攻击
    //右边
    RightFixedFire = 4,
    RightContinueFire = 5,
    RightEnergyFire = 6,
    RightCloseFire = 7
}

public class GunM
{
    //枪械索引d 
    private uint m_GunIndex;
    public uint GunIndex
    {
        set { m_GunIndex = value; }
        get { return m_GunIndex; }
    }
    //枪械名字
    private string m_GunName;
    public string GunName
    {
        set { m_GunName = value; }
        get { return m_GunName; }
    }
    //枪口位置
    private GameObject m_MuzzlePos;
    public GameObject MuzzlePos
    {
        set { m_MuzzlePos = value; }
        get { return m_MuzzlePos; }
    }
    //枪械的涉及类型
    public ShotType[] ShotType;
    //普通-----------------------------------------------
    //普通消耗的MP
    private int m_ComsumeMP;
    public int ComsumeMP
    {
        set { m_ComsumeMP = value; }
        get { return m_ComsumeMP; }
    }
    //普通消耗的HP
    private int m_ComsumeHP;
    public int ComsumeHP
    {
        set { m_ComsumeHP = value; }
        get { return m_ComsumeHP; }
    }
    //普通伤害数值
    private float m_DemageNums;
    public float DemageNums
    {
        set { m_DemageNums = value; }
        get { return m_DemageNums; }
    }
    //硬直系数
    private float m_HardStraight;
    public float HardStraight
    {
        set { m_HardStraight = value; }
        get { return m_HardStraight; }
    }
    //击退系数
    private float m_BeatBack;
    public float BeatBack
    {
        set { m_BeatBack = value; }
        get { return m_BeatBack; }
    }
    //攻击频率CD
    private float m_AttackCD;
    public float AttackCD
    {
        set { m_AttackCD = value; }
        get { return m_AttackCD; }
    }
    //最大蓄能时间
    private float m_MaxEnergyTime;
    public float MaxEnergyTime
    {
        set { m_MaxEnergyTime = value; }
        get { return m_MaxEnergyTime; }
    }

    //----和子弹有关
    //子弹速度
    private uint m_ButtleSpeed;
    public uint ButtleSpeed
    {
        set { m_ButtleSpeed = value; }
        get { return m_ButtleSpeed; }
    }
    //攻击距离
    private float m_AttackDistance;
    public float AttackDistance
    {
        set { m_AttackDistance = value; }
        get { return m_AttackDistance; }
    }
    //子弹散射度数
    private int m_Scatter;
    public int Scatter
    {
        set { m_Scatter = value; }
        get { return m_Scatter; }
    }
    //子弹预设
    private GameObject m_buttle;
    public GameObject Buttle
    {
        set { m_buttle = value; }
        get { return m_buttle; }
    }
   

    //特殊-----------------------------------------------
    //特殊消耗的MP
    private float m_SpecialComsumeMP;
    public float SpecialComsumeMP
    {
        set { m_SpecialComsumeMP = value; }
        get { return m_SpecialComsumeMP; }
    }
    //特殊消耗的HP
    private float m_SpecialComsumeHP;
    public float SpecialComsumeHP
    {
        set { m_SpecialComsumeHP = value; }
        get { return m_SpecialComsumeHP; }
    }
    //特殊伤害数值
    private float m_SpecialDemageNums;
    public float SpecialDemageNums
    {
        set { m_SpecialDemageNums = value; }
        get { return m_SpecialDemageNums; }
    }
    //特殊硬直系数
    private float m_SpecialHardStraight;
    public float SpecialHardStraight
    {
        set { m_SpecialHardStraight = value; }
        get { return m_SpecialHardStraight; }
    }
    //特殊击退系数
    private float m_SpecialBeatBack;
    public float SpecialBeatBack
    {
        set { m_SpecialBeatBack = value; }
        get { return m_SpecialBeatBack; }
    }
    //特殊攻击频率CD
    private float m_SpecialAttackCD;
    public float SpecialAttackCD
    {
        set { m_SpecialAttackCD = value; }
        get { return m_SpecialAttackCD; }
    }
    //特殊最大蓄能时间
    private float m_SpecialMaxEnergyTime;
    public float SpecialMaxEnergyTime
    {
        set { m_SpecialMaxEnergyTime = value; }
        get { return m_SpecialMaxEnergyTime; }
    }

    //特殊攻击中：和子弹 或者其他 预设有关的
    //子弹速度
    private uint m_SpecialButtleSpeed;
    public uint SpecialButtleSpeed
    {
        set { m_SpecialButtleSpeed = value; }
        get { return m_SpecialButtleSpeed; }
    }
    //特殊攻击距离
    private float m_SpecialAttackDistance;
    public float SpecialAttackDistance
    {
        set { m_SpecialAttackDistance = value; }
        get { return m_SpecialAttackDistance; }
    }
    //子弹散射度数
    private int m_SpecialScatter;
    public int SpecialScatter
    {
        set { m_SpecialScatter = value; }
        get { return m_SpecialScatter; }
    }
    //子弹预设
    private GameObject m_Specialbuttle;
    public GameObject SpecialButtle
    {
        set { m_Specialbuttle = value; }
        get { return m_Specialbuttle; }
    }

    //-------------------------------------------------------------------------------------------------
    //武器最多点数
    private uint m_MaxPoint;

    //武器当前的点数
    private uint m_CurrentPoint;

}


