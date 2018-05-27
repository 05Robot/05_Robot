using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using Assets.Script.Nomono;
using UnityEngine;

public class PlayerRobotContral : MonoBehaviour
{



    #region unity控制变量
    public PlayerRobot _mPlayerRobot;
    public float FastMoveCD=3f;
    public float FastMoveDistance = 5;
    public float MoveSpeed = 5f;
    #endregion
    //玩家是否可以控制
    private bool _iscontral = true;

    public bool Contral {
        get { return _iscontral; }
        set { _iscontral = value; }
    }


    private bool _isFastMoveCd=false;
    void Awake()
    {

    }
    // Use this for initialization
    void Start()
    {
        //实例化核心
        _mPlayerRobot = new PlayerRobot(this, new FireCore(), MoveSpeed);
        //开启每秒时间协程
        StartCoroutine(_mPlayerRobot.SecondEvent());
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Contral)
        {
            CheckKey();
        }
      

    }

    void CheckKey()
    {
        float speed = _mPlayerRobot.MoveSpeed;
        Vector2 direction = Vector2.zero;


        Rigidbody2D r2d = GetComponent<Rigidbody2D>();
        if (Input.GetKey(KeyCode.W))
            direction+=new Vector2(0,speed*Time.fixedDeltaTime);
        else if (Input.GetKey(KeyCode.S))
            direction += new Vector2(0, -speed * Time.fixedDeltaTime);




        if (Input.GetKey(KeyCode.A))
            direction += new Vector2(-speed * Time.fixedDeltaTime,0);
        else if (Input.GetKey(KeyCode.D))
            direction += new Vector2(speed * Time.fixedDeltaTime,0);

        if (direction!=Vector2.zero)
        {
            transform.Translate(direction);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!_mPlayerRobot.IsUseCore && !_isFastMoveCd)
            {
                if (direction == Vector2.zero)
                {
                    return;
                }

                speed = _mPlayerRobot.SpecialSpeed;
                StartCoroutine(FastMoveToPosition(speed, direction));
                StartCoroutine(FastMoveCd(FastMoveCD));
                r2d.velocity = Vector2.zero;
                return;
            }
          

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
          FindObjectOfType<HockContral>().StartHock(FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition));
        }
    }

 

    IEnumerator FastMoveToPosition(float speed, Vector2 Ditection)
    {
        //暂定玩家layer为10,敌人为11,场景为12
        RaycastHit2D[] rh2d = Physics2D.RaycastAll(transform.position, Ditection, FastMoveDistance);
        Vector2 target = (Vector2)transform.position + Ditection.normalized * FastMoveDistance;
        DateTime dt = DateTime.Now;
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


        Contral = false;
        while (Vector2.Distance(transform.position, target) > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
            if ((DateTime.Now - dt).Seconds >2)
                break;
            yield return 0;
        }
        Debug.Log("move over");
        Contral = true;

    }

    //协程，硬直时间
    public IEnumerator BeOutOfContral(float seconds)
    {
        Contral = false;
        yield return new WaitForSeconds(seconds);
        Contral = true;
    }

    public IEnumerator FastMoveCd(float cd)
    {
        _isFastMoveCd = true;
        yield return new WaitForSeconds(cd);
        Debug.Log("fastmove cd over");
        _isFastMoveCd = false;
    }

    public void GetDamage(int mpdamage,int hpdamage)
    {
        _mPlayerRobot.GetDamage(mpdamage,hpdamage);
    }
    /// <summary>
    /// 设置核心最大生命值
    /// </summary>
    /// <param name="MaxHppoint"></param>
    public void SetMaxHppoint(int MaxHppoint)
    {
        if (_mPlayerRobot.Core.SetHpPoint(MaxHppoint) )
            _mPlayerRobot.SyncHpMp();
        else
        {
            //todo 显示错误信息
        }
        
    }
    /// <summary>
    /// 设置硬直
    /// </summary>
    public void SetDelay(float seconds)
    {
        StartCoroutine(BeOutOfContral(seconds));
    }
    /// <summary>
    /// 设置击退
    /// </summary>
    /// <param name="distance"></param>
    public void SetKnockback(float distance)
    {
       Vector2 v2_mouse = FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition);
       transform.Translate(((Vector2)transform.position - v2_mouse).normalized*distance,Space.World);
    }


}
