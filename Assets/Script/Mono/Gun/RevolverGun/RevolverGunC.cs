using System.Collections;
using System.Collections.Generic;
using Chronos;
using UnityEngine;

public class RevolverGunC : GunC {
    public Timeline Time
    {
        get { return GetComponent<Timeline>(); }
    }
    [Header("--特殊攻击信息--")]
    #region 特殊攻击内容
    //特殊消耗的MP
    [Rename("消耗MP/发")] [SerializeField] public float m_SpecialComsumeMP;
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

    /// <summary>
    /// 左键普通攻击
    /// </summary>
    protected override void LeftNormalShot()
    {
        base.LeftNormalShot();
    }


    /// <summary>
    /// 左轮手枪 - 特殊攻击
    /// 1、冷却状态（消耗HP）无法使用
    /// 2、2s的子弹时间【1、会有一个20%的伤害提升；2、这段时间内除玩家以外的东西（敌人）速度都变成原来的0.5倍】
    /// 右键点击
    /// </summary>
    protected override void RightNormalShot()
    {
        base.RightNormalShot();
        //【是否为特殊攻击  && 达到CD时间 && 武器管理类判断特殊攻击是否可用(里面会涉及修改Enable是否开启使用)】
        if (!(Gun_Data.GunState == GunState.SpecialState && CanSpecialShotNext && WeaponManager.Instance.SpecialGunCheckOut())) return;
        //如果可以射击【已经开启可以用】
        if (!Gun_Data.SpecialEnable)
            return;
        //-------------------------------------------------------
        //开始计时
        StartCoroutine(SpecialShotCD());
        //-------------------------------------------------------
        //时间控制【除玩家以外的东西（敌人）速度都变成原来的0.5倍】
        TimeManager.Instance.DelayTime(2.0f);

        //20%的伤害提升
        WeaponManager.Instance.ChangeButtleDemagePercent(0.2f, 2);
        //开启特效
        StartCoroutine(StartBuffEffect());
        //角色MPHP减少
        PlayerMPHPChange((int)Gun_Data.SpecialComsumeMP, 0);
    }

    #region 开启特效
    IEnumerator StartBuffEffect()
    {
        GameObject effectPlayer = Instantiate(Gun_Data.SpecialButtle, m_player.transform.position + new Vector3(0, -1, 0), transform.rotation);
        effectPlayer.transform.rotation = Quaternion.Euler(-90, 0, 0);
        yield return Time.WaitForSeconds(2f);
        Destroy(effectPlayer);
    }
    #endregion


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
                print("手枪");
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
