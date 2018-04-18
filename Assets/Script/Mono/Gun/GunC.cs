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
    //枪械索引d 
    [SerializeField] private uint m_GunIndex;
    //枪械名字
    [SerializeField] private string m_GunName;
    //普通攻击-----------------------------------------------
    //普通消耗的MP
    [SerializeField] private int m_ComsumeMP;
    //普通消耗的HP
    [SerializeField] private int m_ComsumeHP;
    //普通伤害数值
    [SerializeField] private uint m_DemageNums;
    //硬直系数
    [SerializeField] private float m_HardStraight;
    //击退系数
    [SerializeField] private float m_BeatBack;
    //攻击距离
    [SerializeField] private uint m_AttackDistance;
    //攻击频率CD
    [SerializeField] private float m_AttackCD;
    //子弹类型
    [SerializeField] private GameObject m_buttle;
    //子弹速度
    [SerializeField] private uint m_ButtleSpeed;
    //最大蓄能时间
    [SerializeField] private int m_MaxEnergyTime;
    //枪拥有的射击类型
    [SerializeField] private ShotType[] ShotType;
    //枪口位置
    [SerializeField] private GameObject m_MuzzlePos;
    //--------------------------------------------------
    #endregion
    protected GunM Gun_Data;
    
    //鼠标位置
    private Vector3 m_mousePos;
    //瞄准的世界坐标位置
    private Vector3 m_aimPos;
    //鼠标左右键是否点击
    private bool LeftShot, RightShot, LeftContinueShot, RightContinueShot;



    protected virtual void Awake()
    {
        LeftShot = RightShot = LeftContinueShot = RightContinueShot = false;//点击
        Gun_Data = new GunM();//枪数据

        //保存枪械信息
        SaveGunData();
    }

    protected virtual void Update()
    {
        //枪方向
        m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_aimPos = new Vector3(m_mousePos.x, m_mousePos.y, transform.position.z);
        test.transform.position = m_aimPos;
        float z;
        if (m_aimPos.y > transform.position.y)//旋转角度
            z = -Vector3.Angle(Vector3.left, m_aimPos - transform.position);
        else
            z = Vector3.Angle(Vector3.left, m_aimPos - transform.position);
       // print(m_aimPos.x + "  " + transform.position.x);
        print(transform.localScale);
        if (m_aimPos.x > transform.position.x)
            transform.localScale = new Vector3(1, -1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
        transform.localRotation = Quaternion.Euler(0, 0, z);
        //枪普通与特殊攻击
        if (Input.GetMouseButtonDown(0)) //左点射
        {
            LeftShot = true;
        }
        if (Input.GetMouseButtonDown(1)) //右点射
        {
            RightShot = true;
        }
        if (Input.GetMouseButton(0)) //左连续射
        {
            LeftContinueShot = true;
        }
        if (Input.GetMouseButton(1)) //右连续射
        {
            RightContinueShot = true;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (LeftShot)
        {
            LeftShot = false;
            NormalShot();
        }if (RightShot)
        {
            RightShot = false;
            NormalShot();
        }if (LeftContinueShot)
        {
            LeftContinueShot = false;
            SpecialShot();
        }if (RightContinueShot)
        {
            RightContinueShot = false;
            SpecialShot();
        }
    }


    //普通开枪射击
    protected virtual void NormalShot()
    {
        //生成子弹（调整位置与角度）
        GameObject buttleGameObject = ObjectPool.Instance.Spawn(Gun_Data.Buttle.name);
        buttleGameObject.transform.position = Gun_Data.MuzzlePos.transform.position;
        buttleGameObject.transform.rotation = Gun_Data.MuzzlePos.transform.rotation;
        //子弹数据填充
        Buttle buttle = buttleGameObject.GetComponent<Buttle>();
        buttle.BulletStart(Gun_Data.ButtleSpeed, Gun_Data.AttackDistance, Gun_Data.DemageNums);
    }
    //特殊技能
    public abstract void SpecialShot();


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
    }
    #endregion
}
