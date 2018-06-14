using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Mono;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class RocketBullet : Bullet {
    [Header("火箭筒和-普通攻击-参数")]
    [Rename("爆炸范围")][SerializeField]private float ExplosionRadius = 3.0f;
    [Rename("爆炸伤害")][SerializeField]private float ExplosionDemage = 1000.0f;
    //[Rename("硬直")][SerializeField]private float ExplosionHard = 2.0f;
    //[Rename("击退")][SerializeField]private float ExplosionBack = 0.5f;
    [SerializeField] private LayerMask layer;

    private List<int> EnemyIDList;

    protected override void Awake()
    {
        base.Awake();
        EnemyIDList = new List<int>();
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
    /// 击中敌人后产生一个伤害为1000的3个单位的AOE
    /// 并且造成2s硬直和0.5个单位的击退
    /// 超过距离还是会爆炸
    /// </summary>
    protected override void GenerateDemage()
    {
        ExplosionHitEffect();

        var shakePreset = ProCamera2DShake.Instance.ShakePresets[1];
        ProCamera2DShake.Instance.Shake(shakePreset);
    }


    private Transform MainEnemy;//射中的敌人
    /// <summary>
    /// 大范围爆炸
    /// </summary>
    //修改Physics2D.Raycast
    void ExplosionHitEffect()
    {
        EnemyIDList.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), ExplosionRadius, layer);//Physics.OverlapSphere()：球形范围内的碰撞器
        for (int i = 0; i < colliders.Length; i++)
        {
            EnemyContral hitEnemyContral = null;
            switch (colliders[i].transform.gameObject.layer)
            {
                //敌人护盾
                case 18:
                    if (EnemyIDList.Contains(colliders[i].transform.GetComponent<ShieldProtect>().ProtectAimGameObject.GetInstanceID())) continue;
                    hitEnemyContral = colliders[i].transform.GetComponent<ShieldProtect>().GetEnemyControl();
                    EnemyIDList.Add(colliders[i].transform.GetComponent<ShieldProtect>().ProtectAimGameObject.GetInstanceID());
                    break;
                //敌人内部
                case 11:
                    if (EnemyIDList.Contains(colliders[i].transform.GetInstanceID())) continue;
                    hitEnemyContral = colliders[i].transform.GetComponent<EnemyContral>();
                    EnemyIDList.Add(colliders[i].transform.GetInstanceID());
                    break;
                //紫水晶与零件箱
                case 19:
                case 20:
                    colliders[i].transform.GetComponent<HitCheckBase>().Broken();
                    break;
            }
            if (hitEnemyContral != null)
            {
                hitEnemyContral.GetDamage(Convert.ToInt32(ExplosionDemage), Convert.ToInt32(ExplosionDemage));
                //硬直
                hitEnemyContral.SetDelay(2, (int)HardStraight);
                //击退
                hitEnemyContral.SetKnockback(transform.position, 0.5f, (int)BeatBack);

            }
        }
    }

}
