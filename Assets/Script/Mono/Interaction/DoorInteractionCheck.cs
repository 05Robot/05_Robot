using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractionCheck : InteractionCheckBase {
    [Header("开门状态：")]
    [SerializeField] private GameObject _openDoorGameObject;

    void Start()
    {

    }

    /// <summary>
    /// 进入触发范围事件
    /// </summary>
    /// <param name="other">触发物体</param>
    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);
        Destroy(gameObject);
        _openDoorGameObject.SetActive(true);
    }

    /// <summary>
    /// 退出触发范围事件
    /// </summary>
    /// <param name="other">触发物体</param>
    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }
}
