using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public abstract class InteractionCheckBase : MonoBehaviour
{
    private enum InteractionType
    {
        PressNothing = 0,
        PressE = 1,
        PressingE = 2,
        PressEsc = 3
    }
    private enum ExitInteractionType
    {
        PressNothing = 0,
        PressE = 1,
        PressingE = 2,
        PressEsc = 3
    }
    protected Collider2D SelfCollider2D;
    [Header("检测的对象：")]
    [SerializeField] private LayerMask m_layerMask;
    [Header("交互的方式:")]
    [SerializeField] private InteractionType[] m_InteractionType;
    //[Header("退出的交互方式：")]
    //[SerializeField] private ExitInteractionType[] m_ExitInteractionType;
    //是否可以触发事件
    protected bool ifCaneInteractioning = false;
    //是否进入触发范围
    protected bool ifInInteractionRange = false;

    void Awake()
    {
        SelfCollider2D = GetComponent<CircleCollider2D>();
        SelfCollider2D.isTrigger = true;
    }

    // 按E最大持续时间
    private const float PRESSING_E_TIME = 1.0f;
    // 按E当前持续时间
    private float pressingETimeing = 0.0f;

    void Update()
    {
        if (ifInInteractionRange)//是否在触发范围之内
        {
            if (ifCaneInteractioning) //是否可以触发事件
            {
                foreach (InteractionType t in m_InteractionType)
                {
                    switch (t)
                    {
                        case InteractionType.PressNothing:
                            InteractionEvent();
                            break;
                        case InteractionType.PressE:
                            if (Input.GetKeyDown(KeyCode.E))
                                InteractionEvent();
                            break;
                        case InteractionType.PressingE:
                            if (Input.GetKey(KeyCode.E))
                            {
                                pressingETimeing += Time.deltaTime;
                                if (pressingETimeing >= PRESSING_E_TIME)
                                {
                                    InteractionEvent();
                                    pressingETimeing = 0.0f;
                                }
                            }
                            break;
                        case InteractionType.PressEsc:
                            if (Input.GetKeyDown(KeyCode.Escape))
                                InteractionEvent();
                            break;
                    }
                }
            }
            else//不可以触发事件
            {
                /*foreach (ExitInteractionType t in m_ExitInteractionType)
                {
                    switch (t)
                    {
                        case ExitInteractionType.PressNothing:
                            ExitInteractionEvent();
                            break;
                        case ExitInteractionType.PressE:
                            if (Input.GetKeyDown(KeyCode.E))
                                ExitInteractionEvent();
                            break;
                        case ExitInteractionType.PressingE:
                            if (Input.GetKey(KeyCode.E))
                                pressingETimeing += Time.deltaTime;
                            if (pressingETimeing >= PRESSING_E_TIME)
                            {
                                ExitInteractionEvent();
                                pressingETimeing = 0.0f;
                            }

                            if (Input.GetKeyUp(KeyCode.E))
                                pressingETimeing = 0.0f;
                            break;
                        case ExitInteractionType.PressEsc:
                            if (Input.GetKeyDown(KeyCode.Escape))
                                ExitInteractionEvent();
                            break;
                    }
                }*/
            }
        }
        
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    protected virtual void InteractionEvent()
    {
    }
    
    /// <summary>
    /// 退出触发事件
    /// </summary>
    protected virtual void ExitInteractionEvent()
    {

    }

    /// <summary>
    /// 进入触发检测
    /// </summary>
    /// <param name="other">触发的物体</param>
    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        //玩家进入范围
        if ((m_layerMask >> other.gameObject.layer & 1) != 1) return;
        ifInInteractionRange = true;
        ifCaneInteractioning = true;
    }

    /// <summary>
    /// 离开触发检测
    /// </summary>
    /// <param name="other">触发的物体</param>
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        //玩家离开范围
        if ((m_layerMask >> other.gameObject.layer & 1) != 1) return;
        ifInInteractionRange = false;
        ifCaneInteractioning = false;
    }
}
