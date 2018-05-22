using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK47SpecialBullet : Bullet
{
    [Header("突击步枪-特殊攻击-参数")]
    [Rename("爆炸范围")][SerializeField] private float ExplosionRadius = 3.0f;//爆炸范围
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
        base.Vanish();
    }

    /// <summary>
    /// todo 【榴弹Bullet】枪榴弹爆炸后会造成周围3个单位内的敌人400点伤害
    /// todo 并且造成2s硬直和0.5个单位的击退
    /// </summary>
    protected override void GenerateDemage()
    {
        //【榴弹Bullet】枪榴弹爆炸后会造成周围3个单位内的敌人400点伤害
        ExplosionHitEffect();
    }


    private Transform MainEnemy;//射中的敌人
    /// <summary>
    /// 大范围爆炸
    /// </summary>
    //todo 修改Physics2D.Raycast
    void ExplosionHitEffect()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x,transform.position.y), ExplosionRadius, layer);//Physics.OverlapSphere()：球形范围内的碰撞器
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
