using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Mono;
using Assets.Script.Nomono;
using UnityEngine;

public class AWMSpecialBullet : Bullet {

    protected HashSet<int> HitPointIDHashSet = new HashSet<int>();//击中的物体的ID集合

    private bool FirstToFly = true;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if (Flying && FirstToFly)
        {
            HitPointIDHashSet.Clear();
            FirstToFly = false;
        }

        if (!Flying)
        {
            FirstToFly = true;
        }
    }


    /// <summary>
    /// 子弹穿透敌人（连续击打好多个敌人）
    /// </summary>
    protected override void Vanish()
    {
        // 大于飞行距离
        if (m_AlreadyFlyDistance > FlyDistance)
        {
            VanishCompletely();
        }
        else//继续飞行继续碰撞
        {
            VanishInCompletely();
        }
    }


    /// <summary>
    /// 完全消失掉
    /// </summary>
    private void VanishCompletely()
    {
        //飞行状态收回
        Flying = false;

        //对象池回收
        projectileParticle.transform.SetParent(null);
        StartCoroutine(DelayUnspawnGameObject(projectileParticle, projectileParticleTime));
        StartCoroutine(DelayUnspawnGameObject(impactParticle, impactParticleTime));
        StartCoroutine(DelayUnspawnGameObject(gameObject, 2.5f));

    }

    /// <summary>
    /// 产生爆炸后继续飞行
    /// </summary>
    private void VanishInCompletely()
    {
        StartCoroutine(DelayUnspawnGameObject(impactParticle, impactParticleTime));

        //碰撞产生伤害
        if (StartOnCollisionEnter)
        {
            GenerateDemage();
            StartOnCollisionEnter = false;
        }
    }

    //对敌人进行伤害，每个敌人伤害一次
    /// <summary>
    /// 正常造成伤害
    /// todo 设置硬直与击退
    /// </summary>
    protected override void GenerateDemage()
    {
        for (int i = 0; i < hitPoint.Length; i++)
        {
            //击中敌人护盾
            if (hitPoint[i].transform.gameObject.layer == 18)
            {
                //已经击中过这个护盾了
                if (HitPointIDHashSet.Contains(hitPoint[i].transform.GetInstanceID()))
                {
                    //不做任何操作
                }
                //没有击中过这个护盾了
                else
                {
                    //对这个敌人进行MP扣除
                    hitPoint[i].transform.GetComponent<ShieldProtect>().GetEnemyControl()
                        .GetDamage(Convert.ToInt32(DemageNums), Convert.ToInt32(DemageNums));
                    //添加击中的护盾的ID
                    HitPointIDHashSet.Add(hitPoint[i].transform.GetInstanceID());
                    //添加护盾里的敌人ID
                    HitPointIDHashSet.Add(hitPoint[i].transform.GetComponent<ShieldProtect>().ProtectAimGameObject
                        .GetInstanceID());

                    //设置硬直击退
                    hitPoint[i].transform.GetComponent<ShieldProtect>().GetEnemyControl()
                        .SetDelay(0.5f, 4);
                    hitPoint[i].transform.GetComponent<ShieldProtect>().GetEnemyControl()
                        .SetKnockback(-transform.right.normalized, 0.5f, 4);
                }
            }
            //击中敌人内部
            if (hitPoint[i].transform.gameObject.layer == 11)
            {
                //已经击中过这个敌人了
                if (HitPointIDHashSet.Contains(hitPoint[i].transform.GetInstanceID()))
                {
                    //不做操作
                }
                //没有击中过这个敌人
                else
                {
                    //对这个敌人进行HP扣除
                    hitPoint[i].transform.GetComponent<EnemyContral>().GetRealDamage(Convert.ToInt32(DemageNums));
                    //添加敌人ID
                    HitPointIDHashSet.Add(hitPoint[i].transform.GetInstanceID());

                    //设置硬直击退
                    hitPoint[i].transform.GetComponent<EnemyContral>().SetDelay(0.5f, 4);
                    hitPoint[i].transform.GetComponent<EnemyContral>()
                        .SetKnockback(-transform.right.normalized, 0.5f, 4);
                }
            }


            //击中紫水晶与零件箱
            if(hitPoint[i].transform.gameObject.layer == 19 || hitPoint[i].transform.gameObject.layer == 20)
                hitPoint[i].transform.GetComponent<HitCheckBase>().Broken();
        }
    }

}
