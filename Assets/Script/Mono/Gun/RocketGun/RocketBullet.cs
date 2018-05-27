using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBullet : Bullet {
    [Header("火箭筒和-普通攻击-参数")]
    [Rename("爆炸范围")][SerializeField]private float ExplosionRadius = 3.0f;
    [Rename("爆炸伤害")][SerializeField]private float ExplosionDemage = 1000.0f;
    [Rename("自身硬直")][SerializeField]private float ExplosionHard = 2.0f;
    [Rename("自身击退")][SerializeField]private float ExplosionBack = 0.5f;
    [SerializeField] private LayerMask layer;

    protected override void Awake()
    {
        base.Awake();   
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Vanish()
    {
        //若为碰撞，而是超过距离，创建爆炸效果
        if (!StartOnCollisionEnter)
        {
            base.GenerateExplosionEffect();
            GenerateDemage();
        }
        base.Vanish();
    }

    /// <summary>
    /// todo 击中敌人后产生一个伤害为1000的3个单位的AOE
    /// todo 并且造成2s硬直和0.5个单位的击退
    /// 超过距离还是会爆炸
    /// </summary>
    protected override void GenerateDemage()
    {
        ExplosionHitEffect();
    }


    private Transform MainEnemy;//射中的敌人
    /// <summary>
    /// 大范围爆炸
    /// </summary>
    //todo 修改Physics2D.Raycast
    void ExplosionHitEffect()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), ExplosionRadius, layer);//Physics.OverlapSphere()：球形范围内的碰撞器
        foreach (Collider2D collider in colliders)
        {
            /*if (!collider.attachedRigidbody)//若无刚体
            {
                continue;
            }
            if (collider.CompareTag("EnemyCollider"))
            {
                MainEnemy = collider.transform.parent;
                for (; ; )
                {
                    if (MainEnemy.CompareTag("Enemy"))
                    {
                        break;
                    }
                    MainEnemy = MainEnemy.transform.parent;
                }

            }*/
        }
    }

}
