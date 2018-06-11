using System.Collections;
using System.Collections.Generic;
using Assets.Script.Nomono;
using UnityEngine;

public class TankAI : EnemyAi
{

    private Transform ShootPoint;
    private Transform type_blade;
    public int Type1_batch = 3;
    public float Type1_BatchDelay=0.3f;
    [Header("近距离形态")]
    public float Type2_Distance = 7f;
    public int Type2_ButtleBatch = 3;
    public int Type2_Damage = 300;
    public float Type2_BattleDistance = 10;
    public uint Type2_BattleSpeed = 10;
    public float Type2_BatchDelay = 0.3f;


    private bool IsType2 = false;
    
    void Start()
    {
        prc = FindObjectOfType<PlayerRobotContral>();
        ShootPoint = transform.Find("ShootPoint");
        type_blade = transform.Find("Type_Blade");
    }

    public override void UpdateLogic()
    {
        //每帧判断玩家与本单位的距离，判断是否可以射击
        //PlayerRobotContral prc = GameObject.FindObjectOfType<PlayerRobotContral>();

        if (!IsShootCD)
        {
            Attack(prc.transform.position);
            EC.StartCoroutine(WaiteForShootCD());
        }



        if (!IsMoveCD)
        {
            //随机找一点，要求在靠近玩家
            float pos_x;
            float pos_y;
            if (transform.position.x - prc.transform.position.x > 0)
            {
                pos_x = UnityEngine.Random.Range(-MoveDistance, 0);
                pos_y = UnityEngine.Random.Range(-MoveDistance, MoveDistance);
            }
            else
            {
                pos_x = UnityEngine.Random.Range(0, MoveDistance);
                pos_y = UnityEngine.Random.Range(-MoveDistance, MoveDistance);
            }

            Vector2 target = new Vector2(transform.position.x + pos_x, transform.position.y + pos_y);
            // Debug.Log(target);
            Move(target, EC.ER.MoveSpeed);
        }

    }
     
    public override void Attack(Vector2 v2)
    {
        if (IsType2)
            StartCoroutine(Type2_Attack());
        else
            StartCoroutine(Type1_Attack());


    }

    IEnumerator Type1_Attack()
    {
        for (int j = 0; j < Type1_batch; j++)
        {
            for (int i = 0; i < 12; i++)
            {
                GameObject buttle = ObjectPool.Instance.Spawn("11.NormalEnemyBullet");

                buttle.transform.position = ShootPoint.position;
                buttle.transform.rotation = Quaternion.Euler(0, 0, -180+15*j + i * 30);
                buttle.GetComponent<Bullet>().BulletStart(ButtleSpeed, ButtleFlyDistance, ButtleDamage);
            }
            yield return new WaitForSeconds(Type1_BatchDelay);
        }
        IsType2 = true;
    }

    IEnumerator Type2_Attack()
    {
        for (int i = 0; i < Type2_BatchDelay; i++)
        {
            for (int j = 0; j < type_blade.childCount; j++)
            {
                
                GameObject buttle = ObjectPool.Instance.Spawn("11.NormalEnemyBullet");

                buttle.transform.position = (type_blade.GetChild(j) as Transform).position;
                buttle.transform.rotation = buttle.transform.rotation.LookTo2D(buttle.transform.position,prc.transform.position);
                buttle.GetComponent<Bullet>().BulletStart(Type2_BattleSpeed, Type2_BattleDistance, Type2_Damage);
            }
           yield return new WaitForSeconds(Type2_BatchDelay);
        }
        IsType2 = false;
    }
    public override void Move(Vector2 target, float speed)
    {

        //射线检测是否有障碍物
        RaycastHit2D[] rh2d = Physics2D.RaycastAll(EC.transform.position, target, MoveDistance);
        //Debug.Log("开始移动");
        foreach (var rh in rh2d)
        {
            if (rh.transform.gameObject.layer == 10 || rh.transform.gameObject.layer == 12)
            {
                target = (Vector2)EC.transform.position + target.normalized * (Vector2.Distance(EC.transform.position, rh.point) - 1);
                EC.StartCoroutine(WaiteForMoveCD(target, speed));
                break;
            }
        }
        EC.StartCoroutine(WaiteForMoveCD(target, speed));


    }


    //public Timeline Time
    //{
    //    get { return GetComponent<Timeline>(); }
    //}

}
