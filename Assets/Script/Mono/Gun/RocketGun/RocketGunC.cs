using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGunC : GunC {
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    /// <summary>
    /// 左普通攻击
    /// 1、 todo 击中敌人后产生一个伤害为1000的3个单位的AOE
    /// </summary>
    protected override void LeftNormalShot()
    {
        base.LeftNormalShot();

    }

}
