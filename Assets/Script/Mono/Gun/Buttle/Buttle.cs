using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/20
****	描述 枪子弹基类
**********************************************************************/
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Buttle : MonoBehaviour
{
    #region 子弹自身属性(速度，飞行距离，伤害值)
    //子弹速度
    private uint m_Speed;
    public uint Speed
    {
        get { return m_Speed; }
        set { m_Speed = value; }
    }
    //子弹飞行距离
    private float m_FlyDistance;
    public float FlyDistance
    {
        get { return m_FlyDistance; }
        set { m_FlyDistance = value; }
    }
    //子弹伤害
    private float m_DemageNums;
    public float DemageNums
    {
        get { return m_DemageNums; }
        set { m_DemageNums = value; }
    }
    #endregion

    #region 脚本内部临时变量
    //判断子弹是否被开启飞行
    private bool StartFly = false;
    //是否飞行状态
    private bool Flying = false;
    //是否开始碰撞
    private bool OnCollisionEnter = false;
    #endregion

    #region 子弹自身组件
    protected Rigidbody2D mRigidbody2D;
    protected PolygonCollider2D mPolygonCollider2D;
    #endregion

    //子弹创建初始化
    protected virtual void Awake()
    {
        mRigidbody2D = GetComponent<Rigidbody2D>();
        mPolygonCollider2D = GetComponent<PolygonCollider2D>();
        #region Rigidbody2D属性初始化
        mRigidbody2D.drag = 0;
        mRigidbody2D.angularDrag = 0;
        mRigidbody2D.gravityScale = 0;
        mRigidbody2D.freezeRotation = true;
        #endregion
    }

    protected virtual void FixedUpdate()
    {
        //是否飞行状态
        if (Flying)
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
    public virtual void BulletStart(uint s_Speed, float s_FlyDistance, float s_DemageNums)
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
       // transform.position += -transform.right * m_Speed * Time.fixedDeltaTime;
        mRigidbody2D.MovePosition(mRigidbody2D.position +
                                  new Vector2(-transform.right.x, -transform.right.y) * m_Speed * Time.deltaTime);
        m_AlreadyFlyDistance += m_Speed * Time.fixedDeltaTime;
        //子弹超过飞行距离 || 子弹碰撞到其他物体
        if (m_AlreadyFlyDistance > FlyDistance || OnCollisionEnter)
        {
            Vanish();//子弹消失
        }
    }

    //碰撞检测
    protected virtual void OnCollisionEnter2D(Collision2D collision2D)
    {
        //todo 判断碰撞，OnCollisionEnter赋值
    }

    //碰撞后产生伤害
    protected abstract void GenerateDemage();

    //子弹消失（撞击或者超过距离）
    protected virtual void Vanish()
    {
        //飞行状态收回
        Flying = false;
        //对象池回收
        ObjectPool.Instance.Unspawn(gameObject);
        //碰撞
        if (OnCollisionEnter)
        {
            GenerateDemage();
            OnCollisionEnter = false;
        }
    }
}
