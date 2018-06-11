using System.Collections;
using System.Collections.Generic;
using Assets.Script.Nomono;
using UnityEngine;

public class ElitesAi : EnemyAi
{
    [Header("近距离形态")]
    public float Type2_Distance = 7f;
    public float Type2_PreButtleAngel = 15;
    public int Type2_ButtleCount = 5;
    public int Type2_ButtleBatch = 3;
    public int Type2_Damage = 300;
    public float Type2_BattleDistance = 10;
    public uint Type2_BattleSpeed = 10;
    public float Type2_BatchDelay = 0.3f;
    private bool IsClose=false;

    private Transform ShootPoint;

    void Start()
    {
        prc = FindObjectOfType<PlayerRobotContral>();
        ShootPoint = transform.Find("ShootPoint");
    }

    public override void UpdateLogic()
    {
        //每帧判断玩家与本单位的距离，判断是否可以射击
        //PlayerRobotContral prc = GameObject.FindObjectOfType<PlayerRobotContral>();
        float dis = Mathf.Abs(Vector2.Distance(EC.transform.position, prc.transform.position));
        if (dis < AttentionDistence)
        {
            if (dis < Type2_Distance)
                IsClose = true;

            if (!IsShootCD)
            {
                if (IsClose)
                {
                    StartCoroutine(Type2_Attack());
                }
                else
                {
                    Attack(prc.transform.position);
                    EC.StartCoroutine(WaiteForShootCD());
                }
            }

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
        GameObject buttle = ObjectPool.Instance.Spawn("11.NormalEnemyBullet");

        buttle.transform.position = ShootPoint.position;
        buttle.transform.rotation = buttle.transform.rotation.LookTo2D(buttle.transform.position, v2);
        buttle.GetComponent<Bullet>().BulletStart(ButtleSpeed, ButtleFlyDistance, ButtleDamage);

    }

    public IEnumerator Type2_Attack()
    {
        for (int i = 0; i < Type2_ButtleBatch; i++)
        {
            float offset_angel=Type2_PreButtleAngel*(Type2_ButtleCount / 2);
            for (int j = 0; j < Type2_ButtleCount; j++)
            {
                GameObject buttle = ObjectPool.Instance.Spawn("11.NormalEnemyBullet");

                buttle.transform.position = ShootPoint.position;
                //buttle.transform.rotation = buttle.transform.rotation.LookTo2D(buttle.transform.position, v2);
                buttle.transform.rotation = buttle.transform.rotation.LookTo2D(buttle.transform.position, prc.transform.position);
                buttle.transform.Rotate(0,0,offset_angel);
                offset_angel -= Type2_PreButtleAngel;
                buttle.GetComponent<Bullet>().BulletStart(Type2_BattleSpeed, Type2_BattleDistance, Type2_Damage);
            }
            yield return new WaitForSeconds(Type2_BatchDelay);
        }
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
