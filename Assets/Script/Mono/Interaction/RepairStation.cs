using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairStation : InteractionCheckBase
{
    [Header("维修站信息：")]
    [SerializeField] private GameObject _RepairStationUI;
    //[SerializeField] private GameObject RepairStationTip;
    [Header("开启维修站图片")]
    [SerializeField] private Sprite _startRepairStationSprite;
    [SerializeField] private Sprite _closeRepairStationSprite;

    [SerializeField] private GameObject RepairStationTip;
    private SpriteRenderer _repairStationSpriteRenderer;


    void Start()
    {
        _repairStationSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 交互后触发事件
    /// </summary>
    protected override void InteractionEvent()
    {
        base.InteractionEvent();
        if (ifCaneInteractioning)
        {
            //游戏暂停
            TimeManager.Instance.StopGame();
        }
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
        if (ifInInteractionRange)
        {
            _repairStationSpriteRenderer.sprite = _startRepairStationSprite;
            TimeManager.Instance.InRepairStation = true;
            RepairStationTip.SetActive(true);
        }
    }

    /// <summary>
    /// 退出触发范围事件
    /// </summary>
    /// <param name="other">触发物体</param>
    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (!ifInInteractionRange)
        {
            _repairStationSpriteRenderer.sprite = _closeRepairStationSprite;
            TimeManager.Instance.InRepairStation = false;
            RepairStationTip.SetActive(false);
        }
    
    }
}