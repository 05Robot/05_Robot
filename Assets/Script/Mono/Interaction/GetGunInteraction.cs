using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGunInteraction : InteractionCheckBase
{
    [Header("当前拾取的武器类型：")]
    [SerializeField] private GunType currentGunType;
    [SerializeField] private GameObject GunTip;

    /// <summary>
    /// 交互后触发事件
    /// </summary>
    protected override void InteractionEvent()
    {
        base.InteractionEvent();
        WeaponManager.Instance.GetGun(currentGunType);
        Destroy(gameObject);
    }

    /// <summary>
    /// 退出后触发事件
    /// </summary>
    protected override void ExitInteractionEvent()
    {
        base.ExitInteractionEvent();


    }

    /// <summary>
    /// 进入触发范围事件
    /// </summary>
    /// <param name="other">触发物体</param>
    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);
        if(ifInInteractionRange)GunTip.SetActive(true);
    }

    /// <summary>
    /// 退出触发范围事件
    /// </summary>
    /// <param name="other">触发物体</param>
    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if(!ifInInteractionRange) GunTip.SetActive(false);


    }
}
