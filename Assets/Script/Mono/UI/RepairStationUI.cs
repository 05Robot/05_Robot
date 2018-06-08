using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairStationUI : MonoBehaviour
{
    #region 核心
    [Header("有关核心--------")]
    //所有核心图片
    [Header("所有核心图片")]
    [SerializeField] private Sprite[] m_AllCore;
    //当前核心位置
    [Header("当前核心位置")]
    [SerializeField] private Image m_CurrentCorePos;
    //当前拥有核心显示
    [Header("当前拥有的核心显示：")]
    [SerializeField]private GameObject[] m_AllCoreShow;
    //所有核心文字
    [Header("所有核心文字")]
    [SerializeField] private Sprite[] m_AllCoreText;
    //当前核心文字位置
    [Header("当前核心文字位置")]
    [SerializeField] private Image m_CurrentCoreTextPos;
    //血管样式修改
    [Header("所有血管样式")][SerializeField]private Sprite[] m_AllHPMPStyle;
    //血管位置
    [Header("血管位置")] [SerializeField] private Image m_HPMPPos;
    //所有需要拖拽出来的核心的图片
    [Header("所有需要拖拽出来的核心的图片")]
    [SerializeField] private GameObject[] m_AllDragCore;
    //所有被复制出来跟随拖动的图片
    private GameObject[] m_AllCopyDraggingCore;
    /// <summary>
    /// 当前拥有的所有核心
    /// </summary>
    public CoreAttribute[] m_AllCoreType;
    /// <summary>
    /// 当前核心种类
    /// </summary>
    public CoreAttribute m_CurrentCoreType;
    #endregion

    #region 武器
    [Header("有关武器--------")]
    //所有武器图片
    [Header("所有装备的武器图片：")]
    [SerializeField]private Sprite[] m_AllGun;
    //当前武器位置
    [Header("当前已经装备的武器位置：")]
    [SerializeField] private Image[] m_NormalGun;
    [SerializeField] private Image m_SpecialGun;
    //当前拥有的武器显示
    [Header("当前拥有的武器显示：")]
    [SerializeField] private GameObject[] m_AllGunShow;
    //所有需要拖拽出来的武器图片
    [Header("所有需要拖拽出来的武器图片【武器库】：")]
    [SerializeField] private GameObject[] m_AllDragGun;
    //所有需要移动换位置的武器图片
    [Header("所有需要移动换位置的武器图片【装备栏】：")]
    [SerializeField] private GameObject[] m_AllDragHavingGun;
    //所有被复制出来跟随拖动的图片
    private GameObject[] m_AllCopyDraggingGun;

    /// <summary>
    /// 当前拥有的武器种类
    /// </summary>
    public GunType[] m_AllGunType;
    /// <summary>
    /// 当前装配上的武器种类
    /// </summary>
    public GunType[] m_CurrentGunType;
    #endregion

    #region 血条
    /// <summary>
    /// 血条总值
    /// </summary>
    private class CoreToBlood
    {
        public int AllBloodNums;
        public int HPUpperNums, MPUpperNums;
        public int HPLowerNums, MPLowerNums;
        public CoreToBlood(int AllBloodNums, int HPUpperNums, int HPLowerNums, int MPUpperNums, int MPLowerNums)
        {
            this.AllBloodNums = AllBloodNums;
            this.HPUpperNums = HPUpperNums;
            this.HPLowerNums = HPLowerNums;
            this.MPUpperNums = MPUpperNums;
            this.MPLowerNums = MPLowerNums;
        }
    }
    private CoreToBlood Initial = new CoreToBlood(2000, 1500, 500, 1500, 500);
    private CoreToBlood Fire = new CoreToBlood(1600, 1400, 1000, 600, 200);
    private CoreToBlood Amethyst = new CoreToBlood(2300, 1800, 1000, 1300, 500);
    private CoreToBlood Frozen = new CoreToBlood(1400, 1000, 300, 1100, 400);
    /// <summary>
    /// 所有核心对应的血条上下限
    /// </summary>
    private CoreToBlood[] m_AllCoreToBlood;
    [Header("有关血条--------")]
    [Header("主条")][SerializeField]private Slider MainSlider_MP;
    [SerializeField]private Slider MainSlider_HP;
    [Header("左红条")] [SerializeField] private Slider HpSilder;
    [Header("右蓝条")] [SerializeField] private Slider MpSilder;
    /// <summary>
    /// 当前主角拥有的HP与MP值
    /// </summary>
    public int m_CurrentHP, m_CurrentMP;
    /// <summary>
    /// 当前HP的上限和下限
    /// </summary>
    private CoreToBlood m_CurrentCoreToBlood;

    #endregion

    #region 商店

    [Header("有关商店--------")]
    [Header("零件数目显示：")] [SerializeField]private Text m_PartText;
    [Header("禁止买图片显示：")] [SerializeField] private GameObject[] m_CanNotBuyPos;
    [Header("各个价格：")] [SerializeField] private int[] m_AllThingPrice;
    /// <summary>
    /// 零件数目
    /// </summary>
    private int m_PartNums = 0;
    /// <summary>
    /// 火箭筒个数
    /// </summary>
    private int m_RocketNums = 0;
    /// <summary>
    /// 是否已有冰冻核心
    /// </summary>
    private bool m_HavingFrozenCore = false;
    //是否可买
    private bool[] m_ifCanBuySomeThing;
    #endregion
    private void Start()
    {
        //todo 获取当前拥有的武器种类，当前已经装备的武器种类
        //WeaponManager.Instance.

        m_AllCopyDraggingGun = new GameObject[6];
        m_AllCopyDraggingCore = new GameObject[4];
        #region 当前拥有的武器进行显示
        foreach (GunType gt in m_AllGunType)
        {
            //未获得该武器
            if (gt == GunType.Null) continue;
            m_AllGunShow[(int) gt].SetActive(true);
        }
        #endregion
        #region 当前装备的武器进行显示,装备Equipped显示
        for (int i = 0; i < m_CurrentGunType.Length; i++)
        {
            //当前武器装备栏为空
            if (m_CurrentGunType[i] == GunType.Null)
            {
                continue;
            }
            m_AllGunShow[(int)m_CurrentGunType[i]].transform.GetChild(2).gameObject.SetActive(true);
            //特殊武器
            if (i == 3)
            {
                m_SpecialGun.sprite = m_AllGun[(int) m_CurrentGunType[i]];
            }
            else
            {
                m_NormalGun[i].sprite = m_AllGun[(int)m_CurrentGunType[i]];
                _GunIndex = (int) m_CurrentGunType[i];
            }
        }
        #endregion
        #region 当前拥有的核心进行显示
        foreach (CoreAttribute ca in m_AllCoreType)
        {
            m_AllCoreShow[(int)ca].SetActive(true);
        }
        #endregion
        #region 当前装备的核心与文字进行显示,装备Equipped显示,血管样式修改
        m_CurrentCorePos.sprite = m_AllCore[(int) m_CurrentCoreType];
        m_CurrentCoreTextPos.sprite = m_AllCoreText[(int) m_CurrentCoreType];
        m_AllCoreShow[(int)m_CurrentCoreType].transform.GetChild(4).gameObject.SetActive(true);

        m_HPMPPos.sprite = m_AllHPMPStyle[(int) m_CurrentCoreType];

        _CoreIndex = (int) m_CurrentCoreType;
        #endregion

        #region 商店管理（零件数量）
        m_ifCanBuySomeThing = new bool[5]{true, true, true, true, true };
        m_PartText.text = m_PartNums.ToString();
        //禁止使用维修服务
        /*if (m_PartNums > ??)
        {
            m_ifCanBuySomeThing[0] = false;
        }*/
        //禁止买火箭
        if (m_RocketNums == 1)
        {
            m_ifCanBuySomeThing[1] = false;
            //todo 显示禁止买图片
            //m_CanNotBuyPos[]
        }
        //禁止买冰冻核心
        if (m_HavingFrozenCore)
        {
            m_ifCanBuySomeThing[2] = false;
            //todo 显示禁止买图片
        }
        #endregion

        #region 血条管理
        m_AllCoreToBlood = new CoreToBlood[4] {Initial, Fire, Amethyst, Frozen};

        m_CurrentCoreToBlood = m_AllCoreToBlood[_CoreIndex];

        #endregion
    }

    #region 血条管理控制



    /// <summary>
    /// 主控制
    /// </summary>
    public void MainSliderDragging()
    {
        //MainSlider_HP.value = m_CurrentCoreToBlood.AllBloodNums - MainSlider_MP.value;
        MainSlider_HP.value = 1 - MainSlider_MP.value;
    }
    #endregion






    #region 商店管理
    private int _storeThingIndex = 0;
    /// <summary>
    /// 选中要买的物体
    /// </summary>
    /// <param name="target"></param>
    public void ClickToBuy(int target)
    {
        _storeThingIndex = target;
    }
    public void Buy()
    {
        //不可买
        if (!m_ifCanBuySomeThing[_storeThingIndex]) return;
        //扣钱
        m_PartNums -= m_AllThingPrice[_storeThingIndex];
        //获取物品
        switch (_storeThingIndex)
        {
            //维修服务
            case 0:
                //todo 补血
                break;
            //火箭筒
            case 1:
                m_RocketNums = 1;
                m_ifCanBuySomeThing[1] = false;
                //todo 显示禁止购买画面
                break;
            //冰冻核心
            case 2:
                m_HavingFrozenCore = true;
                m_ifCanBuySomeThing[1] = false;
                //todo 显示禁止购买画面
                break;
        }
    }

    #endregion


    #region 核心拖拽装配系统
    //当前拖动装备的核心索引
    private int _CoreIndex;
    /// <summary>
    /// 装备核心
    /// </summary>
    public void CoreDrop()
    {
        //当前武器已经装配
        if (CheckCoreIsTaking(_CoreIndex))
            return;
        //-------------------------------------------
        m_CurrentCorePos.sprite = m_AllCore[_CoreIndex];
        m_CurrentCoreTextPos.sprite = m_AllCoreText[_CoreIndex];

        //数据修改
        CoreAttribute oldCoreAttribute = m_CurrentCoreType;
        //修改装备栏核心类型
        m_CurrentCoreType = (CoreAttribute) _CoreIndex;
        //开启装备的核心的Equipped
        m_AllCoreShow[_CoreIndex].transform.GetChild(4).gameObject.SetActive(true);
        //关闭之前武器的Equipped
        m_AllCoreShow[(int)oldCoreAttribute].transform.GetChild(4).gameObject.SetActive(false);

        //血管有关
        //修改血管样式
        m_HPMPPos.sprite = m_AllHPMPStyle[_CoreIndex];
        //修改核心对应血条上下限
        m_CurrentCoreToBlood = m_AllCoreToBlood[_CoreIndex];
        //todo 修改血管

        DestroyImmediate(m_AllCopyDraggingCore[_CoreIndex]);
    }
    //结束拖拽【先放下再结束拖拽】
    public void CoreEndDrag()
    {
        if (m_AllCopyDraggingCore[_CoreIndex] != null)
        {
            DestroyImmediate(m_AllCopyDraggingCore[_CoreIndex]);
        }
    }

    //开始拖动
    public void BeginToDragCore(int target)
    {
        //当前核心已经装配
        if (CheckCoreIsTaking(target))
            return;
        //-------------------------------------------

        _CoreIndex = target;
        m_AllCopyDraggingCore[target] = Instantiate(m_AllDragCore[target], m_AllDragCore[target].transform.position, m_AllDragCore[target].transform.rotation, transform);
        m_AllCopyDraggingCore[target].transform.localScale = new Vector3(1, 1, 1);
    }

    //拖动跟随
    public void DraggingCoreToShowAndFollow()
    {
        //当前核心已经装配
        if (CheckCoreIsTaking(_CoreIndex))
            return;
        //-------------------------------------------

        Vector3 m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 m_aimPos = new Vector3(m_mousePos.x, m_mousePos.y, m_AllCopyDraggingCore[_CoreIndex].transform.position.z);
        m_AllCopyDraggingCore[_CoreIndex].transform.position = m_aimPos;
    }

    /// <summary>
    /// 判断当前核心是否已经装备上去
    /// </summary>
    /// <param name="target">核心索引</param>
    /// <returns>是否已经核心</returns>
    private bool CheckCoreIsTaking(int target)
    {
        if ((int)m_CurrentCoreType == target)
            return true;
        return false;
    }



    #endregion


    #region 已装备武器自定义系统
    /// <summary>
    /// 当前需要转移的武器索引
    /// </summary>
    private int _HaveGunIndex = 0;
    /// <summary>
    /// 当前需要转移的武器的位置
    /// </summary>
    private int _HaveGunOriginPos;
    /// <summary>
    /// 判断是否正在移动已经装备的武器
    /// </summary>
    private bool isMoveEquippedGun = false;
    /// <summary>
    /// 装备武器
    /// </summary>
    /// <param name="GunPos">装备武器位置</param>
    public void HavingGunDrop(int GunPos)
    {
        if (!isMoveEquippedGun) return;
        #region 若无特殊武器且主武器就只有一把，则不允许移动到特殊武器
        if (GunPos == 3)
        {
            int NormalGunNums = 0;
            foreach (GunType gt in m_CurrentGunType)
                if (gt != GunType.Null) ++NormalGunNums;
            //不符合要求
            if (NormalGunNums < 2)
                return;
        }
        #endregion

        Sprite originSprite;
        //目标位置改变------------
        //特殊武器位置
        if (GunPos == 3)
        {
            originSprite = m_SpecialGun.sprite;
            m_SpecialGun.sprite = m_AllGun[_HaveGunIndex];
        }
        //普通武器位置
        else
        {
            originSprite = m_NormalGun[GunPos].sprite;
            m_NormalGun[GunPos].sprite = m_AllGun[_HaveGunIndex];
        }
        //原始位置改变------------
        //特殊武器位置
        if (_HaveGunOriginPos == 3)
        {
            m_SpecialGun.sprite = originSprite;
        }
        //普通武器位置
        else
        {
            m_NormalGun[_HaveGunOriginPos].sprite = originSprite;
        }

        //数据修改
        GunType oldGunType = m_CurrentGunType[GunPos];
        //修改目标装备栏武器类型
        m_CurrentGunType[GunPos] = m_AllGunType[_HaveGunIndex];
        //修改原始装备栏武器类型
        m_CurrentGunType[_HaveGunOriginPos] = oldGunType;
    }

    //结束拖拽【先放下再结束拖拽】
    public void HavingGunEndDrag()
    {
        if (!isMoveEquippedGun) return;
        isMoveEquippedGun = false;
        if (m_AllCopyDraggingGun[_HaveGunIndex] != null)
        {
            DestroyImmediate(m_AllCopyDraggingGun[_HaveGunIndex]);
        }
    }

    //开始拖动
    public void BeginToMoveHavingGun(int target)
    {
        if (m_CurrentGunType[target] == GunType.Null) return;
        isMoveEquippedGun = true;
        _HaveGunOriginPos = target;
        _HaveGunIndex = (int)m_CurrentGunType[target];
        m_AllCopyDraggingGun[_HaveGunIndex] = Instantiate(m_AllDragHavingGun[target], m_AllDragHavingGun[target].transform.position, m_AllDragHavingGun[target].transform.rotation, transform);
        m_AllCopyDraggingGun[_HaveGunIndex].transform.localScale = new Vector3(1, 1, 1);
    }

    //拖动跟随
    public void DraggingHavingGunToShowAndFollow()
    {
        if (!isMoveEquippedGun) return;
        Vector3 m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 m_aimPos = new Vector3(m_mousePos.x, m_mousePos.y, m_AllCopyDraggingGun[_HaveGunIndex].transform.position.z);
        m_AllCopyDraggingGun[_HaveGunIndex].transform.position = m_aimPos;
    }

    #endregion


    #region 武器拖拽装配系统
    //当前拖动装备的武器索引
    private int _GunIndex;
    /// <summary>
    /// 装备武器
    /// </summary>
    /// <param name="GunPos">装备武器位置</param>
    public void GunDrop(int GunPos)
    {
        //当前武器已经装配
        if (CheckGunIsTaking(_GunIndex))
            return;
        //-------------------------------------------
        //特殊武器位置
        if (GunPos == 3)
            m_SpecialGun.sprite = m_AllGun[_GunIndex];
        //普通武器位置
        else
            m_NormalGun[GunPos].sprite = m_AllGun[_GunIndex];

        //数据修改
        GunType oldGunType = m_CurrentGunType[GunPos];
        //修改装备栏武器类型
        m_CurrentGunType[GunPos] = m_AllGunType[_GunIndex];
        //开启装备的武器的Equipped
        m_AllGunShow[(int)m_CurrentGunType[GunPos]].transform.GetChild(2).gameObject.SetActive(true);
        //关闭之前武器的Equipped
        if(oldGunType != GunType.Null) m_AllGunShow[(int)oldGunType].transform.GetChild(2).gameObject.SetActive(false);

        DestroyImmediate(m_AllCopyDraggingGun[_GunIndex]);

    }
    //结束拖拽【先放下再结束拖拽】
    public void GunEndDrag()
    {
        if (m_AllCopyDraggingGun[_GunIndex] != null)
        {
            DestroyImmediate(m_AllCopyDraggingGun[_GunIndex]);
        }

        //当前武器已经装配
        //if (CheckGunIsTaking(_GunIndex))
        //    return;
    } 

    //开始拖动
    public void BeginToDrag(int target)
    {
        //当前武器已经装配
        if (CheckGunIsTaking(target))
            return;
        //-------------------------------------------

        _GunIndex = target;
        m_AllCopyDraggingGun[target] = Instantiate(m_AllDragGun[target], m_AllDragGun[target].transform.position, m_AllDragGun[target].transform.rotation, transform);
        m_AllCopyDraggingGun[target].transform.localScale = new Vector3(1, 1, 1);
    }

    //拖动跟随
    public void DraggingToShowAndFollow()
    {
        //当前武器已经装配
        if (CheckGunIsTaking(_GunIndex))
            return;
        //-------------------------------------------

        Vector3 m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 m_aimPos = new Vector3(m_mousePos.x, m_mousePos.y, m_AllCopyDraggingGun[_GunIndex].transform.position.z);
        m_AllCopyDraggingGun[_GunIndex].transform.position = m_aimPos;
    }

    /// <summary>
    /// 判断当前武器是否已经装备上去
    /// </summary>
    /// <param name="target">武器索引</param>
    /// <returns>是否已经装备</returns>
    private bool CheckGunIsTaking(int target)
    {
        foreach (GunType gt in m_CurrentGunType)
            //当前武器已经装配
            if (gt == m_AllGunType[target])
                return true;
        return false;
    }
    #endregion


    #region 按钮管理（返回游戏【保存】、个人资料、主菜单、帮助）

    

    #endregion


    //选择与被选
    public void SelectToShow(GameObject go)
    {
        go.SetActive(true);
    }
    public void DeSelectToNoShow(GameObject go)
    {
        go.SetActive(false);
    }
}
