using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionCheck : MonoBehaviour {

    [Rename("当前主角")] [SerializeField] private GameObject m_Player;
    [Rename("检测范围/m")] [SerializeField] private float m_Radius = 3.0f;
    [SerializeField] private LayerMask m_LayerMask;
    private Collider2D[] m_AllCollider;


    //-------------------------------------
    //E键操作
    private bool IfPressEDown = false;
    private bool IfPressEUp = false;
    private bool IfPressEing = false;
    
    void Start () {
		
	}
	
	void Update ()
	{
	    PressEToPickUp_Check();
        m_AllCollider = Physics2D.OverlapCircleAll(m_Player.transform.position, m_Radius, m_LayerMask, -10.0f, 10.0f);
	    ColliderCheck();
	}

    #region 按E键检测
    /// <summary>
    /// 按E键检测
    /// </summary>
    private void PressEToPickUp_Check()
    {
        //按下
        if (Input.GetKeyDown(KeyCode.E))
        {
            IfPressEDown = true;
        }
        //按住
        if (Input.GetKey(KeyCode.E))
        {
            IfPressEing = true;
        }
        //抬起
        if (Input.GetKeyUp(KeyCode.E))
        {
            IfPressEUp = true;
        }
    }
    #endregion



    /// <summary>
    /// 周围物体判断
    /// </summary>
    private void ColliderCheck()
    {
        for (int i = 0; i < m_AllCollider.Length; i++)
        {
            int colliderLayer = m_AllCollider[i].gameObject.layer;
            string colliderTag = m_AllCollider[i].tag;
            string colliderName = m_AllCollider[i].name;

            //层判断
            switch (colliderLayer)
            {
                //15层：PlayerGun玩家武器层
                case 15:
                    GetNewGun(colliderName);
                    break;
            }
            //标签判断
            switch (colliderTag)
            {
            }
            //名字判断
            switch (colliderName)
            {
            }
        }
    }


    /// <summary>
    /// 获取新的武器
    /// </summary>
    /// <param name="collider2D"></param>
    private void GetNewGun(string GunName)
    {
        //是否按住E键
        if (IfPressEing)
        {
            //按E计时
            if (!PressETimeFunc())
            {
                return;
            }
        }
        else
        {
            return;
        }
        
        GunType newGunType = 0;
        switch (GunName)
        {
            case "AK47_Gun":
                newGunType =GunType.AK47Gun;
                break;
            case "Revolver_Gun":
                newGunType = GunType.RevolverGun;
                break;
            case "Shot_Gun":
                newGunType = GunType.ShotGun;
                break;
            case "Rocket_Gun":
                newGunType = GunType.RocketGun;
                break;
            case "AWM_Gun":
                newGunType = GunType.AWMGun;
                break;
            case "Sword_Gun":
                newGunType = GunType.Sword;
                break;
            case "Hammer_Gun":
                newGunType = GunType.Hammer;
                break;
        }
        WeaponManager.Instance.GetGun(newGunType);
    }



    private float PressETime = 0.0f;
    private const float PRESS_E_TIME = 1.0f;
    /// <summary>
    /// 按E计时
    /// </summary>
    /// <returns>计时结束？</returns>
    private bool PressETimeFunc()
    {
        PressETime += Time.deltaTime;
        if (PressETime >= PRESS_E_TIME)
        {
            PressETime = 0.0f;
            return true;
        }
        return false;
    }
}
