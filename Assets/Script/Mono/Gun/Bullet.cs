using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/20
****	描述 枪子弹基类
**********************************************************************/
public enum CoreAttribute
{
    Null = 0,
    Initial = 1,//初始
    Fire = 2,//火焰
    Amethyst = 3,//紫水晶
    Frozen = 4,//冰冻
}
public enum BulletBelongTo{
    Player = 0,//玩家
    Enemy = 1//敌人
}
public abstract class Bullet : MonoBehaviour
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

    //核心属性
    private CoreAttribute m_CurrentCoreAttribute;
    public CoreAttribute CurrentCoreAttribute
    {
        get { return m_CurrentCoreAttribute; }
        set { m_CurrentCoreAttribute = value; }
    }
    #endregion

    #region 脚本内部临时变量
    //判断子弹是否被开启飞行
    private bool StartFly = false;
    //是否飞行状态
    protected bool Flying = false;
    //是否开始碰撞
    protected bool StartOnCollisionEnter = false;
    //枪支名字信息
    private string GunName;

    [SerializeField] private LayerMask layerMask;//检测层
    [SerializeField] private float RaycastAdvance = 2f;// 射线检测距离
    private Ray hitRay;//击中的射线
    private RaycastHit hitPoint;// 击中的目标信息
    #endregion

    #region 子弹自身组件
    //protected PolygonCollider2D mPolygonCollider2D;
    //protected Rigidbody Rigidbody;
    #endregion

    #region 子弹样式
    [Rename("子弹爆炸特效")]public GameObject impactParticle;
    [Rename("持续时间")] [SerializeField] protected float impactParticleTime;
    private string impactParticleName;
    [Rename("子弹飞行特效")]public GameObject projectileParticle;
    [Rename("持续时间")] [SerializeField] protected float projectileParticleTime;
    private string projectileParticleName;
    [Rename("枪口特效")]public GameObject muzzleParticle;
    [Rename("持续时间")] [SerializeField] private float muzzleParticleTime = 0.8f;
    private string muzzleParticleName;
    [Rename("子弹拖尾特效")]public GameObject[] trailParticles;

    [Header("-----------------")]
    //todo 一下需要外部赋值进来
    //是否是核心攻击
    [SerializeField]private bool isCoreAttack = false;
    private string m_ObjectPoolName;
    //核心属性
    [SerializeField] private CoreAttribute coreAttributeBullet;
    [SerializeField] private BulletBelongTo bulletBelongTo = BulletBelongTo.Player;
    #endregion

    //子弹创建初始化
    protected virtual void Awake()
    {
        //mPolygonCollider2D = GetComponent<PolygonCollider2D>();
        //Rigidbody = GetComponent<Rigidbody>();

        impactParticleName = impactParticle.name;
        projectileParticleName = projectileParticle.name;
        muzzleParticleName = muzzleParticle.name;

    }

    protected virtual void Update()
    {
        //是否飞行状态
        if (Flying)
        {
            if (StartFly)//是否开始飞行
            {
                m_AlreadyFlyDistance = 0;
                StartFly = false;
            }
            FlyAndLimit();//子弹飞行与距离限制
        }
    }


    //被开启时的初始化过程
    //todo 传进当前核心
    public void BulletStart(uint s_Speed, float s_FlyDistance, float s_DemageNums, 
        string s_GunName = "AK47Gun", bool s_isCoreAttack = false, CoreAttribute s_coreAttributeBullet = CoreAttribute.Initial)
    {
        Speed = s_Speed;
        FlyDistance = s_FlyDistance;
        DemageNums = s_DemageNums;
        GunName = s_GunName;
        StartFly = Flying = true;
        isCoreAttack = s_isCoreAttack;
        coreAttributeBullet = s_coreAttributeBullet;

        switch (bulletBelongTo)
        {
            case BulletBelongTo.Player:
                //是否【核心的子弹】攻击判断
                if (isCoreAttack)
                    m_ObjectPoolName = "Bullet/Core/" + coreAttributeBullet + "/";
                else
                    m_ObjectPoolName = "Bullet/" + GunName + "/";
                break;
            case BulletBelongTo.Enemy:
                m_ObjectPoolName = "Bullet/Enemy/";
                break;
        }

        //产生子弹与枪口特效
        GenerateProjectileAndMuzzleParticles();
    }

    /// <summary>
    /// 产生子弹与枪口特效
    /// </summary>
    private void GenerateProjectileAndMuzzleParticles()
    {
        Vector3 LooAtPos = transform.position - transform.right;

        //创建子弹特效
        projectileParticle = ObjectPool.Instance.Spawn(m_ObjectPoolName + projectileParticleName);
        projectileParticle.transform.position = transform.position;
        projectileParticle.transform.LookAt(LooAtPos);
        //projectileParticle.transform.Rotate(transform.right, 180, Space.Self);
        projectileParticle.transform.SetParent(transform);

        //创建枪口特效
        if (muzzleParticle)
        {
            muzzleParticle = ObjectPool.Instance.Spawn(m_ObjectPoolName + muzzleParticleName);
            muzzleParticle.transform.position = transform.position;
            muzzleParticle.transform.LookAt(LooAtPos);
            muzzleParticle.transform.Rotate(transform.right, 180,Space.Self);

            StartCoroutine(DelayUnspawnGameObject(muzzleParticle, muzzleParticleTime));
        }
    }



    //子弹直直飞行与距离限制
    private Vector3 nextStep;
    protected float m_AlreadyFlyDistance;//已经飞行的距离
    private void FlyAndLimit()
    {
        nextStep = -transform.right * m_Speed * Time.deltaTime;
        //碰撞检测
        OnCollision(nextStep);
        //飞行下一步
        transform.position += nextStep;
        //飞行距离统计
        m_AlreadyFlyDistance += m_Speed * Time.deltaTime;
        //子弹超过飞行距离 || 子弹碰撞到其他物体
        if (m_AlreadyFlyDistance > FlyDistance || StartOnCollisionEnter)
        {
            Vanish();//子弹消失
        }
    }

    /// <summary>
    /// 碰撞检测
    /// </summary>
    /// <param name="step">下一步移动方向</param>
    //todo 修改Physics2D.Raycast
    private void OnCollision(Vector3 step)
    {
        // 射线检测
        if (Physics.Raycast(transform.position, -transform.right, out hitPoint, step.magnitude * RaycastAdvance, layerMask))
        {
            //击中
            //hitRay = new Ray(transform.position, -transform.right);
            if (!StartOnCollisionEnter)
            {
                StartOnCollisionEnter = true;
                //击中创建爆炸效果
                GenerateExplosionEffect();
            }
        }
    }

    /// <summary>
    /// 创建爆炸效果
    /// </summary>
    private void GenerateExplosionEffect()
    {
        impactParticle = ObjectPool.Instance.Spawn(m_ObjectPoolName + impactParticleName);
        impactParticle.transform.position = transform.position;
        impactParticle.transform.rotation = Quaternion.FromToRotation(-transform.right, hitPoint.normal);

        //foreach (GameObject trail in trailParticles)
        {
            //GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
           // curTrail.transform.parent = null;
           // Destroy(curTrail, 3f);
        }

        /*ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();
        for (int i = 1; i < trails.Length; i++)
        {
            ParticleSystem trail = trails[i];
            if (trail.gameObject.name.Contains("Trail"))
            {
                trail.transform.SetParent(null);
                Destroy(trail.gameObject, 2f);
            }
        }*/
    }

    //子弹消失（撞击或者超过距离）
    protected virtual void Vanish()
    {
        //飞行状态收回
        Flying = false;

        //对象池回收
        projectileParticle.transform.SetParent(null);
        StartCoroutine(DelayUnspawnGameObject(projectileParticle, projectileParticleTime));
        StartCoroutine(DelayUnspawnGameObject(impactParticle, impactParticleTime));
        StartCoroutine(DelayUnspawnGameObject(gameObject, 2.5f));

        //碰撞产生伤害
        if (StartOnCollisionEnter)
        {
            GenerateDemage();
            StartOnCollisionEnter = false;
        }
    }

    /// <summary>
    /// 延时回收物体
    /// </summary>
    protected IEnumerator DelayUnspawnGameObject(GameObject go, float DelayTime)
    {
        yield return new WaitForSeconds(DelayTime);
        ObjectPool.Instance.Unspawn(go);
    }

    /// <summary>
    /// 普通攻击  碰撞后产生伤害
    /// </summary>
    protected virtual void GenerateDemage()
    {
        //todo 普通子弹造成的基本伤害


        //todo CoreAttribute CurrentCoreAttribute
        //todo 不同的核心产生不同的AOE伤害 + 附加状态 + 持续伤害/速度/无视Mp
    }
}
