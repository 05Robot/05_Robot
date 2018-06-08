using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using Assets.Script.Mono;
using Assets.Script.Nomono;
using Chronos;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/20
****	描述 枪子弹基类
**********************************************************************/
public enum CoreAttribute
{
    Null = 4,
    Initial = 0,//初始
    Fire = 1,//火焰
    Amethyst = 2,//紫水晶
    Frozen = 3,//冰冻
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
    //private Ray hitRay;//击中的射线
    protected RaycastHit2D[] hitPoint;// 击中的目标信息
    //protected int LastHitPointID = -1;//击中的物体的ID
    #endregion

    #region 子弹自身组件
    //protected PolygonCollider2D mPolygonCollider2D;
    //protected Rigidbody Rigidbody;
    public Timeline Time
    {
        get { return GetComponent<Timeline>(); }
    }
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
    //需要外部赋值进来
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
    //传进当前核心
    public void BulletStart(uint s_Speed, float s_FlyDistance, float s_DemageNums, 
        string s_GunName = "AK47Gun", CoreAttribute s_coreAttributeBullet = CoreAttribute.Initial)
    {
        Speed = s_Speed;
        FlyDistance = s_FlyDistance;
        DemageNums = s_DemageNums;
        GunName = s_GunName;
        StartFly = Flying = true;
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
    //Physics2D.Raycast
    private void OnCollision(Vector3 step)
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = new Vector2(-transform.right.x, -transform.right.y);

        hitPoint = Physics2D.RaycastAll(origin, direction, step.magnitude * RaycastAdvance, layerMask);

        for (int i = 0; i < hitPoint.Length; i++)
        {
            if (hitPoint[i].transform != null)
            {
                if (!StartOnCollisionEnter)
                {
                    StartOnCollisionEnter = true;
                    //击中创建爆炸效果
                    GenerateExplosionEffect();
                }
            }
        }
    }

    /// <summary>
    /// 创建爆炸效果
    /// </summary>
    protected void GenerateExplosionEffect()
    {
        for (int i = 0; i < hitPoint.Length; i++)
        {
            impactParticle = ObjectPool.Instance.Spawn(m_ObjectPoolName + impactParticleName);
            impactParticle.transform.position = transform.position;
            impactParticle.transform.rotation = Quaternion.FromToRotation(-transform.right, hitPoint[i].normal);
        }

        /*foreach (GameObject trail in trailParticles)
        {
            GameObject curTrail = transform.Find(projectileParticle.name + "/" + trail.name).gameObject;
            curTrail.transform.parent = null;
            Destroy(curTrail, 3f);
        }
        ParticleSystem[] trails = GetComponentsInChildren<ParticleSystem>();
        for (int i = 1; i < trails.Length; i++)
        {
            ParticleSystem trail = trails[i];
            if (trail.gameObject.name.Contains("Trail"))
            {
                trail.transform.SetParent(null);
                Destroy(trail.gameObject, 2f);
            }
        }
        */
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
    /// 打在护盾上或者是人物身上
    /// </summary>
    protected virtual void GenerateDemage()
    {
        //伤害取整
        int Damage = Convert.ToInt32(DemageNums);
        for (int i = 0; i < hitPoint.Length; i++)
        {
            EnemyContral hitEnemyContral = null;
            switch (hitPoint[i].transform.gameObject.layer)
            {
                //击中玩家护盾
                case 17:
                    //对玩家进行伤害（应该是扣mp）
                    hitPoint[i].transform.GetComponent<ShieldProtect>().GetPlayerControl().GetDamage(Damage, Damage);
                    break;
                //击中玩家内部
                case 10:
                    //对玩家进行伤害（应该是扣hp）
                    hitPoint[i].transform.GetComponent<PlayerRobotContral>().GetRealDamage(Damage);
                    break;
                //击中敌人护盾
                case 18:
                    //对敌人进行伤害（应该是扣mp）
                    hitEnemyContral = hitPoint[i].transform.GetComponent<ShieldProtect>().GetEnemyControl();
                    hitEnemyContral.GetDamage(Damage, Damage);
                    break;
                //击中敌人内部
                case 11:
                    //对敌人进行伤害（应该是扣hp）
                    hitEnemyContral = hitPoint[i].transform.GetComponent<EnemyContral>();
                    hitEnemyContral.GetRealDamage(Damage);
                    break;
            }
            
            //-----------------------------------------------------------------------------------------------------------------
            //核心攻击 AOE + 特殊效果
            //当前是否核心攻击 && 是否打中敌人护盾/敌人内部
            if (isCoreAttack && hitEnemyContral != null)
            {
                //不同的核心产生不同的AOE伤害
                switch (m_CurrentCoreAttribute)
                {
                    case CoreAttribute.Initial:
                        //3个范围内200伤害
                        EnemyDamage(CurrentAoeCollider2D(3),200);
                        break;
                    case CoreAttribute.Fire:
                        //3个范围内200伤害
                        EnemyDamage(CurrentAoeCollider2D(3),200);
                        //燃烧效果，敌人持续减血
                        hitEnemyContral.ER.AddAbnormalState(new AbnormalState_Burn(hitEnemyContral.ER, 3));
                        break;
                    case CoreAttribute.Amethyst:
                        //5个范围内400伤害
                        EnemyDamage(CurrentAoeCollider2D(5),400);
                        //todo 无视MP，修改紫水晶子弹检测layer，不添加敌人护盾检测，直接打在敌人身上
                        break;
                    case CoreAttribute.Frozen:
                        //3个范围内200伤害
                        EnemyDamage(CurrentAoeCollider2D(3), 200);
                        //冰冻效果，敌人持续减速
                        hitEnemyContral.ER.AddAbnormalState(new AbnormalState_Frozen(hitEnemyContral.ER, 3));
                        break;
                }
            }
        }


    }

    /// <summary>
    /// 敌人伤害
    /// </summary>
    private void EnemyDamage(Collider2D[] AllCollider2D, int DamageNums)
    {
        for (int i = 0; i < AllCollider2D.Length; i++)
        {
            EnemyContral hitEnemyContral = null;
            switch (AllCollider2D[i].transform.gameObject.layer)
            {
                //敌人护盾
                case 18:
                    hitEnemyContral = hitPoint[i].transform.GetComponent<ShieldProtect>().GetEnemyControl();
                    break;
                //敌人内部
                case 11:
                    hitEnemyContral = hitPoint[i].transform.GetComponent<EnemyContral>();
                    break;
            }

            if (hitEnemyContral != null)
            {
                hitEnemyContral.GetDamage(DamageNums, DamageNums);
                //todo 硬直
                //hitEnemyBaseRobot.EC.SetDelay(0.5f,4);
                Vector2 hitEnemyPos = new Vector2(hitEnemyContral.transform.position.x,
                    hitEnemyContral.transform.position.y);
                Vector2 thisBulletPos = new Vector2(transform.position.x, transform.position.y);
                //todo 击退
                //hitEnemyContral.SetKnockback(hitEnemyPos - thisBulletPos,);
            }
        }
    }


    /// <summary>
    /// 圆型AOE检测
    /// </summary>
    /// <param name="Range">检测范围</param>
    /// <returns>检测结果</returns>
    private Collider2D[] CurrentAoeCollider2D(int Range)
    {
        return Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), Range, layerMask);//Physics.OverlapSphere()：球形范围内的碰撞器
    }
}
