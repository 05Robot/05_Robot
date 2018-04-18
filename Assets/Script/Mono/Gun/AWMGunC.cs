using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/15
****	描述 狙击枪控制类
**********************************************************************/
public class AWMGunC : GunC
{
    
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

    //普通射击
    protected override void NormalShot()
    {
        base.NormalShot();

    }
    //特殊射击
    public override void SpecialShot()
    {
    }
}
