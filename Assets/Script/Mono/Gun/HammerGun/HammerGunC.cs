﻿using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Mono;
using Chronos;
using UnityEngine;

public class HammerGunC : GunC
{
    public Timeline Time
    {
        get { return GetComponent<Timeline>(); }
    }
    [Header("--特殊攻击信息--")]
    #region 特殊攻击内容
    //特殊消耗的MP
    [Rename("消耗MP/发")]
    [SerializeField]
    public float m_SpecialComsumeMP;
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
    [Header("--特殊信息--")]
    [Rename("锤影")]
    [SerializeField]private MeleeWeaponTrail HammerTrail;
    private bool HammerGunNormalAttacking = false;//是否攻击中
    private HashSet<int> HitPointIDHashSet;//普通攻击中的敌人与敌人护盾的ID

    //蓄能攻击特效
    private GameObject RedEnergyEffect;

    //取锤子冲撞是否结束
    //锤子冲撞特效
    private GameObject RedDashEffect;

    private CapsuleCollider2D thisCapsuleCollider2D;

    private enum AimQuadrant
    {
        first = 0,
        second = 1,
        third = 2,
        fourth = 3
    }
    private enum AttackDirection
    {
        ClockWise = 0,
        AntiClockWise = 1
    }
    protected override void Awake()
    {
        base.Awake();
        SaveGunSpecialData();

        RedEnergyEffect = m_player.transform.Find("RedEnergyEffect").gameObject;

        RedDashEffect = m_player.transform.Find("RedDashEffect").gameObject;

        HitPointIDHashSet = new HashSet<int>();

        thisCapsuleCollider2D = GetComponent<CapsuleCollider2D>();

    }
    protected override void Update()
    {
        base.Update();

        if (HammerGunNormalAttacking && (Gun_Data.Enable || Gun_Data.SpecialEnable))
        {
            HammerTrail.Emit = true;
            thisCapsuleCollider2D.enabled = false;
        }
        else
        {
            HammerTrail.Emit = false;
            thisCapsuleCollider2D.enabled = true;
        }
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
        //刷新击中敌人集合
        HitPointIDHashSet.Clear();
        //是否为普通攻击 && 已经开启可以用 && 达到CD时间
        if (!(Gun_Data.GunState == GunState.NormalState && Gun_Data.Enable && HammerGunCanShotNext))
            return;
        //武器射击CD计时
        StartCoroutine(ShotCD());
        //击打动作
        AimQuadrant currentAimQuadrant = CheckAimQuadrant();
        switch (currentAimQuadrant)
        {
            case AimQuadrant.second:
            case AimQuadrant.fourth:
                NormalCloseAttack(AttackDirection.AntiClockWise);
                break;
            case AimQuadrant.first:
            case AimQuadrant.third:
                NormalCloseAttack(AttackDirection.ClockWise);
                break;
        }

        //角色MPHP减少
        PlayerMPHPChange(Gun_Data.ComsumeMP, Gun_Data.ComsumeHP);
    }
    #region 左键普通攻击CD判断
    private float m_HammerGunNormalCurrent = 0;//当前射击的CD
    private bool HammerGunCanShotNext = true;//达到CD时间，可以射击下一回合
    IEnumerator ShotCD()
    {
        HammerGunCanShotNext = false;
        while (m_HammerGunNormalCurrent < Gun_Data.AttackCD)
        {
            m_HammerGunNormalCurrent += Time.deltaTime;
            //更新CD条
            if (Gun_Data.GunState == GunState.NormalState && Gun_Data.Enable)
            {
                Target.Instance.ChangeCDSlider(Gun_Data.AttackCD, m_HammerGunNormalCurrent);
            }
            yield return null;
        }
        m_HammerGunNormalCurrent = 0;
        //更新CD条
        if (Gun_Data.GunState == GunState.NormalState && Gun_Data.Enable)
        {
            Target.Instance.ChangeCDSlider(Gun_Data.AttackCD, Gun_Data.AttackCD);
        }
        HammerGunCanShotNext = true;
    }
    #endregion

    /// <summary>
    /// 判断当前象限
    /// </summary>
    private AimQuadrant CheckAimQuadrant()
    {
        if (m_aimPos.x > transform.position.x)
        {
            if (m_aimPos.y > transform.position.y)
                return AimQuadrant.first;
            else
                return AimQuadrant.fourth;
        }
        else
        {
            if (m_aimPos.y > transform.position.y)
                return AimQuadrant.second;
            else
                return AimQuadrant.third;
        }
    }

    /// <summary>
    /// 近战攻击 
    /// </summary>
    /// <param name="currentAttackDirection"></param>
    private void NormalCloseAttack(AttackDirection currentAttackDirection)
    {
        //进行自定义攻击旋转-----------
        int centerDegree = Gun_Data.Scatter / 2 + 20;
        switch (currentAttackDirection)
        {
            case AttackDirection.ClockWise:
                transform.RotateAround(m_GunHandle.transform.position, Vector3.forward, -centerDegree);
                StartCoroutine(StartToCloseAttack(Gun_Data.ButtleSpeed, Gun_Data.Scatter));
                break;
            case AttackDirection.AntiClockWise:
                transform.RotateAround(m_GunHandle.transform.position, Vector3.forward, centerDegree);
                StartCoroutine(StartToCloseAttack(-Gun_Data.ButtleSpeed, -Gun_Data.Scatter));
                break;
        }
    }

    /// <summary>
    /// 武器旋转攻击
    /// </summary>
    /// <param name="RotateDegree">旋转方向与速度</param>
    /// <param name="MaxRotateDegree">最大旋转角度</param>
    IEnumerator StartToCloseAttack(float RotateDegree, float MaxRotateDegree)
    {
        //关闭武器旋转控制
        GunRotateControl = false;
        float closeAttackDegreeing = 0.0f;//近战攻击角度累加
        HammerGunNormalAttacking = true;//近战攻击中
        WeaponManager.Instance.ChangeLeftGunState = false;//无法切枪
        while (true)
        {
            closeAttackDegreeing += RotateDegree;
            transform.RotateAround(m_GunHandle.transform.position, Vector3.forward, RotateDegree);
            if (MaxRotateDegree > 0)
            {
                if (closeAttackDegreeing >= MaxRotateDegree)
                    break;
            }
            else
            {
                if (closeAttackDegreeing <= MaxRotateDegree)
                    break;
            }
            yield return null;
        }
        HammerGunNormalAttacking = false;
        WeaponManager.Instance.ChangeLeftGunState = true;//可以切枪
        //开启武器旋转控制
        GunRotateControl = true;
    }


    /// <summary>
    /// 攻击触发机制
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay2D(Collider2D other)
    {
        //是否攻击中
        if (!HammerGunNormalAttacking) return;

        //对敌人造成伤害
        switch (Gun_Data.GunState)
        {
            //锤子普通攻击
            case GunState.NormalState:
                //击中敌人护盾 && 不重复击中
                if (other.gameObject.layer == 18 && !HitPointIDHashSet.Contains(other.GetInstanceID()))
                {
                    other.transform.GetComponent<ShieldProtect>().GetEnemyControl().GetDamage(Convert.ToInt32(Gun_Data.DemageNums), Convert.ToInt32(Gun_Data.DemageNums));
                    HitPointIDHashSet.Add(other.GetInstanceID());
                    HitPointIDHashSet.Add(other.transform.GetComponent<ShieldProtect>().ProtectAimGameObject.GetInstanceID());
                    //设置硬直击退
                    other.transform.GetComponent<ShieldProtect>().GetEnemyControl().SetDelay(0.5f, 3);
                    other.transform.GetComponent<ShieldProtect>().GetEnemyControl().SetKnockback(transform.position, 0.5f, 3);
                }

                //否则，击中的是敌人内部
                if (other.gameObject.layer == 11 && !HitPointIDHashSet.Contains(other.GetInstanceID()))
                {
                    other.transform.GetComponent<EnemyContral>().GetDamage(Convert.ToInt32(Gun_Data.DemageNums), Convert.ToInt32(Gun_Data.DemageNums));
                    HitPointIDHashSet.Add(other.GetInstanceID());
                    //设置硬直击退
                    other.transform.GetComponent<EnemyContral>().SetDelay(0.5f, 3);
                    other.transform.GetComponent<EnemyContral>().SetKnockback(transform.position, 0.5f, 3);
                }

                //击中紫水晶与零件箱
                if (other.gameObject.layer == 19 || other.gameObject.layer == 20)
                    other.transform.GetComponent<HitCheckBase>().Broken();
                break;
            //锤子特殊攻击
            case GunState.SpecialState:
                //击中敌人护盾 && 可重复击中
                if (other.gameObject.layer == 18)
                {
                    other.transform.GetComponent<ShieldProtect>().GetEnemyControl().GetDamage(Convert.ToInt32(Gun_Data.SpecialDemageNums), Convert.ToInt32(Gun_Data.SpecialDemageNums));
                    //设置硬直击退
                    other.transform.GetComponent<ShieldProtect>().GetEnemyControl().SetDelay(0.5f, 4);
                    other.transform.GetComponent<ShieldProtect>().GetEnemyControl().SetKnockback(transform.position, 0.5f, 4);
                }

                //击中敌人内部 && 是否没有护盾
                if (other.gameObject.layer == 11 && other.transform.GetComponent<EnemyContral>().ER.CurrentMp <= 0)
                {
                    other.transform.GetComponent<EnemyContral>().GetDamage(Convert.ToInt32(Gun_Data.SpecialDemageNums),Convert.ToInt32(Gun_Data.SpecialDemageNums));
                    //设置硬直击退
                    other.transform.GetComponent<EnemyContral>().SetDelay(0.5f, 4);
                    other.transform.GetComponent<EnemyContral>().SetKnockback(transform.position, 0.5f, 4);
                }
                //击中紫水晶与零件箱
                if (other.gameObject.layer == 19 || other.gameObject.layer == 20)
                    other.transform.GetComponent<HitCheckBase>().Broken();
                break;
        }
    }

    /// <summary>
    /// 右键蓄能过程
    /// 1、Slider修改
    /// 2、如果CD未到，不断重置蓄能累计时间
    /// </summary>
    protected override void RightEnergying()
    {
        base.RightEnergying();
        //瞄准Target数值变换
        if (CanSpecialShotNext)//如果可以冲撞
        {
            //【是否为特殊攻击 && 武器管理类判断特殊攻击是否可用(里面会涉及修改Enable是否开启使用) && 已经开启可以用】
            if (!(Gun_Data.GunState == GunState.SpecialState && WeaponManager.Instance.SpecialGunCheckOut() &&
                  Gun_Data.SpecialEnable))
            {
                //真的不能冲撞了
                CanFire = false;
                return;
            }
            //真的可以冲撞了
            CanFire = true;
            //更新蓄能时间条
            Target.Instance.ChangeTargetSlider(Gun_Data.SpecialMaxEnergyTime, base.RightEnergyTime);
            //蓄能特效生成
            RedEnergyEffect.SetActive(true);
        }
        else//如果不能冲撞
        {
            base.RightEnergyTime = 0;
        }
    }

    /// <summary>
    /// 锤子 - 特殊攻击
    /// 1、冷却状态（消耗HP）无法使用
    /// 2、松开右键后，向鼠标指向方向冲刺，冲刺距离=子弹距离，冲刺速度=子弹速度，对沿途敌人造成相同伤害和1s硬直，会穿过敌人
    /// 3、移动距离：3+4t
    /// 4、移动速度：12+2t
    /// 右键蓄能攻击
    /// </summary>
    private bool CanFire = false;//蓄能所用，判断是否真的开始冲撞了
    protected override void RightEnergyShot()
    {
        base.RightEnergyShot();
        //如果可以冲撞【达到CD时间】
        if (CanFire)
        {
            Target.Instance.Init();//瞄准Target数值归0
            CanFire = false;
        }
        else
            return;
        //-------------------------------------------------------
        //开始计时
        StartCoroutine(SpecialShotCD());
        //------------------------------------------------------
        //蓄能特效消失
        RedEnergyEffect.SetActive(false);
        //冲撞特效显示
        RedDashEffect.transform.LookAt(m_aimPos);
        RedDashEffect.SetActive(true);
        //-------------------------------------------------------
        //todo 调用人物冲撞函数（传进冲刺距离，冲刺速度，或者是伤害和硬直）
        GunRotateControl = false;
        HammerGunNormalAttacking = true;
        Invoke("RecoverSpecialDashRotate", RightEnergyTime/4.0f);
        StartCoroutine(m_playerRobotContral.Bump(2 * RightEnergyTime + Gun_Data.SpecialButtleSpeed, 4 * RightEnergyTime + Gun_Data.SpecialAttackDistance));

        //角色MPHP减少
        PlayerMPHPChange((int)Gun_Data.SpecialComsumeMP + base.RightEnergyTime * 50.0f, 0);
    }
    #region 右键特殊攻击CD判断
    private float m_SpecialCurrent = 0;//当前冲撞的CD
    private bool CanSpecialShotNext = true;//达到CD时间，可以冲撞下一回合
    IEnumerator SpecialShotCD()
    {
        CanSpecialShotNext = false;
        while (m_SpecialCurrent < Gun_Data.SpecialAttackCD)
        {
            m_SpecialCurrent += Time.deltaTime;
            //更新CD条
            if (Gun_Data.GunState == GunState.SpecialState)
            {
                Target.Instance.ChangeSpecialCDSlider(Gun_Data.SpecialAttackCD, m_SpecialCurrent);
            }
            yield return null;
        }
        m_SpecialCurrent = 0;
        //更新CD条
        if (Gun_Data.GunState == GunState.SpecialState)
        {
            Target.Instance.ChangeSpecialCDSlider(Gun_Data.SpecialAttackCD, Gun_Data.SpecialAttackCD);
        }
        CanSpecialShotNext = true;
    }
    #endregion

    /// <summary>
    /// 冲撞完毕，运行武器旋转，特殊武器切换为普通武器
    /// </summary>
    private void RecoverSpecialDashRotate()
    {
        //开启武器旋转
        GunRotateControl = true;
        //-------------------------------------------------------
        //特殊武器切回主武器
        WeaponManager.Instance.SpecialGunToNormalGun();
        //冲撞特效消失
        RedDashEffect.SetActive(false);

        HammerGunNormalAttacking = false;
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
