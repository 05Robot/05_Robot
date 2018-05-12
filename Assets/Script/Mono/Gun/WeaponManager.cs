using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    public GameObject[] LeftGun;//主武器
    public GameObject RightGun;//特助武器

    //当前子弹统一属性------------------------
    //当前射击出子弹的数目
    private static int m_CurrentButtleNum = 1;
    public static int CurrentButtleNum
    {
        get
        {
            return m_CurrentButtleNum;
        }

        set
        {
            m_CurrentButtleNum = value;
        }
    }

    //当前射击出子弹的伤害百分比
    private static float m_CurrentButtleDemagePercent = 1;
    public static float CurrentButtleDemagePercent
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


    protected override void Awake()
    {
        base.Awake();

    }

	void Start () {
		
	}
	
	void Update () {
		
	}



    /// <summary>
    /// 核心状态改变监听器
    /// </summary>
    /// <param name="currentCore">角色当前的核心</param>
    public void CoreChangeLister(BaseCore currentCore)
    {

    }
}
