using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using Assets.Script.Nomono;
using UnityEngine;

public class PlayerRobotContral : MonoBehaviour
{
    public PlayerRobot _mPlayerRobot;
    public float FastMoveDistance = 5;
    //玩家是否可以控制
    private bool IsContral = true;
    void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        //实例化核心
        _mPlayerRobot = new PlayerRobot(FireCore.fc, 10f);
        //开启每秒时间协程
        StartCoroutine(_mPlayerRobot.SecondEvent());
        _mPlayerRobot.PRC = this;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckKey();

    }

    void CheckKey()
    {
        float speed = _mPlayerRobot.MoveSpeed;



        Rigidbody2D r2d = GetComponent<Rigidbody2D>();
        if (Input.GetKey(KeyCode.W))
            r2d.velocity = new Vector2(r2d.velocity.x, speed);
        else if (Input.GetKey(KeyCode.S))
            r2d.velocity = new Vector2(r2d.velocity.x, -speed);
        else
            r2d.velocity = new Vector2(r2d.velocity.x, 0);



        if (Input.GetKey(KeyCode.A))
            r2d.velocity = new Vector2(-speed, r2d.velocity.y);
        else if (Input.GetKey(KeyCode.D))
            r2d.velocity = new Vector2(speed, r2d.velocity.y);
        else
            r2d.velocity = new Vector2(0, r2d.velocity.y);


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (r2d.velocity == Vector2.zero)
            {
                return;
            }

            speed = _mPlayerRobot.SpecialSpeed;
            StartCoroutine(FastMoveToPosition(speed, r2d.velocity));
            r2d.velocity = Vector2.zero;

        }
    }

    public void SetContral(bool contral)
    {
        IsContral = contral;

    }

    IEnumerator FastMoveToPosition(float speed, Vector2 Ditection)
    {
        //暂定玩家layer为10,敌人为11,场景为12
        RaycastHit2D[] rh2d = Physics2D.RaycastAll(transform.position, Ditection, FastMoveDistance);
        Vector2 target = (Vector2)transform.position + Ditection.normalized * FastMoveDistance;
        //先射线检测判断一下，是否有物体挡住
     
            foreach (var rh in rh2d)
            {
                if (rh.transform.gameObject.layer == 11 || rh.transform.gameObject.layer == 12)
                {

                    target = (Vector2)transform.position + Ditection.normalized * (Vector2.Distance(transform.position, rh.point)-1);
                    break;
                }
                else
                {
                    continue;
                }
            }
       

        SetContral(false);
        while (Vector2.Distance(transform.position, target) > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
            yield return 0;
        }
        Debug.Log("move over");
        SetContral(true);

    }

    public void GetDamage(int mpdamage,int hpdamage)
    {
        _mPlayerRobot.GetDamage(mpdamage,hpdamage);
    }
}
