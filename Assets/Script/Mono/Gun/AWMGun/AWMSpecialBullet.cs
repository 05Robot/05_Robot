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
    /// </summary>
    private bool HaveEnemyShield = false;
    
    protected override void GenerateDemage()
    {
        for (int i = 0; i < hitPoint.Length; i++)
        {
            switch (hitPoint[i].transform.gameObject.layer)
            {
                //击中敌人护盾
                case 18:
                    HaveEnemyShield = true;
                    break;
                //击中敌人内部
                case 11:
                    HaveEnemyShield = false;
                    break;
            }

            //敌人有护盾//对敌人进行伤害（应该是扣mp）
            if (HaveEnemyShield)
            {

            }
            //敌人没有护盾//对敌人进行伤害（应该是扣hp）
            else
            {

            }

            if (LastHitPointID != hitPoint[i].transform.GetInstanceID())
            {
                LastHitPointID = hitPoint[i].transform.GetInstanceID();
                //todo 进行伤害

            }

            HitPointIDHashSet.Add(hitPoint[i].transform.GetInstanceID());
        }
    }

}
