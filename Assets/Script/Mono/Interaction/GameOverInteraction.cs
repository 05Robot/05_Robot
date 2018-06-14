using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class GameOverInteraction : InteractionCheckBase
{
    public GameObject GameOver;

    /// <summary>
    /// 进入触发范围事件
    /// </summary>
    /// <param name="other">触发物体</param>
    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);
        if (ifInInteractionRange)
        {
            GameOver.SetActive(true);
            GameManager.Instance.ChangeGameStatu(GameManager.GameStatu.Pause);
        }
    }


}
