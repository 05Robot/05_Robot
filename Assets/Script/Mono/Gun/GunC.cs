using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/15
****	描述 枪控制基础类
**********************************************************************/
public abstract class GunC : MonoBehaviour
{
    //瞄准测试
    public GameObject test;
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
    //最大蓄能时间
    [SerializeField] private float m_MaxEnergyTime;
    //--------------------------------------------------
    #endregion
    protected GunM Gun_Data;
    
    //鼠标位置
    private Vector3 m_mousePos;
    //瞄准的世界坐标位置
    private Vector3 m_aimPos;
    //鼠标左右键是否点击
    private bool LeftOnce, RightOnce, LeftDowning, RightDowning, LeftDownUping, RightDownUping;//普通点击、按住和松开
    private bool LeftDownEnergy, RightDownEnergy, LeftDownUpingEnergy, RightDownUpingEnergy;//蓄能按住和松开
    private enum MouseClick//鼠标点击类型
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
    }

    protected virtual void Update()
    {
        //枪方向-----------------------------------------------------------------
        m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_aimPos = new Vector3(m_mousePos.x, m_mousePos.y, transform.position.z);
        test.transform.position = m_aimPos;
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

    //鼠标点击事件监听
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
    //点射（点一下）
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
    //近战攻击
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

    //鼠标点击事件判断与实现
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


    //左键普通开枪射击
    protected virtual void LeftNormalShot()
    {
        //还没达到CD时间
        if (!CanShotNext) return;
        StartCoroutine(ShotCD());
        //生成子弹（调整位置与角度）
        GameObject buttleGameObject = ObjectPool.Instance.Spawn(Gun_Data.Buttle.name);
        buttleGameObject.transform.position = Gun_Data.MuzzlePos.transform.position;
        buttleGameObject.transform.rotation = Gun_Data.MuzzlePos.transform.rotation;
        //子弹数据填充
        Buttle buttle = buttleGameObject.GetComponent<Buttle>();
        buttle.BulletStart(Gun_Data.ButtleSpeed, Gun_Data.AttackDistance, Gun_Data.DemageNums);
    }
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

    //右键普通攻击
    protected virtual void RightNormalShot() { }

    //左右键连续攻击抬起
    protected virtual void LeftContinueShotUping() { }
    protected virtual void RightContinueShotUping() { }

    //左右蓄能过程与攻击
    protected virtual void LeftEnergying() { }
    protected virtual void RightEnergying() { }
    protected virtual void LeftEnergyShot() { }
    protected virtual void RightEnergyShot() { }


    //超级特殊技能？？？
    protected virtual void SpecialShot(){ }


    #region 枪械信息（保存）
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
    }
    #endregion
}
