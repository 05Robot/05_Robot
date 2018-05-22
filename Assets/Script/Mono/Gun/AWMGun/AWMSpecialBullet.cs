using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AWMSpecialBullet : Bullet {

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }


    /// <summary>
    /// todo 子弹穿透敌人（连续击打好多个敌人）
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

    /// <summary>
    /// 正常造成伤害
    /// </summary>
    protected override void GenerateDemage()
    {

    }

}
