using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairStation : InteractionCheckBase
{
    [Header("维修站信息：")]
    [SerializeField] private GameObject RepairStationCanvas;
    [SerializeField] private GameObject RepairStationTip;

    /// <summary>
    /// 交互后触发事件
    /// </summary>
    protected override void InteractionEvent()
    {
        base.InteractionEvent();
        //触发的事件
        RepairStationCanvas.SetActive(true);


    }

    /// <summary>
    /// 退出后触发事件
    /// </summary>
    protected override void ExitInteractionEvent()
    {
        base.ExitInteractionEvent();
        //退出事件

    }

    /// <summary>
    /// 进入触发范围事件
    /// </summary>
    /// <param name="other">触发物体</param>
    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);
        RepairStationTip.SetActive(true);

    }

    /// <summary>
    /// 退出触发范围事件
    /// </summary>
    /// <param name="other">触发物体</param>
    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        RepairStationTip.SetActive(false);


    }
}