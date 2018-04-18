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
    FixedFire = 0,//点射
    ContinueFire = 1,//连续射
    EnergyFire = 2,//蓄能射击
    CloseFire = 3//近战攻击
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
    //枪械的涉及类型
    protected ShotType[] m_shotType;
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
    private uint m_DemageNums;
    public uint DemageNums
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
    private int m_MaxEnergyTime;
    public int MaxEnergyTime
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
    private uint m_AttackDistance;
    public uint AttackDistance
    {
        set { m_AttackDistance = value; }
        get { return m_AttackDistance; }
    }
    //子弹预设
    private GameObject m_buttle;
    public GameObject Buttle
    {
        set { m_buttle = value; }
        get { return m_buttle; }
    }
    //枪口位置
    private GameObject m_MuzzlePos;
    public GameObject MuzzlePos
    {
        set { m_MuzzlePos = value; }
        get { return m_MuzzlePos; }
    }
    //特殊-----------------------------------------------
    //特殊消耗的MP
    /* private int m_SpecialComsumeMP;
     //特殊消耗的HP
     private int m_SpecialComsumeHP;
     //特殊伤害数值
     private int m_SpecialDemageNums;
     //硬直系数
     private float m_SpecialHardStraight;
     //击退系数
     private float m_SpecialBeatBack;
     //攻击距离
     private float m_SpecialAttackDistance;
     //攻击频率
     private float m_SpecialAttackFrequency;
     //特殊攻击方式
     public abstract void SpecialAttackWay();
     */

    //武器最多点数
    private uint m_MaxPoint;

    //武器当前的点数
    private uint m_CurrentPoint;

     
}


