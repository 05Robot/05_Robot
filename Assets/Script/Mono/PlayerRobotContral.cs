using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using Assets.Script.Mono;
using Assets.Script.Nomono;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerRobotContral : MonoBehaviour
{



    #region unity控制变量
    public PlayerRobot _mPlayerRobot;
    [Header("--修改项--")]
    [Rename("快速移动cd")]
    public float FastMoveCD=3f;
    [Rename("快速移动最大距离")]
    public float FastMoveDistance = 5;
    [Rename("移动速度")]
    public float MoveSpeed = 5f;
    /// <summary>
    /// 0 左下
    /// 1 左
    /// 2.左上
    /// 3.右上
    /// 4.右
    /// 5.右下
    /// </summary>
    [Rename("角色位图，6个方向")]
    public List<Sprite> Sprites = new List<Sprite>(6);

    #endregion
    //玩家是否可以控制
    private bool _iscontral = true;

    public bool Contral {
        get { return _iscontral; }
        set { _iscontral = value; }
    }
    [HideInInspector]
    public SpriteRenderer SR;
    [HideInInspector]
    public AudioSource AS;
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
        SR = GetComponent<SpriteRenderer>();
        AS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Contral)
        {
            CheckKey();
            CheckDitection();
        }
      

    }

    void CheckKey()
    {
        float speed = _mPlayerRobot.MoveSpeed;
        Vector2 direction = Vector2.zero;


        Rigidbody2D r2d = GetComponent<Rigidbody2D>();
        if (Input.GetKey(KeyCode.W))
            direction+=new Vector2(0,speed*Time.deltaTime);
        else if (Input.GetKey(KeyCode.S))
            direction += new Vector2(0, -speed * Time.deltaTime);




        if (Input.GetKey(KeyCode.A))
            direction += new Vector2(-speed * Time.deltaTime, 0);
        else if (Input.GetKey(KeyCode.D))
            direction += new Vector2(speed * Time.deltaTime, 0);

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

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(Bump(MoveSpeed * 2,20));
        }
    }

    void CheckDitection()
    {
        Vector2 target=Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //float angle= Vector2.Angle(Vector2.left, target);
        //if (Vector3.Cross(Vector2.left, v2).z > 0)
        //    angle = -angle;
        Vector2 v2 = ( (Vector2)transform.position- target ).normalized;
        float angle = -Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        //if (angle < 0)
        //    angle = 360 + angle;
        if (angle<30 && angle>=-30)
            SR.sprite = Sprites[0];
        else if (angle < 90 && angle >= 30)
            SR.sprite = Sprites[1];
        else if (angle <150 && angle >= 90)
            SR.sprite = Sprites[2];
        else if (angle < -150 || angle >= 150)
            SR.sprite = Sprites[3];
        else if (angle < -90 && angle >= -150)
            SR.sprite = Sprites[4];
        else if (angle <-30 && angle >=-90)
            SR.sprite = Sprites[5];


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
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
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


    #region 接口
  
    /// <summary>
    /// 返回核心生命值上下限及描述
    /// </summary>
    /// <param name="minhp"></param>
    /// <param name="maxhp"></param>
    /// <param name="discription"></param>
    public void GetCoreMinMaxHp(out int minhp, out int maxhp,out string discription)
    {
       minhp = _mPlayerRobot.Core.MinHp;
       maxhp= _mPlayerRobot.Core.MaxHp;
       discription = _mPlayerRobot.Core.Discription;
    }
    /// <summary>
    /// 设置核心最大生命值
    /// </summary>
    /// <param name="MaxHppoint"></param>
    public void SetMaxHppoint(int MaxHppoint)
    {
        if (_mPlayerRobot.Core.SetHpPoint(MaxHppoint))
            _mPlayerRobot.SyncHpMp();
        else
        {
            //todo 显示错误信息
        }

    }

    public IEnumerator Bump(float speed,float distance)
    {
        Vector2 v2=FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition);
        Vector2 Ditection = v2 - (Vector2)transform.position;
        List<EnemyContral> enemyList=new List<EnemyContral>();
        //暂定玩家layer为10,敌人为11,场景为12
        RaycastHit2D[] rh2d = Physics2D.RaycastAll(transform.position, Ditection, distance);
        Vector2 target = (Vector2)transform.position + Ditection.normalized * distance;
        DateTime dt = DateTime.Now;
        //先射线检测判断一下，是否有物体挡住

        foreach (var rh in rh2d)
        {
            if ( rh.transform.gameObject.layer == 12)
            {

                target = (Vector2)transform.position + Ditection.normalized * (Vector2.Distance(transform.position, rh.point) - 1);
                break;
            }
            else if (rh.transform.gameObject.layer == 11)
            {
                enemyList.Add(rh.transform.GetComponent<EnemyContral>());
            }
            else
            {
                continue;
            }
        }



        Contral = false;
        while (Vector2.Distance(transform.position, target) > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (enemyList.Count > 0)
            {
                foreach (var enemy in enemyList)
                {
                   
                    if (Vector2.Distance(enemy.transform.position, this.transform.position) < 2)
                    {
                        if (enemy.EAI.isFly==false)
                        {
                            Debug.Log("撞飞");
                            enemy.EAI.isFly = true;
                            enemy.Contral = false;
                            StartCoroutine(enemy.EAI .WaiteForMoveCD(target, speed));
                        }
                           
                            
                        
                       

                    }
                }
            }

            if ((DateTime.Now - dt).Seconds > 2)
                break;

            yield return 0;
        }
        Debug.Log("move over");
        Contral = true;
        foreach (var enemy in enemyList)
        {
            enemy.EAI.isFly = false;
            enemy.Contral = true;
        }
    }


    /// <summary>
    /// 普通伤害接口
    /// </summary>
    /// <param name="mpdamage"></param>
    /// <param name="hpdamage"></param>
    public void GetDamage(int mpdamage, int hpdamage)
    {
        _mPlayerRobot.GetDamage(mpdamage, hpdamage);
    }
    /// <summary>
    /// 真实伤害
    /// </summary>
    /// <param name="hpdamage"></param>
    public void GetRealDamage(int hpdamage)
    {
        _mPlayerRobot.GetReallyDamage(hpdamage);
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
        transform.Translate(((Vector2)transform.position - v2_mouse).normalized * distance, Space.World);
    }

    #endregion




}
