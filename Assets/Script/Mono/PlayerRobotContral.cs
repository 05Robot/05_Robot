using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using Assets.Script.Nomono;
using UnityEngine;

public class PlayerRobotContral : MonoBehaviour
{
    public PlayerRobot _mPlayerRobot;
    //玩家是否可以控制
    private bool IsContral = true;
    void Awake()
    {
   
    }
	// Use this for initialization
	void Start () {
        //实例化核心
        _mPlayerRobot = new PlayerRobot(FireCore.fc, 10f);
        GameManager.Instance.PRC = this;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    CheckKey();

	}

    void CheckKey()
    {
        float speed = _mPlayerRobot.MoveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            //speed = _mPlayerRobot.SpecialSpeed;
           // StartCoroutine(FastMoveToPosition(4*speed))

        }
        
        Rigidbody2D r2d=GetComponent<Rigidbody2D>();
        if (Input.GetKeyDown(KeyCode.W))
        {
            r2d.velocity=new Vector2(r2d.velocity.x, speed);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            r2d.velocity = new Vector2(r2d.velocity.x, -speed);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            r2d.velocity = new Vector2(-speed, r2d.velocity.y);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            r2d.velocity = new Vector2(speed, r2d.velocity.y);
        }

        if (Input.GetKeyUp(KeyCode.A)|| Input.GetKeyUp(KeyCode.D))
        {
            r2d.velocity=new Vector2(0, r2d.velocity.y);
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            r2d.velocity = new Vector2(r2d.velocity.x,0);
        }

    }

    public void SetContral(bool contral)
    {
        IsContral = contral;
        
    }

    IEnumerator FastMoveToPosition(float speed,Vector2 Ditection)
    {
      //  RaycastHit2D rh2d=Physics2D.Raycast(transform.position,)
       //先射线检测判断一下，是否有物体挡住
       SetContral(true);

        yield return 0;
    }
}
