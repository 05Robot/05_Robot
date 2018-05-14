using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public enum GunType//枪种类
{
    AK47Gun = 1,//突击步枪
    RevolverGun = 2,//左轮
    ShotGun = 3,//霰弹枪
    RocketGun = 4,//火箭筒
    AWMGun = 5,//狙击枪
    Sword = 6,//剑
    Hammer = 7//锤子
}

public class WeaponManager : Singleton<WeaponManager>
{
    #region 血条消耗的状态（消耗HP/MP）
    private enum BloodConsumeState
    {
        Hp = 0,
        Mp = 0
    }
    #endregion
    #region 所有当前武器
    [Header("--所有所有所有的枪--")]//所有所有所有的枪（预设）
    [SerializeField] private GameObject m_AK47Gun;
    [SerializeField] private GameObject m_RevolverGun;
    [SerializeField] private GameObject m_ShotGun;
    [SerializeField] private GameObject m_RocketGun;
    [SerializeField] private GameObject m_AWMGun;
    [SerializeField] private GameObject m_Sword;
    [SerializeField] private GameObject m_Hammer;
    private AK47GunC m_Ak47GunC;
    private RevolverGunC m_RevolverGunC;
    private ShotGunC m_ShotGunC;
    private RocketGunC m_RocketGunC;
    private AWMGunC m_AWMGunC;
    private SwordGunC m_SwordGunC;
    private HammerGunC m_HammerGunC;
    private Dictionary<GunType, GunC> AllGunCDic;//枪普通攻击基类配对（类型，普攻击基类控制脚本）
    private Dictionary<GunType, GameObject> AllGunDic;//枪配对（类型，实体）
    [Header("--当前拥有的枪--")]
    private GameObject[] LeftGun;//主武器(3把)
    private GameObject RightGun;//特殊武器
    [Header("--当前拥有的枪的类型--")]
    public GunType[] LeftGunType;//主武器种类(最多3把)
    public GunType RightGunType;//特殊武器种类
    //--------
    [Header("--当前手上的武器-")]
    private GunType m_CurrentLeftGunType;//当前手上的主武器
    private GunType m_CurrentRightGunType;//当前手上的特殊武器
    private int m_CurrentLeftGunIndex = 0;//当前手上主武器的武器槽索引
    #endregion


    #region 当前子弹统一属性
    //当前射击出子弹的数目
    private static int m_CurrentButtleNum = 1;
    /*public static int CurrentButtleNum
    {
        get
        {
            return m_CurrentButtleNum;
        }

        set
        {
            m_CurrentButtleNum = value;
        }
    }*/

    //当前射击出子弹的伤害百分比
    private static float m_CurrentButtleDemagePercent = 1;
    /*public static float CurrentButtleDemagePercent
    {
        get
        {
            return m_CurrentButtleDemagePercent;
        }

        set
        {
            m_CurrentButtleDemagePercent = value;
        }
    }
    */
    #endregion

    #region 与主角角色属性有关
    private BloodConsumeState m_CurrentBloodConsumeState = BloodConsumeState.Mp;
    #endregion

    protected override void Awake()
    {
        base.Awake();
    }

    void Start () {
        #region 初始化（所有所有枪配对、设为不可见、所有脚本获取与配对）
        AllGunDic = new Dictionary<GunType, GameObject>
        {
            {GunType.AK47Gun, m_AK47Gun},
            {GunType.RevolverGun, m_RevolverGun},
            {GunType.ShotGun, m_ShotGun},
            {GunType.RocketGun, m_RocketGun},
            {GunType.AWMGun, m_AWMGun},
            {GunType.Sword, m_Sword},
            {GunType.Hammer, m_Hammer}
        };
        foreach (var keyValuePair in AllGunDic)//所有抢设为不可见
            keyValuePair.Value.SetActive(false);
        //脚本获取------ todo 可能出错，待定
        m_Ak47GunC = m_AK47Gun.GetComponent<AK47GunC>();
        m_RevolverGunC = m_RevolverGun.GetComponent<RevolverGunC>();
        m_ShotGunC = m_ShotGun.GetComponent<ShotGunC>();
        m_RocketGunC = m_RocketGun.GetComponent<RocketGunC>();
        m_AWMGunC = m_AWMGun.GetComponent<AWMGunC>();
        m_SwordGunC = m_Sword.GetComponent<SwordGunC>();
        m_HammerGunC = m_Hammer.GetComponent<HammerGunC>();
        AllGunCDic = new Dictionary<GunType, GunC>
        {
            {GunType.AK47Gun, m_Ak47GunC},
            {GunType.RevolverGun, m_RevolverGunC},
            {GunType.ShotGun, m_ShotGunC},
            {GunType.RocketGun, m_RocketGunC},
            {GunType.AWMGun, m_AWMGunC},
            {GunType.Sword, m_SwordGunC},
            {GunType.Hammer, m_HammerGunC}
        };
        #endregion

        #region 初始化（当前已拥有枪【LeftGun与LeftGunType配对,RightGun与RightGunType配对】、修改枪的状态【普通or特殊】、主武器默认显示一把(Enable设置)、当前手里的武器(普通与特殊)）
        //当前已有武器配对
        LeftGun = new GameObject[LeftGunType.Length];
        for (int i = 0; i < LeftGunType.Length; i++)
        {
            LeftGun[i] = AllGunDic[LeftGunType[i]];
            AllGunCDic[LeftGunType[i]].GunState = GunState.NormalState;//修改枪的状态【普通】
        }
        RightGun = AllGunDic[RightGunType];
        AllGunCDic[RightGunType].GunState = GunState.SpecialState;//修改枪的状态【特殊】
        //当前主武器默认显示一把，并且设置Enable
        LeftGun[m_CurrentLeftGunIndex].SetActive(true);
        AllGunCDic[LeftGunType[m_CurrentLeftGunIndex]].IfGunCanUse(true);
        //当前手的武器(普通与特殊)类型复制
        m_CurrentLeftGunType = LeftGunType[m_CurrentLeftGunIndex];
        m_CurrentRightGunType = RightGunType;
        #endregion

    }


    void Update ()
    {
        //鼠标点击事件监听
        MouseKeyListener();
        //鼠标滚轮监听事件
        MouseWheelListener();
        //键盘监听事件
        KeyBoardListener();
        //血条扣血状态监听事件【消耗HP是不可使用特殊攻击】
        MpAndHpListener();
    }

    void FixedUpdate()
    {
  
    }


    #region 鼠标与键盘监听事件
    /// <summary>
    /// 鼠标滚轮监听事件
    /// </summary>
    private void MouseWheelListener()
    {
        //向下滚动
        if (Input.GetAxis("Mouse ScrollWheel") < -0.1)
        {
            ChangeLeftGun(m_CurrentLeftGunIndex + 1);
        }
        //向上滚动
        if (Input.GetAxis("Mouse ScrollWheel") > 0.1)
        {
            ChangeLeftGun(m_CurrentLeftGunIndex - 1);
        }
    }
    /// <summary>
    /// 键盘监听事件
    /// </summary>
    private void KeyBoardListener()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeLeftGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeLeftGun(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeLeftGun(3);
    }
    /// <summary>
    /// 鼠标点击事件监听
    /// </summary>
    private void MouseKeyListener()
    {
        //todo 如何隐藏物体
        if (Input.GetMouseButtonDown(1))//右键按下
        {
            
        }
        if (Input.GetMouseButtonUp(1))//右键抬起
        {

        }
    }
    #endregion


    #region 切枪函数
    /// <summary>
    /// 切换主武器的当前手里的枪
    /// </summary>
    /// <param name="TargetGunIndex">切换的目标的枪索引</param>
    private void ChangeLeftGun(int TargetGunIndex)
    {
        if (TargetGunIndex < 0)
            TargetGunIndex += LeftGunType.Length;
        TargetGunIndex = TargetGunIndex % LeftGunType.Length;
        //修改武器属性--
        ChangeLeftGunData(m_CurrentLeftGunIndex, TargetGunIndex);
        //--------------
        //当前枪索引更新
        m_CurrentLeftGunIndex = TargetGunIndex;
        //当前抢类型更新
        m_CurrentLeftGunType = LeftGunType[m_CurrentLeftGunIndex];
    }

    /// <summary>
    /// 切枪
    /// 修改武器属性
    /// </summary>
    /// <param name="lastLeftGunIndex">上一把武器索引</param>
    /// <param name="currentLeftGunIndex">当前武器索引</param>
    private void ChangeLeftGunData(int lastLeftGunIndex, int currentLeftGunIndex)
    {
        //修改武器属性（Enable：是否开启可用）
        AllGunCDic[LeftGunType[lastLeftGunIndex]].IfGunCanUse(false);
        AllGunCDic[LeftGunType[currentLeftGunIndex]].IfGunCanUse(true);

        //修改当前武器的旋转度
        LeftGun[currentLeftGunIndex].transform.localRotation = LeftGun[lastLeftGunIndex].transform.localRotation;

        //修改武器的可见性（SetEnable）
        LeftGun[lastLeftGunIndex].SetActive(false);
        LeftGun[currentLeftGunIndex].SetActive(true);
    }

    #endregion



    /// <summary>
    /// 【用于维修站修改枪】
    /// 修改主角当前拥有的枪
    /// </summary>
    /// <param name="targetLeftGunIndex">目标修改的武器槽</param>
    /// <param name="currentChangeGunState">修改主武器 or 特殊武器枪</param>
    /// <param name="targetGunType">修改成什么类型的枪</param>
    public void ChangeGunType(GunState currentChangeGunState, GunType targetGunType, int targetLeftGunIndex = 0)
    {
        //todo 判断主副武器不能重复

        switch (currentChangeGunState)
        {
            //修改主武器的枪
            case GunState.NormalState:
                break;
            //修改特殊武器的枪
            case GunState.SpecialState:
                break;
        }
    }

    /// <summary>
    /// 冷却状态
    /// MP与HP数值使用监听
    /// </summary>
    private void MpAndHpListener()
    {
        //todo 判断当前血条状态(调用外部进行判断)
        BloodConsumeState newBloodConsumeState = BloodConsumeState.Mp;


        //对不同消耗状态进行操作
        if (newBloodConsumeState != m_CurrentBloodConsumeState)
        {
            if (newBloodConsumeState == BloodConsumeState.Mp)//新的状态为消耗Mp (可用特殊攻击，修改Enable)
            {
                AllGunCDic[RightGunType].IfGunCanUse(true);
            }
            else//新的状态为消耗Hp (不可用特殊攻击,修改Enable)
            {
                AllGunCDic[RightGunType].IfGunCanUse(false);
            }
            m_CurrentBloodConsumeState = newBloodConsumeState;
        }
    }

    //-----------------------------------------------------------------------------------
    //外部调用



    /// <summary>
    /// 核心状态改变监听器
    /// </summary>
    /// <param name="currentCore">角色当前的核心</param>
    public void CoreChangeLister(BaseCore currentCore)
    {

    }

    /// <summary>
    /// 修改总的伤害值
    /// </summary>
    /// <param name="offsetPercent">增加比例</param>
    /// <param name="durationTime">持续时间</param>
    public void ChangeButtleDemagePercent(float offsetPercent, int durationTime = 0)
    {
        m_CurrentButtleDemagePercent += offsetPercent;
        if (durationTime != 0)
        {
            StartCoroutine(DemagePercentDuration(offsetPercent, durationTime));
        }
    }
    #region 伤害持续时间判断
    IEnumerator DemagePercentDuration(float offsetPercent, int durationTime)
    {
        float Timer = 0;
        while (true)
        {
            Timer += Time.deltaTime;
            if (Timer >= durationTime)
                break;
            yield return 0;
        }
        m_CurrentButtleDemagePercent -= offsetPercent;
    }
    #endregion

    /// <summary>
    /// 子弹生成
    /// </summary>
    /// <param name="ButtleName">子弹名字</param>
    /// <param name="Pos">子弹初始位置</param>
    /// <param name="Rotate">子弹初始旋转</param>
    /// <param name="Scatte">散射度数</param>
    /// <param name="ButtleSpeed">子弹速度</param>
    /// <param name="AttackDistance">子弹飞行距离</param>
    /// <param name="DemageNums">子弹伤害</param>
    public void GenerateNormalButton(string ButtleName, Vector3 Pos, Vector3 Rotate, int Scatte, uint ButtleSpeed, float AttackDistance, float DemageNums)
    {
        //生成子弹（调整位置与角度）
        GameObject buttleGameObject = ObjectPool.Instance.Spawn(ButtleName);
        buttleGameObject.transform.position = Pos;
        buttleGameObject.transform.rotation = Quaternion.Euler(
            Rotate + new Vector3(0, 0, GenerateScatteringNums(Scatte)));
        //子弹数据填充
        Buttle buttle = buttleGameObject.GetComponent<Buttle>();
        float currentDeamge = DemageNums * m_CurrentButtleDemagePercent;//伤害比例加成
        buttle.BulletStart(ButtleSpeed, AttackDistance, currentDeamge);

        print("AK47特殊伤害：" + currentDeamge);
    }
    /// <summary>
    /// 散射随机值生成
    /// </summary>
    /// <param name="scatter">散射度数</param>
    /// <returns>散射角度的随机值</returns>
    protected float GenerateScatteringNums(int scatter)
    {
        return UnityEngine.Random.Range(scatter * -1.0f, scatter);
    }

}
