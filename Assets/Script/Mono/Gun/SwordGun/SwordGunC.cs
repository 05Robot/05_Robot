using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Mono;
using Chronos;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/15
****	描述 AK47突击步枪控制类
**********************************************************************/
public class SwordGunC : GunC
{
    public Timeline Time
    {
        get { return GetComponent<Timeline>(); }
    }
    [Header("--特殊攻击信息--")]
    #region 特殊攻击内容
    //特殊消耗的MP
    [Rename("消耗MP/发")][SerializeField] public float m_SpecialComsumeMP;
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
    [Rename("剑影")][SerializeField]private MeleeWeaponTrail SwordTrail;
    [Header("特殊攻击能够穿过的对象")]
    [SerializeField] private LayerMask AttackLayer;
    private bool SwordGunNormalAttacking = false;//是否攻击中
    private HashSet<int> HitPointIDHashSet;//普通攻击中的敌人与敌人护盾的ID
    //private HashSet<int> SpecialHitPoint
    
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
        HitPointIDHashSet = new HashSet<int>();
        thisCapsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }
    protected override void Update()
    {
        base.Update();

        if (SwordGunNormalAttacking && (Gun_Data.Enable || Gun_Data.SpecialEnable))
        {
            SwordTrail.Emit = true;
            thisCapsuleCollider2D.enabled = false;
        }
        else
        {
            SwordTrail.Emit = false;
            thisCapsuleCollider2D.enabled = true;
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    #region 普通攻击
    /// <summary>
    /// 左键普通攻击
    /// </summary>
    protected override void LeftNormalShot()
    {
        //是否为普通攻击 && 已经开启可以用 && 达到CD时间
        if (!(Gun_Data.GunState == GunState.NormalState && Gun_Data.Enable && SwordGunCanShotNext))
            return;
        //武器射击CD计时
        StartCoroutine(ShotCD());
        //刷新击中敌人集合
        HitPointIDHashSet.Clear();
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
    #region 左键普通开枪射击CD判断
    private float m_SwordGunNormalCurrent = 0;//当前射击的CD
    private bool SwordGunCanShotNext = true;//达到CD时间，可以射击下一回合
    IEnumerator ShotCD()
    {
        SwordGunCanShotNext = false;
        while (m_SwordGunNormalCurrent < Gun_Data.AttackCD)
        {
            m_SwordGunNormalCurrent += Time.deltaTime;
            //更新CD条
            if (Gun_Data.GunState == GunState.NormalState && Gun_Data.Enable)
            {
                Target.Instance.ChangeCDSlider(Gun_Data.AttackCD, m_SwordGunNormalCurrent);
            }
            yield return null;
        }
        m_SwordGunNormalCurrent = 0;
        //更新CD条
        if (Gun_Data.GunState == GunState.NormalState && Gun_Data.Enable)
        {
            Target.Instance.ChangeCDSlider(Gun_Data.AttackCD, Gun_Data.AttackCD);
        }
        SwordGunCanShotNext = true;
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
        SwordGunNormalAttacking = true;//近战攻击中
        WeaponManager.Instance.ChangeLeftGunState = false;//无法切枪
        while (true)
        {
            closeAttackDegreeing += RotateDegree;
            transform.RotateAround(m_GunHandle.transform.position, Vector3.forward, RotateDegree);
            if (MaxRotateDegree > 0){
                if (closeAttackDegreeing >= MaxRotateDegree)
                    break;
            } else { 
                if (closeAttackDegreeing <= MaxRotateDegree)
                    break;
            }
            yield return null;
        }
        SwordGunNormalAttacking = false;
        WeaponManager.Instance.ChangeLeftGunState = true;//可以切枪
        //开启武器旋转控制
        GunRotateControl = true;
    }
    #endregion


    #region 特殊攻击
    /// <summary>
    /// 剑 - 特殊攻击
    /// 1、冷却状态（消耗HP）无法使用
    /// 2、剑直接飞出去又自动飞回来
    /// 右键点击
    /// </summary>
    protected override void RightNormalShot()
    {
        //【是否为特殊攻击  && 达到CD时间 && 武器管理类判断特殊攻击是否可用(里面会涉及修改Enable是否开启使用)】
        if (!(Gun_Data.GunState == GunState.SpecialState && CanSpecialShotNext && WeaponManager.Instance.SpecialGunCheckOut())) return;
        //如果可以射击【已经开启可以用】
        if (!Gun_Data.SpecialEnable)
            return;
        //-------------------------------------------------------
        //开始计时
        StartCoroutine(SpecialShotCD());
        //-------------------------------------------------------
        //击打动作
        AimQuadrant currentAimQuadrant = CheckAimQuadrant();
        switch (currentAimQuadrant)
        {
            case AimQuadrant.second:
            case AimQuadrant.fourth:
                StartToSpecialAttack(AttackDirection.AntiClockWise);
                break;
            case AimQuadrant.first:
            case AimQuadrant.third:
                StartToSpecialAttack(AttackDirection.ClockWise);
                break;
        }
        //角色MPHP减少
        PlayerMPHPChange((int)Gun_Data.SpecialComsumeMP, 0);
    }

    /// <summary>
    /// 扔剑前提动作
    /// </summary>
    /// <param name="currentAttackDirection"></param>
    private float SpecialRotateDirection;
    private void StartToSpecialAttack(AttackDirection currentAttackDirection)
    {
        //进行自定义攻击旋转-----------
        int centerDegree = Gun_Data.Scatter / 2 + 20;
        switch (currentAttackDirection)
        {
            case AttackDirection.ClockWise:
                transform.RotateAround(m_GunHandle.transform.position, Vector3.forward, -centerDegree);
                StartCoroutine(StartToFlyRotate(1));
                SpecialRotateDirection = 1;
                break;
            case AttackDirection.AntiClockWise:
                transform.RotateAround(m_GunHandle.transform.position, Vector3.forward, centerDegree);
                StartCoroutine(StartToFlyRotate(-1));
                SpecialRotateDirection = -1;
                break;
        }
    }


    /// <summary>
    /// 武器飞出去
    /// </summary>
    /// <returns></returns>
    //是否开启非敌人碰撞检测
    private bool StartToOtherColliderCheck = false;
    //是否在特殊攻击中碰撞到其他非敌人物体
    private bool m_collisionOthers = false;
    IEnumerator StartToFlyRotate(float direction)
    {
        StartToOtherColliderCheck = true;
        //关闭武器旋转控制
        GunRotateControl = false;
        SwordGunNormalAttacking = true;//近战攻击中
        WeaponManager.Instance.ChangeLeftGunState = false;//无法切枪
        float FlyRotateDistance = 0.0f;
        Vector3 target = (m_aimPos - transform.position) * 20;
        while (true)
        {
            if (m_collisionOthers) break;
            yield return null;
            Vector3 targetPos = Vector3.MoveTowards(transform.position, target, Gun_Data.SpecialButtleSpeed * Time.deltaTime * 0.5f);
            //飞行距离判断
            FlyRotateDistance += Vector3.Distance(transform.position, targetPos);
            if (FlyRotateDistance >= Gun_Data.SpecialAttackDistance)
                break;
            //自身旋转
            transform.RotateAround(transform.position, Vector3.forward, direction * Gun_Data.SpecialButtleSpeed);
            //自身往前飞
            transform.position = targetPos;
        }
        //飞行结束
        if(!m_collisionOthers)StartCoroutine(StartToFlyBack(direction));
        m_collisionOthers = false;
    }


    /// <summary>
    /// 武器飞回来
    /// </summary>
    /// <returns></returns>
    IEnumerator StartToFlyBack(float direction)
    {
        //关闭碰撞非敌人检测
        StartToOtherColliderCheck = false;
        while (true)
        {
            Vector3 target = (m_player.transform.position - transform.position).normalized;
            //自身往回飞
            transform.position = transform.position + target * Gun_Data.SpecialButtleSpeed * Time.deltaTime;
            //自身旋转
            transform.RotateAround(transform.position, Vector3.forward, direction * Gun_Data.SpecialButtleSpeed);
            if (Vector3.Distance(transform.position, m_player.transform.position) <= 1f)
                break;
            yield return null;
        }
        //飞回玩家身边
        SwordGunNormalAttacking = false;//近战结束
        WeaponManager.Instance.ChangeLeftGunState = true;//可以切枪
        //开启武器旋转控制
        GunRotateControl = true;
        //剑回到当前手上（结合玩家方向），WeaponManager
        transform.parent.position = WeaponManager.Instance.CurrentWeaponPos;
        transform.parent.localPosition = new Vector3(-5.5f,0,-0.1f);
        //-------------------------------------------------------
        //隐藏剑
        WeaponManager.Instance.SpecialGunToNormalGun();
    }

    #region 右键特殊攻击CD判断
    private float m_SpecialCurrent = 0;//当前射击的CD
    private bool CanSpecialShotNext = true;//达到CD时间，可以射击下一回合
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
    #endregion




    /// <summary>
    /// 攻击触发机制
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerStay2D(Collider2D other)
    {
        //是否攻击中
        if (!SwordGunNormalAttacking) return;

        //对敌人造成伤害
        switch (Gun_Data.GunState)
        {
            //剑的普通攻击
            case GunState.NormalState:
                //击中敌人护盾 && 不重复击中
                if (other.gameObject.layer == 18 && !HitPointIDHashSet.Contains(other.GetInstanceID()))
                {
                    other.transform.GetComponent<ShieldProtect>().GetEnemyControl().GetDamage(Convert.ToInt32(Gun_Data.DemageNums), Convert.ToInt32(Gun_Data.DemageNums));
                    HitPointIDHashSet.Add(other.GetInstanceID());
                    HitPointIDHashSet.Add(other.transform.GetComponent<ShieldProtect>().ProtectAimGameObject.GetInstanceID());
                    //设置硬直击退
                    other.transform.GetComponent<ShieldProtect>().GetEnemyControl().SetDelay(0.5f, 2);
                    other.transform.GetComponent<ShieldProtect>().GetEnemyControl().SetKnockback(transform.position, 0.5f, 2);
                }

                //否则，击中的是敌人内部
                if (other.gameObject.layer == 11 && !HitPointIDHashSet.Contains(other.GetInstanceID()))
                {
                    other.transform.GetComponent<EnemyContral>().GetDamage(Convert.ToInt32(Gun_Data.DemageNums), Convert.ToInt32(Gun_Data.DemageNums));
                    HitPointIDHashSet.Add(other.GetInstanceID());
                    //设置硬直击退
                    other.transform.GetComponent<EnemyContral>().SetDelay(0.5f, 2);
                    other.transform.GetComponent<EnemyContral>().SetKnockback(transform.position, 0.5f, 2);
                }
                //击中紫水晶与零件箱
                if (other.gameObject.layer == 19 || other.gameObject.layer == 20)
                {
                    other.transform.GetComponent<HitCheckBase>().Broken();
                }
                break;
            //剑的特殊攻击
            case GunState.SpecialState:
                //不在攻击角色范围内 && 是第一次击中其他物体
                //结束飞行，进行飞回
                if ((AttackLayer >> other.gameObject.layer & 1) != 1 && StartToOtherColliderCheck)
                {
                    m_collisionOthers = true;
                    //结束飞行
                    StartCoroutine(StartToFlyBack(SpecialRotateDirection));
                    //关闭碰撞非敌人检测
                    StartToOtherColliderCheck = false;
                }
                //在攻击角色范围内
                else
                {
                    //击中敌人护盾 && 可重复击中
                    if (other.gameObject.layer == 18)
                    {
                        other.transform.GetComponent<ShieldProtect>().GetEnemyControl().GetDamage(Convert.ToInt32(Gun_Data.SpecialDemageNums), Convert.ToInt32(Gun_Data.SpecialDemageNums));
                        //设置硬直击退
                        other.transform.GetComponent<ShieldProtect>().GetEnemyControl().SetDelay(0.5f, 3);
                        other.transform.GetComponent<ShieldProtect>().GetEnemyControl().SetKnockback(transform.position, 0.5f, 3);
                    }
                    //击中敌人内部 && 是否没有护盾
                    if (other.gameObject.layer == 11 && !other.transform.GetComponent<EnemyContral>().ER.IsConsumeMp)
                    {
                        other.transform.GetComponent<EnemyContral>().GetDamage(Convert.ToInt32(Gun_Data.SpecialDemageNums), Convert.ToInt32(Gun_Data.SpecialDemageNums));
                        //设置硬直击退
                        other.transform.GetComponent<EnemyContral>().SetDelay(0.5f, 3);
                        other.transform.GetComponent<EnemyContral>().SetKnockback(transform.position, 0.5f, 3);
                    }
                    //击中紫水晶与零件箱
                    if (other.gameObject.layer == 19 || other.gameObject.layer == 20)
                        other.transform.GetComponent<HitCheckBase>().Broken();
                }
                break;
        }
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
