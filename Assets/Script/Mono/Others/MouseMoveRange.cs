using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseMoveRange : MonoBehaviour
{
    private enum MouseState
    {
        InTheRange = 0,
        OutTheRange = 1
    }

    [SerializeField] private Transform Player;
    private ProCamera2DPointerInfluence PC2DPI;
    private bool m_ifCanMouseEnter;
    private MouseState m_currentMouseState, m_newMouseState;
    void Start()
    {
        PC2DPI = Camera.main.GetComponent<ProCamera2DPointerInfluence>();
        m_ifCanMouseEnter = false;
        m_currentMouseState = m_newMouseState = MouseState.InTheRange;
    }

    void FixedUpdate()
    {
        if (m_currentMouseState != m_newMouseState)
        {
            switch (m_newMouseState)
            {
                case MouseState.InTheRange://鼠标进入区域
                    PC2DPI.enabled = false;
                    //ProCamera2D.Instance.AddCameraTarget(Player);
                    break;
                case MouseState.OutTheRange: //鼠标离开区域
                    PC2DPI.enabled = true;
                    //ProCamera2D.Instance.RemoveAllCameraTargets(0.5f);
                    break;
            }
            
            m_currentMouseState = m_newMouseState;
        }
    }

    void Update()
    {
        Collider2D[] col = Physics2D.OverlapPointAll(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        m_ifCanMouseEnter = false;
        if (col.Length > 0)
        {
            foreach (Collider2D c in col)
            {
                if (c.name.Equals("MouseMoveRange"))
                {
                    m_ifCanMouseEnter = true;
                }
            }
        }

        if (m_ifCanMouseEnter)//在里面
            m_newMouseState = MouseState.InTheRange;
        else//在外面
            m_newMouseState = MouseState.OutTheRange;
    }
}
