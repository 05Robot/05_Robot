using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/20
****	描述 枪子弹基类
**********************************************************************/
public abstract class Buttle : MonoBehaviour
{
    //子弹速度
    private uint m_Speed;
    public uint Speed
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }
    //子弹飞行距离
    private uint m_FlyDistance;
    public uint FlyDistance
    {
        get { return m_FlyDistance; }
        set { m_FlyDistance = value; }
    }
    //子弹伤害
    private uint m_DemageNums;
    public uint DemageNums
    {
        get { return m_DemageNums; }
        set { m_DemageNums = value; }
    }
    //判断子弹是否被开启飞行
    private bool StartFly = false;
    //是否飞行状态
    private bool Flying = false;
    protected virtual void FixedUpdate()
    {
        if (Flying)//是否飞行状态
        {
            if (StartFly)//是否第一次飞行
            {
                m_AlreadyFlyDistance = 0;
                StartFly = false;
            }
            FlyAndLimit();//子弹飞行与距离限制
        }
    }

    //被开启时的初始化过程
    public virtual void BulletStart(uint s_Speed, uint s_FlyDistance, uint s_DemageNums)
    {
        Speed = s_Speed;
        FlyDistance = s_FlyDistance;
        DemageNums = s_DemageNums;
        StartFly = Flying = true;
    }

    //子弹直直飞行与距离限制
    private float m_AlreadyFlyDistance;//已经飞行的距离
    protected virtual void FlyAndLimit()
    {
        //子弹飞行
        transform.position += -transform.right * m_Speed * Time.fixedDeltaTime;
        m_AlreadyFlyDistance += m_Speed * Time.fixedDeltaTime;
        if (m_AlreadyFlyDistance > FlyDistance)
        {
            Vanish();//子弹消失
        }
    }

    //子弹消失（撞击或者超过距离）
    protected virtual void Vanish()
    {
        Flying = false;
        ObjectPool.Instance.Unspawn(gameObject);
    }
}
