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
        //实例化核心
     _mPlayerRobot=new PlayerRobot(null, 10f);   
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    CheckKey();

	}

    void CheckKey()
    {
        Rigidbody2D r2d=GetComponent<Rigidbody2D>();
        if (Input.GetKeyDown(KeyCode.W))
        {
            r2d.velocity=new Vector2(r2d.velocity.x,_mPlayerRobot.MoveSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            r2d.velocity = new Vector2(r2d.velocity.x, -_mPlayerRobot.MoveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            r2d.velocity = new Vector2(-_mPlayerRobot.MoveSpeed, r2d.velocity.y);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            r2d.velocity = new Vector2(_mPlayerRobot.MoveSpeed, r2d.velocity.y);
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
}
