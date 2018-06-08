using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public enum GunType//枪种类
{
    Null = 7,//没有枪
    AK47Gun = 0,//突击步枪
    RevolverGun = 1,//左轮
    ShotGun = 2,//霰弹枪
    RocketGun = 6,//火箭筒
    AWMGun = 3,//狙击枪
    Sword = 4,//剑
    Hammer = 5//锤子
}
/// <summary>
/// 左手为主武器，右手为特殊武器
/// </summary>
public class WeaponManager : Singleton<WeaponManager>
{
    #region 血条消耗的状态（消耗HP/MP）
    private enum BloodConsumeState
    {
        Hp = 0,
        Mp = 1
    }
    #endregion

    #region 当前发射子弹，使用左手or右手
    private enum UseLeftOrRightGun
    {
        LeftGun = 0,
        RightGun = 1
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
    //private GunType m_CurrentRightGunType;//当前手上的特殊武器
    private int m_CurrentLeftGunIndex = 0;//当前手上主武器的武器槽索引
    #endregion


    #region 当前子弹统一属性
    //当前射击出子弹的数目
    private static int m_CurrentBulletNum = 1;
    //当前射击出子弹的伤害百分比
    private static float m_CurrentButtleDemagePercent = 1;
    #endregion

    #region 与主角角色属性有关
    private PlayerRobotContral m_PlayerRobotContral;
    private BloodConsumeState m_CurrentBloodConsumeState = BloodConsumeState.Mp;//消耗状态
    private UseLeftOrRightGun m_CurrentUseGunis = UseLeftOrRightGun.LeftGun;//当前用的是左手右手
    [Header("--有关核心--")]
    //public CoreAttribute m_CurrentCoreAttribute = CoreAttribute.Null;//当前核心状态
    public BaseCore m_CurrentBaseCore = null;//当前核心状态

    [SerializeField] private GameObject[] m_CoreBullet;//核心攻击子弹
    [SerializeField] private GameObject[] m_WeaponPos;//各个武器位置
    #endregion

    //------------------------------------
    //是否可用武器系统
    private bool WeaponManagerState = true;
    //是否可用切枪系统
    public bool ChangeLeftGunState = true;
    //是否可用特殊武器系统
    private bool SpecialGunState;
    //--------
    //当前真的真的拥有的武器
    private List<GunType> GunTypePossession;
    //当前火箭筒个数（最多一个）
    private int m_RocketGunNumber = 1;
    public int RocketGunNumber
    {
        get { return m_RocketGunNumber; }
    }
    //当前是否火箭筒状态
    private bool m_RocketGunState = false;




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
        //所有抢设为不可见
        foreach (var keyValuePair in AllGunCDic)
        {
            keyValuePair.Value.SpriteRendererEnabled = false;
        }
        #endregion

        #region 初始化（当前已拥有枪【LeftGun与LeftGunType配对,RightGun与RightGunType配对】、修改枪的状态【普通or特殊】、主武器默认显示一把(Enable设置)、当前手里的武器(普通与特殊)）
        //当前已有武器配对
        LeftGun = new GameObject[3];
        GunTypePossession = new List<GunType>();
        for (int i = 0; i < LeftGunType.Length; i++)
        {
            if (LeftGunType[i] == GunType.Null) continue;
            //武器初始化
            GunTypePossession.Add(LeftGunType[i]);
            LeftGun[i] = AllGunDic[LeftGunType[i]];
            AllGunCDic[LeftGunType[i]].GunState = GunState.NormalState;//修改枪的状态【普通】
            AllGunCDic[LeftGunType[i]].IfGunCanUse(false);   
        }
        //开始没有特殊武器
        //关闭特殊武器系统
        SpecialGunState = false;
        //RightGun = AllGunDic[RightGunType];
        //AllGunCDic[RightGunType].GunState = GunState.SpecialState;//修改枪的状态【特殊】
        //当前主武器默认显示第一把，并且设置Enable
        m_CurrentLeftGunType = LeftGunType[0];
        AllGunCDic[m_CurrentLeftGunType].SpriteRendererEnabled = true;//LeftGun[m_CurrentLeftGunIndex].SetActive(true);
        AllGunCDic[LeftGunType[m_CurrentLeftGunIndex]].IfGunCanUse(true);
        //当前手的武器(普通与特殊)类型复制
        m_CurrentLeftGunType = LeftGunType[m_CurrentLeftGunIndex];
        //m_CurrentRightGunType = RightGunType;
        #endregion

        //获取人物控制
        m_PlayerRobotContral = GameObject.FindWithTag("Player").GetComponent<PlayerRobotContral>();

        //todo 读取xml文件
    }


    void Update ()
    {
        //不可用武器系统
        if (!WeaponManagerState) return;

        
        //鼠标点击事件监听
        //MouseKeyListener();


        //鼠标滚轮监听事件
        MouseWheelListener();
        //键盘监听事件
        KeyBoardListener();
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
            int tempLeftGunIndex = m_CurrentLeftGunIndex;
            bool isChangeLeftGunOk;
            do
            {
                tempLeftGunIndex++;
                isChangeLeftGunOk = ChangeLeftGun(tempLeftGunIndex);

            } while (!isChangeLeftGunOk);
        }
        //向上滚动
        if (Input.GetAxis("Mouse ScrollWheel") > 0.1)
        {
            int tempLeftGunIndex = m_CurrentLeftGunIndex;
            bool isChangeLeftGunOk;
            do
            {
                tempLeftGunIndex--;
                isChangeLeftGunOk = ChangeLeftGun(tempLeftGunIndex);

            } while (!isChangeLeftGunOk);
        }
    }
    /// <summary>
    /// 键盘监听事件
    /// </summary>
    private void KeyBoardListener()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeLeftGun(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeLeftGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeLeftGun(2);

        if (Input.GetKeyDown(KeyCode.Q))
            ChangeToRocketGun();
    }

    #endregion


    #region 切枪函数
    /// <summary>
    /// 切换主武器的当前手里的枪
    /// </summary>
    /// <param name="TargetGunIndex">切换的目标的枪索引</param>
    private bool ChangeLeftGun(int TargetGunIndex)
    {
        //切枪系统无法用
        if (!ChangeLeftGunState)
            return true;
        //当前状态是否火箭筒状态
        if (m_RocketGunState)
            UseRocketGun(false);

        if (TargetGunIndex < 0)
            TargetGunIndex += LeftGunType.Length;
        TargetGunIndex = TargetGunIndex % LeftGunType.Length;

        //当前槽位为空,无法切枪
        if (LeftGunType[TargetGunIndex] == GunType.Null) return false;

        //修改武器属性--
        ChangeLeftGunData(m_CurrentLeftGunIndex, TargetGunIndex);
        //--------------
        //当前枪索引更新
        m_CurrentLeftGunIndex = TargetGunIndex;
        //当前抢类型更新
        m_CurrentLeftGunType = LeftGunType[m_CurrentLeftGunIndex];

        //更新CDSlider
        Target.Instance.InitZeroCDSlider();

        return true;
    }

    /// <summary>
    /// 切枪 【主武器】
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
        AllGunCDic[LeftGunType[lastLeftGunIndex]].SpriteRendererEnabled = false;//LeftGun[lastLeftGunIndex].SetActive(false);
        AllGunCDic[LeftGunType[currentLeftGunIndex]].SpriteRendererEnabled = true;//LeftGun[currentLeftGunIndex].SetActive(true);

    }

    /// <summary>
    /// 变换为火箭筒
    /// </summary>
    private void ChangeToRocketGun()
    {
        if (m_RocketGunNumber == 0) return;

        //当前武器消失不可用
        AllGunCDic[m_CurrentLeftGunType].IfGunCanUse(false);
        AllGunCDic[m_CurrentLeftGunType].SpriteRendererEnabled = false;
        //火箭筒显示
        AllGunCDic[GunType.RocketGun].IfGunCanUse(true);
        AllGunCDic[GunType.RocketGun].SpriteRendererEnabled = true;
        //更新当前左手武器类型为火箭筒
        m_CurrentLeftGunType = GunType.RocketGun;
        //开启火箭筒状态
        m_RocketGunState = true;
        //特殊武器无法使用
        SpecialGunState = false;
    }
    /// <summary>
    /// 火箭筒发射
    /// </summary>
    private void UseRocketGun(bool use)
    {
        m_RocketGunNumber = use ? 0 : 1;

        //火箭筒消失
        AllGunCDic[GunType.RocketGun].IfGunCanUse(false);
        AllGunCDic[GunType.RocketGun].SpriteRendererEnabled = false;
        //切换为原来的左手武器状态
        m_CurrentLeftGunType = LeftGunType[m_CurrentLeftGunIndex];
        //原来武器消失可用
        AllGunCDic[m_CurrentLeftGunType].IfGunCanUse(true);
        AllGunCDic[m_CurrentLeftGunType].SpriteRendererEnabled = true;
        //关闭火箭状态
        m_RocketGunState = false;
        //开启特殊武器功能
        SpecialGunState = true;
    }
    #endregion




    /// <summary>
    /// 冷却状态[内部调用判断]
    /// MP与HP数值使用监听
    /// 【每次射击时进行判断】
    /// </summary>
    private void MpAndHpListener(out bool IfSpecialGunCanUse)
    {
        //不可用特殊武器系统
        if (!SpecialGunState)
        {
            IfSpecialGunCanUse = false;
            return;
        }

        BloodConsumeState newBloodConsumeState = BloodConsumeState.Mp;
        //判断当前血条状态(调用外部进行判断)
        //当前不用核心，即当前消耗的是MP，如果核心不为空，即当前小号的是HP
        newBloodConsumeState = m_CurrentBaseCore == null ? BloodConsumeState.Mp : BloodConsumeState.Hp;

        //对不同消耗状态进行操作
        //【特殊武器 在消耗Hp情况下不能攻击】
        IfSpecialGunCanUse = true;
        if (newBloodConsumeState != m_CurrentBloodConsumeState)
        {
            if (newBloodConsumeState == BloodConsumeState.Mp)//新的状态为消耗Mp (可用特殊攻击，修改Enable)
            {
                IfSpecialGunCanUse = true;
            }
            else//新的状态为消耗Hp (不可用特殊攻击,修改Enable)
            {
                IfSpecialGunCanUse = false;
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
        m_CurrentBaseCore = currentCore;

        //当前不使用核心
        if (m_CurrentBaseCore == null)
            ChangeButtleDemagePercent(0.0f);
        else
        {
            //当前使用核心
            switch (m_CurrentBaseCore.Element)
            {
                //普通核心
                case BaseCore.CoreElement.Primary:
                    ChangeButtleDemagePercent(0.1f);
                    break;
                //火焰核心
                case BaseCore.CoreElement.Fire:
                    ChangeButtleDemagePercent(0.1f);
                    break;
                //冰冻核心
                case BaseCore.CoreElement.Ice:
                    ChangeButtleDemagePercent(0.2f);
                    break;
                //紫水晶
                case BaseCore.CoreElement.Amethyst:
                    ChangeButtleDemagePercent(0.4f);
                    break;
            }
        }
    }

    /// <summary>
    /// 角色旋转角度变化
    /// </summary>
    /// todo 修改枪支位置
    public void PlayerDirectionLister()
    {
        //todo WeaponPos
    }


    /// <summary>
    /// -------------------------------------------------------------------------------------------------------------------important!!!
    /// 特殊武器使用时，状态检测
    /// 判断当前特殊武器是否可以射击
    /// 1、特殊武器，判断是否是消耗Mp为不是Hp
    /// </summary>
    /// <returns>是否可以射击</returns>
    public bool SpecialGunCheckOut()
    {
        //判断特殊武器是否违背Hp不可用原则
        //【并且更新特殊武器GunSpecialEnable】
        bool ifSpecialGunCanUse;//特殊武器是否可以用
        MpAndHpListener(out ifSpecialGunCanUse);

        //如果特殊武器可以用【开启Renderer进行显示】
        if (ifSpecialGunCanUse)
        {
            //特殊武器可以用【开启Renderer进行显示，开启(开启可用)Enable】
            AllGunCDic[RightGunType].SpriteRendererEnabled = true;
            AllGunCDic[RightGunType].IfGunCanUse(true);
            //主武器不可用【关闭Renderer进行显示，关闭(开启可用)Enable】
            AllGunCDic[LeftGunType[m_CurrentLeftGunIndex]].SpriteRendererEnabled = false;
            AllGunCDic[LeftGunType[m_CurrentLeftGunIndex]].IfGunCanUse(false);
        }
        else//特殊武器不能用
        {
            return false;
        }


        //---------------------------------------------------
        //【特殊武器可以用情况下】
        //当前使用的是右手（特殊武器）
        m_CurrentUseGunis = UseLeftOrRightGun.RightGun;
        //判断当前特殊武器种类
        switch (RightGunType)
        {
            //以下等待攻击结束才切回主武器
            case GunType.AK47Gun:
                break;
            case GunType.Sword:
                break;
            case GunType.Hammer:
                break;
            case GunType.RocketGun:
                break;
            //----------------------------
            //todo 以下会自身【硬直】0.3s才切回主武器
            case GunType.RevolverGun:
                StartCoroutine(SpecialGunShowTiming());
                break;
            case GunType.ShotGun:
                StartCoroutine(SpecialGunShowTiming());
                break;
            case GunType.AWMGun:
                StartCoroutine(SpecialGunShowTiming());
                break;
        }

        return true;
    }
    #region 【特殊武器】使用后硬直计时
    //【特殊武器】使用后(武器)硬直计时
    IEnumerator SpecialGunShowTiming()
    {
        float specialGunShowTiming = 0;
        while (true)
        {
            specialGunShowTiming += Time.deltaTime;
            if (specialGunShowTiming >= 0.3f) break;
            yield return 0;
        }
        //更新当前使用左/右手
        m_CurrentUseGunis = UseLeftOrRightGun.LeftGun;

        //【特殊武器】使用完之后（主武器显示，特殊武器消失）
        //计时结束，消失特殊武器，展现主武器
        //特殊武器关闭（开启使用）【关闭Renderer进行显示，关闭(开启可用)Enable】
        AllGunCDic[RightGunType].IfGunCanUse(false);
        AllGunCDic[RightGunType].SpriteRendererEnabled = false;
        //主武器可用【开启Renderer进行显示，开启(开启可用)Enable】
        AllGunCDic[LeftGunType[m_CurrentLeftGunIndex]].SpriteRendererEnabled = true;
        AllGunCDic[LeftGunType[m_CurrentLeftGunIndex]].IfGunCanUse(true);
    }
    #endregion
    /// <summary>
    /// 【特殊武器】使用完之后（主武器显示，特殊武器消失）【AK47、剑、锤子】
    /// </summary>
    public void SpecialGunToNormalGun()
    {
        StartCoroutine(SpecialGunShowTiming());
    }

    #region 外部 来 修改总的伤害值比例 + 修改总的发弹量
    /// <summary>
    /// 修改总的伤害值
    /// </summary>
    /// <param name="offsetPercent">增加比例</param>
    /// <param name="durationTime">持续时间</param>
    public void ChangeButtleDemagePercent(float offsetPercent, int durationTime = 0)
    {
        m_CurrentButtleDemagePercent = 1;
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
    /// 修改总的发弹量
    /// </summary>
    /// <param name="addBulletNums">增加每次射击的子弹数</param>
    /// <param name="durationTime">持续时间</param>
    public void ChangeBulletNum(int addBulletNums, int durationTime = 0)
    {
        m_CurrentBulletNum += addBulletNums;
        if (durationTime != 0)
        {
            StartCoroutine(BulletNumDuration(addBulletNums, durationTime));
        }
    }
    #region 增加总的发弹量持续时间判断
    IEnumerator BulletNumDuration(int addBulletNums, int durationTime)
    {
        float Timer = 0;
        while (true)
        {
            Timer += Time.deltaTime;
            if (Timer >= durationTime)
                break;
            yield return 0;
        }
        m_CurrentBulletNum -= addBulletNums;
    }
    #endregion
    #endregion

    /// <summary>
    /// 单颗子弹生成
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
        if (m_CurrentBulletNum > 1)
        {
            //创建子弹【子弹散射，间隔15°】
            GenerateNormalButton(ButtleName, Pos, Rotate, m_CurrentBulletNum, 15, ButtleSpeed, AttackDistance, DemageNums);
            return;
        }
        //创建子弹【子弹散射随机值：GenerateScatteringNums(Scatte)】
        CreateBullet(ButtleName, Pos, Rotate, GenerateScatteringNums(Scatte), ButtleSpeed, AttackDistance, DemageNums);
    }
    /// <summary>
    /// 多个子弹生成
    /// </summary>
    /// <param name="bulletNums">子弹数目</param>
    /// <param name="interval">子弹间隔</param>
    /// <param name="ButtleName">子弹名字</param>
    /// <param name="Pos">子弹初始位置</param>
    /// <param name="Rotate">子弹初始旋转</param>
    /// <param name="Scatte">散射度数</param>
    /// <param name="ButtleSpeed">子弹速度</param>
    /// <param name="AttackDistance">子弹飞行距离</param>
    /// <param name="DemageNums">子弹伤害</param>
    public void GenerateNormalButton(string ButtleName, Vector3 Pos, Vector3 Rotate, int bulletNums, int intervalDegrees, uint ButtleSpeed, float AttackDistance, float DemageNums)
    {
        int halfBulletNums = bulletNums / 2;
        int halfIntervalDegrees = intervalDegrees / 2;
        //散射【一次性射出多颗子弹（有间隔）】
        for (int i = 0; i < halfBulletNums; i++)
        {
            CreateBullet(ButtleName, Pos, Rotate, halfIntervalDegrees, ButtleSpeed, AttackDistance, DemageNums);
            halfIntervalDegrees += intervalDegrees;
        }
        halfIntervalDegrees = -(intervalDegrees / 2);
        for (int i = 0; i < halfBulletNums; i++)
        {
            CreateBullet(ButtleName, Pos, Rotate, halfIntervalDegrees, ButtleSpeed, AttackDistance, DemageNums);
            halfIntervalDegrees -= intervalDegrees;
        }
    }

    /// <summary>
    /// 创建子弹
    /// </summary>
    /// <param name="ButtleName">子弹名字</param>
    /// <param name="Pos">子弹初始位置</param>
    /// <param name="Rotate">子弹初始旋转</param>
    /// <param name="Scatte">散射度数</param>
    /// <param name="ButtleSpeed">子弹速度</param>
    /// <param name="AttackDistance">子弹飞行距离</param>
    /// <param name="DemageNums">子弹伤害</param>
    private void CreateBullet(string ButtleName, Vector3 Pos, Vector3 Rotate, float Scatte, uint ButtleSpeed, float AttackDistance, float DemageNums)
    {
        //生成子弹（调整位置与角度）
        //【判断当前使用左/右手】
        GunType currentGunType = m_CurrentLeftGunType;
        switch (m_CurrentUseGunis)
        {
            case UseLeftOrRightGun.LeftGun:
                currentGunType = m_CurrentLeftGunType;
                break;
            case UseLeftOrRightGun.RightGun:
                currentGunType = RightGunType;
                break;
        }
        //【判断当前是否核心攻击】
        string currentBulletName = "Bullet/" + currentGunType + "/" + ButtleName;
        CoreAttribute currentCoreAttribute = CoreAttribute.Initial;
        if (m_CurrentBaseCore != null)
        {
            switch (m_CurrentBaseCore.Element)
            {
                case BaseCore.CoreElement.Primary:
                    currentBulletName = "Bullet/Core/Initial/" + m_CoreBullet[0].name;
                    currentCoreAttribute = CoreAttribute.Initial;
                    break;
                case BaseCore.CoreElement.Fire:
                    currentBulletName = "Bullet/Core/Fire/" + m_CoreBullet[1].name;
                    currentCoreAttribute = CoreAttribute.Fire;
                    break;
                case BaseCore.CoreElement.Amethyst:
                    currentBulletName = "Bullet/Core/Amethyst/" + m_CoreBullet[2].name;
                    currentCoreAttribute = CoreAttribute.Amethyst;
                    break;
                case BaseCore.CoreElement.Ice:
                    currentBulletName = "Bullet/Core/Frozen/" + m_CoreBullet[3].name;
                    currentCoreAttribute = CoreAttribute.Frozen;
                    break;
            }
        }
        //currentBulletName = ButtleName;//敌人子弹测试
        GameObject buttleGameObject = ObjectPool.Instance.Spawn(currentBulletName);
        buttleGameObject.transform.position = Pos;
        buttleGameObject.transform.rotation = Quaternion.Euler(
            Rotate + new Vector3(0, 0, Scatte));
        //子弹数据填充
        Bullet buttle = buttleGameObject.GetComponent<Bullet>();
        float currentDeamge = DemageNums * m_CurrentButtleDemagePercent;//伤害比例加成
        buttle.BulletStart(ButtleSpeed, AttackDistance, currentDeamge, currentGunType.ToString(), currentCoreAttribute);
        
        //更新当前使用左/右手
        m_CurrentUseGunis = UseLeftOrRightGun.LeftGun;

        //当前为火箭系统
        if (m_RocketGunState) { UseRocketGun(true); }
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




    //----------------------------------------------------------------------------------------------------
    //外部调用
    //捡武器  换武器等等


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

        //        //不可使用特殊武器系统
        //if (RightGunType == GunType.Null) SpecialGunState = false;
    }

    /// <summary>
    /// 捡到新武器
    /// </summary>
    /// <param name="newGunType"></param>
    public void GetGun(GunType newGunType)
    {
        if ((int)newGunType == 7) return;

        switch (newGunType)
        {
            //火箭筒（一次性）
            case GunType.RocketGun:
                m_RocketGunNumber = 1;
                break;
            //其他（永久性）
            default:
                GunTypePossession.Add(newGunType);
                //优先放入特殊武器槽
                if (RightGunType == GunType.Null)
                {
                    //开启特殊武器系统
                    SpecialGunState = true;
                    RightGunType = newGunType;
                    RightGun = AllGunDic[RightGunType];
                    AllGunCDic[RightGunType].GunState = GunState.SpecialState;//修改枪的状态【特殊】
                    AllGunCDic[RightGunType].IfGunCanUse(false);
                }
                else
                {
                    //其次一次放入主武器中
                    for (int i = 0; i < LeftGunType.Length; i++)
                    {
                        if (LeftGunType[i] == GunType.Null)
                        {
                            LeftGunType[i] = newGunType;
                            LeftGun[i] = AllGunDic[LeftGunType[i]];
                            AllGunCDic[LeftGunType[i]].GunState = GunState.NormalState;//修改枪的状态【普通】
                            AllGunCDic[LeftGunType[i]].IfGunCanUse(false);
                            break;
                        }
                    }
                }
                
                break;
        }
    }
    

    /// <summary>
    /// 当前武器系统是否可用
    /// </summary>
    /// <param name="newState">是否可用</param>
    public void ChangeWeaponManagerState(bool newState)
    {
        WeaponManagerState = newState;
        AllGunCDic[LeftGunType[m_CurrentLeftGunIndex]].IfGunCanUse(newState);
        AllGunCDic[RightGunType].IfGunCanUse(newState);
    }
}
