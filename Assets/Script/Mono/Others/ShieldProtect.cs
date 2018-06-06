using System.Collections;
using System.Collections.Generic;
using Chronos;
using UnityEngine;

public class ShieldProtect : MonoBehaviour {
    /// <summary>
    /// 外部调用
    /// </summary>
    public void ShieldCall()
    {
        m_ShieldKeepTiming = 0.0f;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 护盾下的目标
    /// </summary>
    public GameObject ProtectAimGameObject;

    private Timeline Time
    {
        get { return GetComponent<Timeline>(); }
    }
    /// <summary>
    /// 护盾持续时间
    /// </summary>
    private float m_ShieldKeepTiming = 0.0f;
    /// <summary>
    /// 护盾需要持续总时间
    /// </summary>
    private float ShieldShouldKeepTime = 1.5f;
    void Update()
    {
        if (m_ShieldKeepTiming <= ShieldShouldKeepTime)
        {
            m_ShieldKeepTiming += Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
