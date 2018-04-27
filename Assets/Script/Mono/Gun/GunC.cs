﻿using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Assets.Script;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/15
****	描述 枪控制基础类
**********************************************************************/
public abstract class GunC : MonoBehaviour
{
    /// <summary>
    /// 角色对象
    /// </summary>
    [SerializeField] private GameObject m_player;
    /// <summary>
    /// 角色对象控制器
    /// </summary>
    protected PlayerRobotContral m_playerRobotContral;
    /// <summary>
    /// 射击准心
    /// </summary>
    public GameObject Target;
    #region 枪械信息（公开）
    [Header("--枪械信息--")]
    //枪械索引d 
    [SerializeField] private uint m_GunIndex;
    //枪械名字
    [SerializeField] private string m_GunName;
    //枪拥有的射击类型
    [SerializeField] private ShotType[] ShotType;
    //枪口位置
    [SerializeField] private GameObject m_MuzzlePos;
    //普通攻击-----------------------------------------------
    [Header("--普通攻击信息--")]
    //普通消耗的MP
    [SerializeField] private int m_ComsumeMP;
    //普通消耗的HP
    [SerializeField] private int m_ComsumeHP;
    //普通伤害数值
    [SerializeField] private float m_DemageNums;
    //硬直系数
    [SerializeField] private float m_HardStraight;
    //击退系数
    [SerializeField] private float m_BeatBack;
    //攻击距离
    [SerializeField] private float m_AttackDistance;
    //攻击频率CD
    [SerializeField] private float m_AttackCD;
    //子弹类型
    [SerializeField] private GameObject m_buttle;
    //子弹速度
    [SerializeField] private uint m_ButtleSpeed;
    //子弹散射度数
    [SerializeField] private int m_Scatter;
    //最大蓄能时间
    [SerializeField] private float m_MaxEnergyTime;
    //--------------------------------------------------
    #endregion
    /// <summary>
    /// 枪的所有具体数据
    /// </summary>
    protected GunM Gun_Data;

    /// <summary>
    /// 鼠标位置
    /// </summary>
    private Vector3 m_mousePos;
    /// <summary>
    /// 瞄准的世界坐标位置
    /// </summary>
    private Vector3 m_aimPos;
    /// <summary>
    /// 鼠标左右键点击的各种状态判断
    /// </summary>
    private bool LeftOnce, RightOnce, LeftDowning, RightDowning, LeftDownUping, RightDownUping;//普通点击、按住和松开
    private bool LeftDownEnergy, RightDownEnergy, LeftDownUpingEnergy, RightDownUpingEnergy;//蓄能按住和松开

    /// <summary>
    /// //鼠标点击键位类型
    /// </summary>
    private enum MouseClick
    {
        MouseLeft = 0,//左键
        MouseRight = 1,//右键
        MouseMiddle = 2//中键
    }

    protected virtual void Awake()
    {
        LeftOnce = RightOnce = LeftDowning = RightDowning = LeftDownEnergy = RightDownEnergy = LeftDownUping = RightDownUping = LeftDownUpingEnergy = RightDownUpingEnergy = false;//点击
        Gun_Data = new GunM();//枪数据

        //保存枪械信息
        SaveGunData();

        //获取角色控制类
        m_playerRobotContral = m_player.GetComponent<PlayerRobotContral>();
    }

    protected virtual void Update()
    {
        //枪方向-----------------------------------------------------------------
        m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_aimPos = new Vector3(m_mousePos.x, m_mousePos.y, transform.position.z);
        Target.transform.position = m_aimPos;
        float z;
        if (m_aimPos.y > transform.position.y)//旋转角度
            z = -Vector3.Angle(Vector3.left, m_aimPos - transform.position);
        else
            z = Vector3.Angle(Vector3.left, m_aimPos - transform.position);
        if (m_aimPos.x > transform.position.x)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
        transform.localRotation = Quaternion.Euler(0, 0, z);

        //鼠标按下监听
        ListenMouseEvent();
    }

    protected virtual void FixedUpdate()
    {
        //鼠标点击事件判断与实现
        JudgeMouseEvent();
    }

    /// <summary>
    /// 鼠标点击事件监听
    /// </summary>
    private void ListenMouseEvent()
    {
        foreach (ShotType ST in Gun_Data.ShotType)//抢的设射击类型
        {
            switch (ST)
            {
                //左边
                case global::ShotType.LeftFixedFire://点射（点一下）
                    ShotFixedFire(MouseClick.MouseLeft);
                    break;
                case global::ShotType.LeftContinueFire://点住不放（一发一发）
                    ShotContinueFire(MouseClick.MouseLeft);
                    break;
                case global::ShotType.LeftEnergyFire://点住不放（蓄能）
                    ShotEnergyFire(MouseClick.MouseLeft);
                    break;
                case global::ShotType.LeftCloseFire://近战攻击
                    ShotCloseFire(MouseClick.MouseLeft);
                    break;
                //右边
                case global::ShotType.RightFixedFire://点射（点一下）
                    ShotFixedFire(MouseClick.MouseRight);
                    break;
                case global::ShotType.RightContinueFire://点住不放（一发一发）
                    ShotContinueFire(MouseClick.MouseRight);
                    break;
                case global::ShotType.RightEnergyFire://点住不放（蓄能）
                    ShotEnergyFire(MouseClick.MouseRight);
                    break;
                case global::ShotType.RightCloseFire://近战攻击
                    ShotCloseFire(MouseClick.MouseRight);
                    break;
            }
        }
    }
    /// <summary>
    /// 点射（点一下）攻击判断
    /// </summary>
    /// <param name="MC">鼠标点击键</param>
    private void ShotFixedFire(MouseClick MC)
    {
        if (MC == MouseClick.MouseLeft) //左键
        {
            if (Input.GetMouseButtonDown(0))
                LeftOnce = true;
        }
        else if (MC == MouseClick.MouseRight) //右键
        {
            if (Input.GetMouseButtonDown(1))
                RightOnce = true;
        }
        else if (MC == MouseClick.MouseMiddle) //中键
        {

        }
    }
    //点住不放（一发一发）
    private const float CHICK_INTERVAL = 0.3f;//按一次和长按的时间间隔
    private float m_LeftDownListener = 0, m_RightDownListener = 0;//左右键按住监听
    /// <summary>
    /// 按住不放攻击判断
    /// </summary>
    /// <param name="MC">鼠标点击键</param>
    private void ShotContinueFire(MouseClick MC)
    {
        if (MC == MouseClick.MouseLeft) //左键
        {
            if (Input.GetMouseButton(0))//按住
            {
                m_LeftDownListener+= Time.deltaTime;
                if (m_LeftDownListener > CHICK_INTERVAL)//确认是长按
                {
                    LeftDowning = true;//确认是长按
                    LeftOnce = false;//确认不是按一下
                }
            }
            if (LeftDowning && Input.GetMouseButtonUp(0))//抬起
            {
                m_LeftDownListener = 0;
                LeftDownUping = true;
                LeftDowning = false;
            }
        }
        else if (MC == MouseClick.MouseRight) //右键
        {
            if (Input.GetMouseButton(1))//按住
            {
                m_RightDownListener += Time.deltaTime;
                if (m_RightDownListener > CHICK_INTERVAL)//确认是长按
                {
                    RightDowning = true;//确认是长按
                    RightOnce = false;//确认不是按一下
                }
            }
            if (RightDowning && Input.GetMouseButtonUp(1))//抬起
            {
                m_RightDownListener = 0;
                RightDownUping = true;
                RightDowning = false;
            }
        }
        else if (MC == MouseClick.MouseMiddle) //中键
        {

        }
    }
    //点住不放（蓄能）
    private float m_LeftEnergyTime;
    protected float LeftEnergyTime//左键蓄能时间
    {
        set
        {
            if (value > Gun_Data.MaxEnergyTime)
                m_LeftEnergyTime = Gun_Data.MaxEnergyTime;
            else
            {
                m_LeftEnergyTime = value;
            }
        }
        get { return m_LeftEnergyTime; }
    }
    private float m_RightEnergyTime;
    protected float RightEnergyTime//右键（特殊）蓄能时间
    {
        set
        {
            if (value > Gun_Data.SpecialMaxEnergyTime)
                m_RightEnergyTime = Gun_Data.SpecialMaxEnergyTime;
            else
            {
                m_RightEnergyTime = value;
            }
        }
        get { return m_RightEnergyTime; }
    }
    /// <summary>
    /// 蓄能攻击判断
    /// </summary>
    /// <param name="MC">鼠标点击键</param>
    private void ShotEnergyFire(MouseClick MC)
    {
        if (MC == MouseClick.MouseLeft) //左键
        {
            if (Input.GetMouseButton(0)) //按住
            {
                LeftEnergyTime += Time.deltaTime;
                LeftDownEnergy = true;
            }
            if (LeftDownEnergy && Input.GetMouseButtonUp(0)) //抬起
            {
                //LeftEnergyTime = 0;(置0 调到if()中)
                LeftDownUpingEnergy = true;
                LeftDownEnergy = false;
            }
        }
        else if (MC == MouseClick.MouseRight) //右键
        {
            if (Input.GetMouseButton(1)) //按住
            {
                RightEnergyTime += Time.deltaTime;
                RightDownEnergy = true;
            }
            if (RightDownEnergy && Input.GetMouseButtonUp(1)) //抬起
            {
                //RightEnergyTime = 0;(置0 调到if()中)
                RightDownUpingEnergy = true;
                RightDownEnergy = false;
            }
        }
        else if (MC == MouseClick.MouseMiddle) //中键
        {

        }
    }
    /// <summary>
    /// 近战攻击判断
    /// </summary>
    /// <param name="MC">鼠标点击键</param>
    private void ShotCloseFire(MouseClick MC)
    {
        if (MC == MouseClick.MouseLeft) //左键
        {

        }
        else if (MC == MouseClick.MouseRight) //右键
        {

        }
        else if (MC == MouseClick.MouseMiddle) //中键
        {

        }
    }

    /// <summary>
    /// 鼠标点击事件判断与实现
    /// </summary>
    private void JudgeMouseEvent()
    {
        if (LeftOnce)
        {
            LeftNormalShot();
            LeftOnce = false;
        }
        if (RightOnce)
        {
            RightNormalShot();
            RightOnce = false;
        }
        if (LeftDowning)
        {
            LeftNormalShot();
        }
        if (RightDowning)
        {
            RightNormalShot();
        }
        if (LeftDownUping)
        {
            LeftContinueShotUping();
            LeftDownUping = false;
            LeftDowning = false;

        }
        if (RightDownUping)
        {
            RightContinueShotUping();
            RightDownUping = false;
            RightDowning = false;
        }

        //蓄能
        if (LeftDownEnergy)
        {
            LeftEnergying();
        }
        if (LeftDownUpingEnergy)
        {
            LeftEnergyShot();
            LeftDownUpingEnergy = false;
            LeftEnergyTime = 0;//左键蓄能累积时间置0（初始化）
        }
        if (RightDownEnergy)
        {
            RightEnergying();
        }
        if (RightDownUpingEnergy)
        {
            RightEnergyShot();
            RightDownUpingEnergy = false;
            RightEnergyTime = 0;//右键(特殊)蓄能累积时间置0（初始化）
        }
    }

    /// <summary>
    /// 左键普通点射攻击
    /// </summary>
    protected virtual void LeftNormalShot()
    {
        //还没达到CD时间
        if (!CanShotNext) return;
        StartCoroutine(ShotCD());
        //生成子弹（调整位置与角度）
        GameObject buttleGameObject = ObjectPool.Instance.Spawn(Gun_Data.Buttle.name);
        buttleGameObject.transform.position = Gun_Data.MuzzlePos.transform.position;
        buttleGameObject.transform.rotation = Quaternion.Euler(Gun_Data.MuzzlePos.transform.rotation.eulerAngles + new Vector3(0,0, GenerateNormalScatteringNums()));
        //子弹数据填充
        Buttle buttle = buttleGameObject.GetComponent<Buttle>();
        buttle.BulletStart(Gun_Data.ButtleSpeed, Gun_Data.AttackDistance, Gun_Data.DemageNums);//子弹初始化（速度、距离、伤害）
        //角色MPHP减少
        PlayerMPHPChange(Gun_Data.ComsumeMP, Gun_Data.ComsumeHP);
    }
    //左键普通开枪射击CD判断
    private float m_Current = 0;//当前射击的CD
    private bool CanShotNext = true;//达到CD时间，可以射击下一回合
    IEnumerator ShotCD()
    {
        CanShotNext = false;
        while (m_Current < Gun_Data.AttackCD)
        {
            m_Current += Time.fixedDeltaTime;
            yield return null;
        }
        m_Current = 0;
        CanShotNext = true;
    }


    /// <summary>
    /// 右键普通点射攻击
    /// </summary>
    protected virtual void RightNormalShot() { }

    /// <summary>
    /// 左键连续攻击后抬起操作
    /// </summary>
    protected virtual void LeftContinueShotUping() { }
    /// <summary>
    /// 右键连续攻击后抬起操作
    /// </summary>
    protected virtual void RightContinueShotUping() { }

    /// <summary>
    /// 左蓄能过程
    /// </summary>
    protected virtual void LeftEnergying() { }
    /// <summary>
    /// 右蓄能过程
    /// </summary>
    protected virtual void RightEnergying() { }
    /// <summary>
    /// 左蓄能攻击
    /// </summary>
    protected virtual void LeftEnergyShot() { }
    /// <summary>
    /// 右蓄能攻击
    /// </summary>
    protected virtual void RightEnergyShot() { }


    /// <summary>
    /// 超级特殊技能？？？
    /// </summary>
    protected virtual void SpecialShot(){ }


    /// <summary>
    /// 散射随机值生成
    /// 普通散射
    /// </summary>
    /// <returns>普通散射角度的随机值</returns>
    protected virtual float GenerateNormalScatteringNums()
    {
        return Random.Range(Gun_Data.Scatter * -1.0f, Gun_Data.Scatter);
    }

    /// <summary>
    /// 散射随机值生成
    /// 特殊散射
    /// </summary>
    /// <returns>特殊散射角度的随机值</returns>
    protected virtual float GeneratSpecialScatteringNums(){ return 0;}



    #region 枪械信息（保存）
    /// <summary>
    /// 枪械信息（保存）
    /// </summary>
    private void SaveGunData()
    {
        Gun_Data.Buttle = m_buttle;
        //枪械索引d 
        Gun_Data.GunIndex = m_GunIndex;
        //枪械名字
        Gun_Data.GunName = m_GunName;
        //普通攻击-----------------------------------------------
        //普通消耗的MP
        Gun_Data.ComsumeMP = m_ComsumeMP;
        //普通消耗的HP
        Gun_Data.ComsumeHP = m_ComsumeHP;
        //普通伤害数值
        Gun_Data.DemageNums = m_DemageNums;
        //硬直系数
        Gun_Data.HardStraight = m_HardStraight;
        //击退系数
        Gun_Data.BeatBack = m_BeatBack;
        //攻击距离
        Gun_Data.AttackDistance = m_AttackDistance;
        //攻击频率CD
        Gun_Data.AttackCD = m_AttackCD;
        //子弹类型
        Gun_Data.Buttle = m_buttle;
        //最大蓄能时间
        Gun_Data.MaxEnergyTime = m_MaxEnergyTime;
        //枪口坐标
        Gun_Data.MuzzlePos = m_MuzzlePos;
        //子弹速度
        Gun_Data.ButtleSpeed = m_ButtleSpeed;
        //设计类型
        Gun_Data.ShotType = ShotType;
        //子弹散射度数
        Gun_Data.Scatter = m_Scatter;
    }
    #endregion


    /// <summary>
    /// 角色MP与HP信息传递
    /// </summary>
    /// <param name="comsumeMp">消耗的MP</param>
    /// <param name="comsumeHp">消耗的HP</param>
    protected void PlayerMPHPChange(float comsumeMp,float comsumeHp)
    {
        m_playerRobotContral.GetDamage((int) comsumeMp, (int) comsumeHp);
    }


    /// <summary>
    /// 核心状态改变监听器
    /// </summary>
    /// <param name="currentCore">角色当前的核心</param>
    public void CoreChangeLister(BaseCore currentCore)
    {

    }
    
}
