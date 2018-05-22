using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/20
****	描述 所有枪普通子弹类
**********************************************************************/
public class AllGunNormalBullet : Bullet
{
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

    protected override void GenerateDemage()
    {
        base.GenerateDemage();

    }
}
