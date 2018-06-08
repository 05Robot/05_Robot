using System.Collections;
using System.Collections.Generic;
using Assets.Script.Mono;
using Chronos;
using UnityEngine;

public class ShieldProtect : MonoBehaviour {
    private enum ShieldType
    {
        Player = 0,
        Enemy = 1
    }
    private Timeline Time
    {
        get { return GetComponent<Timeline>(); }
    }


    /// <summary>
    /// 护盾下的目标
    /// </summary>
    [Header("护盾下的目标：")]
    public GameObject ProtectAimGameObject;
    [Header("护盾下的目标类型：")]
    [SerializeField] private ShieldType m_ProtectAimShieldType;
    [Header("护盾阻挡的对象层：")]
    [SerializeField] private LayerMask m_layerMask;


    private PlayerRobotContral m_PlayerRobotContral;
    private EnemyContral m_EnemyRobotContral;

    private CircleCollider2D m_CircleCollider2D;

    void Start()
    {
        m_CircleCollider2D = GetComponent<CircleCollider2D>();

        switch (m_ProtectAimShieldType)
        {
            //玩家护盾
            case ShieldType.Player:
                m_PlayerRobotContral = ProtectAimGameObject.GetComponent<PlayerRobotContral>();
                break;
            //敌人护盾
            case ShieldType.Enemy:
                m_EnemyRobotContral = ProtectAimGameObject.GetComponent<EnemyContral>();
                break;
        }

        transform.parent = null;
    }



    void Update()
    {
        transform.position = ProtectAimGameObject.transform.position;
        //护盾开闭
        switch (m_ProtectAimShieldType)
        {
            //玩家护盾
            case ShieldType.Player:
                //当前没有MP
                if (m_PlayerRobotContral._mPlayerRobot.CurrentMp <= 0)
                {
                    //关闭护盾
                    m_CircleCollider2D.enabled = false;
                }
                //当前有MP
                else
                {
                    //开启护盾
                    m_CircleCollider2D.enabled = true;
                }
                break;
            //敌人护盾
            case ShieldType.Enemy:
                //当前没有MP
                if (m_EnemyRobotContral.ER.CurrentMp <= 0)
                {
                    //关闭护盾
                    m_CircleCollider2D.enabled = false;
                }
                //当前有MP
                else
                {
                    //开启护盾
                    m_CircleCollider2D.enabled = true;
                }
                break;
        }

        //护盾持续
        if (m_ShieldKeepTiming <= ShieldShouldKeepTime)
        {
            m_ShieldKeepTiming += Time.deltaTime;
        }else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 护盾持续时间
    /// </summary>
    private float m_ShieldKeepTiming = 0.0f;
    /// <summary>
    /// 护盾需要持续总时间
    /// </summary>
    private float ShieldShouldKeepTime = 1.5f;
    /// <summary>
    /// 护盾开启调用
    /// </summary>
    private void ShieldCall()
    {
        m_ShieldKeepTiming = 0.0f;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    /// <summary>
    /// 获取玩家控制
    /// </summary>
    /// <returns></returns>
    public PlayerRobotContral GetPlayerControl()
    {
        ShieldCall();
        return m_PlayerRobotContral;
    }
    /// <summary>
    /// 获取敌人控制
    /// </summary>
    /// <returns></returns>
    public EnemyContral GetEnemyControl()
    {
        ShieldCall();
        return m_EnemyRobotContral;
    }

        
}
