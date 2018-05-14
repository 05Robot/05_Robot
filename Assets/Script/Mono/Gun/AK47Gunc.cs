using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/15
****	描述 AK47突击步枪控制类
**********************************************************************/
public class AK47GunC : GunC  {
    [Header("--特殊攻击信息--")]
    #region 特殊攻击内容
    //特殊消耗的MP
    [Rename("消耗MP/发")][SerializeField] public float m_SpecialComsumeMP;
    //特殊消耗的HP
    [Rename("消耗HP/发")][SerializeField] private float m_SpecialComsumeHP;
    //特殊伤害数值
    [Rename("伤害/发")][SerializeField] private float m_SpecialDemageNums;
    //特殊硬直系数
    [Rename("硬直系数")][SerializeField] private float m_SpecialHardStraight;
    //特殊击退系数
    [Rename("击退系数")][SerializeField] private float m_SpecialBeatBack;
    //特殊攻击频率CD
    [Rename("射击频率(s)（攻击速度）")][SerializeField] private float m_SpecialAttackCD;
    //特殊最大蓄能时间
    [Rename("最大蓄能时间")][SerializeField] private float m_SpecialMaxEnergyTime;
    //特殊攻击中：和子弹 或者其他 预设有关的
    //子弹散射度数
    [Rename("散射度数")][SerializeField] private int m_SpecialScatter;
    //子弹速度
    [Rename("子弹速度（单位/s）")][SerializeField] private uint m_SpecialButtleSpeed;
    //特殊攻击距离
    [Rename("子弹距离")][SerializeField] private float m_SpecialAttackDistance;
    //子弹预设
    [Rename("子弹预设")][SerializeField] private GameObject m_SpecialButtle;
    //特殊攻击是否可用/开启
    [Rename("特殊攻击是否可用")][SerializeField] private bool m_SpecialEnable;
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
    /// 左普通攻击
    /// </summary>
    protected override void LeftNormalShot()
    {
        base.LeftNormalShot();
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
        if (CanSpecialShotNext)//如果可以射击
        {
            Target.GetComponent<Target>().ChangeTargetSlider(Gun_Data.SpecialMaxEnergyTime, base.RightEnergyTime);
        }
        else//如果不能射击
        {
            base.RightEnergyTime = 0;
        }
    }

    /// <summary>
    /// 突击步枪 - 特殊攻击
    /// 1、冷却状态（消耗HP）无法使用
    /// 2、todo 【榴弹Bullet】枪榴弹爆炸后会造成周围3个单位内的敌人400点伤害，并且造成2s硬直和0.5个单位的击退
    /// 3、MP蓄能消耗：100+25t
    /// 4、子弹蓄能距离：2+4t
    /// 右键蓄能攻击
    /// </summary>
    ///
    protected override void RightEnergyShot()
    {
        base.RightEnergyShot();
        //如果可以射击【是否为普通攻击 && 已经开启可以用 && 达到CD时间】
        if (Gun_Data.GunState == GunState.SpecialState  && Gun_Data.SpecialEnable && CanSpecialShotNext) 
            Target.GetComponent<Target>().Init();//瞄准Target数值归0
        else
            return;
        //-------------------------------------------------------
        //开始计时
        StartCoroutine(SpecialShotCD());
        //-------------------------------------------------------
        //生成子弹（调整位置与角度）
        WeaponManager.Instance.GenerateNormalButton(Gun_Data.SpecialButtle.name,
            Gun_Data.MuzzlePos.transform.position, Gun_Data.MuzzlePos.transform.rotation.eulerAngles, Gun_Data.SpecialScatter,
            Gun_Data.SpecialButtleSpeed, Gun_Data.SpecialAttackDistance + base.RightEnergyTime * 4.0f, Gun_Data.SpecialDemageNums);

        //角色MPHP减少
        PlayerMPHPChange((int) Gun_Data.SpecialComsumeMP + base.RightEnergyTime * 25.0f, 0); 
    }
    #region 右键特殊攻击CD判断
    private float m_SpecialCurrent = 0;//当前射击的CD
    private bool CanSpecialShotNext = true;//达到CD时间，可以射击下一回合
    IEnumerator SpecialShotCD()
    {
        CanSpecialShotNext = false;
        while (m_SpecialCurrent < Gun_Data.SpecialAttackCD)
        {
            m_SpecialCurrent += Time.fixedDeltaTime;
            yield return null;
        }
        m_SpecialCurrent = 0;
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
