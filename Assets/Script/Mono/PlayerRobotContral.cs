using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using Assets.Script.Mono;
using Assets.Script.Nomono;
using Chronos;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerRobotContral : MonoBehaviour
{



    #region unity控制变量
    public PlayerRobot _mPlayerRobot;
    [Header("--修改项--")]
    [Rename("快速移动cd")]
    public float FastMoveCD = 3f;
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
    [Rename("角色快速移动位图")]
    public List<Sprite> Sprites = new List<Sprite>();

    #endregion
    //玩家是否可以控制
    private bool _iscontral = true;

    public bool Contral
    {
        get { return _iscontral; }
        set
        {
            _iscontral = value;
            Anim.SetBool("IsContral", Contral);
        }
    }
    [HideInInspector]
    public SpriteRenderer SR;
    [HideInInspector]
    public AudioSource AS;
    [HideInInspector]
    public Animator Anim;
    private bool _isFastMoveCd = false;
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
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Contral)
        {
            CheckKey();
            //CheckDitection();
        }


    }

    void CheckKey()
    {
        float speed = _mPlayerRobot.MoveSpeed;
        Vector2 direction = Vector2.zero;


        Rigidbody2D r2d = GetComponent<Rigidbody2D>();
        if (Input.GetKey(KeyCode.W))
            direction += new Vector2(0, speed * Time.deltaTime);
        else if (Input.GetKey(KeyCode.S))
            direction += new Vector2(0, -speed * Time.deltaTime);




        if (Input.GetKey(KeyCode.A))
            direction += new Vector2(-speed * Time.deltaTime, 0);
        else if (Input.GetKey(KeyCode.D))
            direction += new Vector2(speed * Time.deltaTime, 0);

        if (direction != Vector2.zero)
        {
            transform.Translate(direction);
            Anim.SetInteger("IdeaAngleType", 0);
            Anim.SetInteger("RunAngleType", CheckDitection());
        }
        else
        {
            Anim.SetInteger("IdeaAngleType", CheckDitection());
            Anim.SetInteger("RunAngleType", 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
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
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {

            FindObjectOfType<HockContral>().StartHock(FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition));
        }

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    StartCoroutine(Bump(MoveSpeed * 2,20));
        //}
    }

    int CheckDitection()
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 v2 = ((Vector2)transform.position - target).normalized;
        float angle = -Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

        if (angle < 30 && angle >= -30)
            return 2;
        else if (angle < 90 && angle >= 30)
            return 3;
        else if (angle < 150 && angle >= 90)
            return 4;
        else if (angle < -150 || angle >= 150)
            return 5;
        else if (angle < -90 && angle >= -150)
            return 6;
        else if (angle < -30 && angle >= -90)
            return 1;

        return 0;


    }
    /// <summary>
    /// 返回当前输入方向对应的精灵
    /// </summary>
    /// <param name="v2"></param>
    /// <returns></returns>
    public int CheckInputDirction(Vector2 v2)
    {
        float pos_x = v2.x;
        float pos_y = v2.y;
        if (pos_x <= 0 && pos_y < 0)
        {
            return 0;
        }
        else if (pos_x < 0 && pos_y.Equals(0))
        {
            return 1;
        }
        else if (pos_x < 0 && pos_y > 0)
        {
            return 2;
        }
        else if (pos_x >= 0 && pos_y > 0)
        {
            return 3;
        }
        else if (pos_x > 0 && pos_y.Equals(0))
        {
            return 4;
        }
        else if (pos_x > 0 && pos_y < 0)
        {
            return 5;
        }
        else
        {
            return 0;
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

                target = (Vector2)transform.position + Ditection.normalized * (Vector2.Distance(transform.position, rh.point) - 1);
                break;
            }
            else
            {
                continue;
            }
        }


        Contral = false;
        Anim.Play("Idea");
        Anim.SetInteger("IdeaAngleType", 0);
        Anim.SetInteger("RunAngleType", 0);
        Anim.SetInteger("FastMoveType", CheckInputDirction(Ditection));
        Anim.SetBool("IsFastMove", true);

        while (Vector2.Distance(transform.position, target) > 1)
        {
            //transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if ((DateTime.Now - dt).Seconds > 2)
                break;
            yield return 0;
        }
        Debug.Log("move over");
        Contral = true;
        Anim.SetBool("IsFastMove", false);

        //快速移动Effect粒子显示
        GameObject effectGO = ObjectPool.Instance.Spawn("0.PlaySprintLightEffect");
        effectGO.transform.position = transform.position;
        effectGO.transform.Rotate(Vector3.forward,
            Vector3.SignedAngle(effectGO.transform.right, new Vector3(-Ditection.x, -Ditection.y), Vector3.forward),
            Space.Self);
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
    /// 返回核心生命值上下限
    /// </summary>
    /// <param name="minhp"></param>
    /// <param name="maxhp"></param>
    /// <param name="discription"></param>
    public void GetCoreMinMaxHp(out int minhp, out int maxhp)
    {
        minhp = _mPlayerRobot.Core.MinHp;
        maxhp = _mPlayerRobot.Core.MaxHp;

    }
    /// <summary>
    /// 返回当前生命值已经当前最大生命值，不会修改
    /// </summary>
    public void GetCurrentAndMaxHp(out int currenthp, out int maxhp)
    {
        currenthp = _mPlayerRobot.CurrentHp;
        maxhp = _mPlayerRobot.MaxHp;
    }

    /// <summary>
    /// 返回当前MP已经当前最大MP，不会修改
    /// </summary>
    public void GetCurrentAndMaxMP(out int currentmp, out int maxmp)
    {
        currentmp = _mPlayerRobot.CurrentMp;
        maxmp = _mPlayerRobot.MaxMp;
    }
    /// <summary>
    /// 设置核心最大生命值
    /// </summary>
    /// <param name="MaxHppoint"></param>
    public void SetMaxHppoint(int MaxHppoint)
    {
        float HpRate = _mPlayerRobot.CurrentHp / (float)_mPlayerRobot.MaxHp;
        if (_mPlayerRobot.Core.SetHpPoint(MaxHppoint))
        {
            _mPlayerRobot.MaxHp = _mPlayerRobot.Core.CurrentHpPoint;
            _mPlayerRobot.CurrentHp = (int)(_mPlayerRobot.MaxHp * HpRate);
            _mPlayerRobot.SyncHpMp();
        }
        else
        {
            //todo 显示错误信息
        }

    }

    public IEnumerator Bump(float speed, float distance)
    {
        Vector2 v2 = FindObjectOfType<Camera>().ScreenToWorldPoint(Input.mousePosition);
        Vector2 Ditection = v2 - (Vector2)transform.position;
        List<EnemyContral> enemyList = new List<EnemyContral>();
        //暂定玩家layer为10,敌人为11,场景为12
        RaycastHit2D[] rh2d = Physics2D.RaycastAll(transform.position, Ditection, distance);
        Vector2 target = (Vector2)transform.position + Ditection.normalized * distance;
        DateTime dt = DateTime.Now;
        //先射线检测判断一下，是否有物体挡住

        foreach (var rh in rh2d)
        {
            if (rh.transform.gameObject.layer == 12)
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
                        if (enemy.EAI.isFly == false)
                        {
                            Debug.Log("撞飞");
                            enemy.EAI.isFly = true;
                            enemy.Contral = false;
                            StartCoroutine(enemy.EAI.WaiteForMoveCD(target, speed));
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

    public void ChangeCore(BaseCore core)
    {
        float HpRate = _mPlayerRobot.CurrentHp / (float)_mPlayerRobot.MaxHp;
        this._mPlayerRobot.Core = core;
        _mPlayerRobot.MaxHp = core.CurrentHpPoint;
        _mPlayerRobot.CurrentHp = (int)(_mPlayerRobot.MaxHp * HpRate);

    }

    /// <summary>
    /// 普通伤害接口
    /// </summary>
    /// <param name="mpdamage"></param>
    /// <param name="hpdamage"></param>
    public void GetDamage(int mpdamage, int hpdamage)
    {
        _mPlayerRobot.GetDamage(mpdamage, hpdamage);

        var shakePreset = ProCamera2DShake.Instance.ShakePresets[3];
        ProCamera2DShake.Instance.Shake(shakePreset);
    }
    /// <summary>
    /// 真实伤害
    /// </summary>
    /// <param name="hpdamage"></param>
    public void GetRealDamage(int hpdamage)
    {
        _mPlayerRobot.GetReallyDamage(hpdamage);


        var shakePreset = ProCamera2DShake.Instance.ShakePresets[3];
        ProCamera2DShake.Instance.Shake(shakePreset);
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

    public Timeline Time
    {
        get { return GetComponent<Timeline>(); }
    }


}
