using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*********************************************************************
****	作者 ZMK 
****	时间 2018/4/14
****	描述 单例抽象基类
**********************************************************************/
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;
    public static T Instance
    {
        get { return m_instance; }
    }

    protected virtual void Awake()
    {
        m_instance = this as T;
    }
}


