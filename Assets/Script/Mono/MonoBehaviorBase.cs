using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/15
****	描述 
**********************************************************************/
public class MonoBehaviorBase : MonoBehaviour
{

    protected virtual void Update()
    {
        return;
    }

    protected int TimeCount = 0;
    protected bool TimeChange = false;
    protected virtual void FixedUpdate()
    {
        if (TimeChange)
        {
            if (TimeCount != 6)
            {
                TimeCount++;
                return;
            }else
            {
                TimeCount = 0;
            }
        }
    }
}
