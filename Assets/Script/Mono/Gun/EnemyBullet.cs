using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/5/20
****	描述 敌人子弹类
**********************************************************************/
public class EnemyBullet : Bullet {
    /// <summary>
    /// 敌人子弹类型 
    /// </summary>
    private enum EnemyBulletType
    {
        NormalEnemyBullet = 0,
        EliteEnemyBullet = 1,
        BulletScreen = 2
    }
    [SerializeField] private EnemyBulletType m_CurrentEnemyBulletType = EnemyBulletType.NormalEnemyBullet;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
    }
    //子弹消失（撞击或者超过距离）
    protected override void Vanish()
    {
        base.Vanish();
    }


    /*
    //todo 敌人子弹伤害重写
    //todo 对玩家进行扣血：MP / HP
    protected override void GenerateDemage()
    {
        switch (m_CurrentEnemyBulletType)
        {
            case EnemyBulletType.NormalEnemyBullet:


                break;
            case EnemyBulletType.BulletScreen:


                break;
            case EnemyBulletType.EliteEnemyBullet:


                break;
        }
    }
    */
}
