using System.Collections;
using System.Collections.Generic;
using Assets.Script.Nomono;
using Chronos;
using UnityEngine;

public class SampleAi : EnemyAi
{
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
        if (Mathf.Abs(Vector2.Distance(EC.transform.position, prc.transform.position)) < AttentionDistence)
        {
            if (!IsShootCD)
            {
                Attack(prc.transform.position);
                EC.StartCoroutine(WaiteForShootCD());
            }

        }

        if (!IsMoveCD)
        {
            //随机找一点，要求在靠近玩家
            float pos_x;
            float pos_y;
            if (transform.position.x-prc.transform.position.x>0)
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
            Move(target,EC.ER.MoveSpeed);
        }

    }

    public override void Attack(Vector2 v2)
    {
        GameObject buttle = ObjectPool.Instance.Spawn("11.NormalEnemyBullet");

        buttle.transform.position = ShootPoint.position;
        buttle.transform.rotation = buttle.transform.rotation.LookTo2D(buttle.transform.position, v2);
        //Vector3 direction = (EC.transform.position - (Vector3)v2).normalized;
        //if (v2.y> buttle.transform.position.y)
        //{
        //    buttle.transform.rotation = Quaternion.Euler(0, 0, -Vector2.Angle(Vector2.right, direction));
        //}  
        //else
        //{
        //    buttle.transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, direction));
        //}
        //buttle.transform.rotation=Quaternion.Euler(0,0, Mathf.Atan2(direction.y, direction.x) * Mathf.Deg2Rad);


        buttle.GetComponent<Bullet>().BulletStart(ButtleSpeed, ButtleFlyDistance, ButtleDamage);

    }

    public override void Move(Vector2 target,float speed)
    {
      
        //射线检测是否有障碍物
        RaycastHit2D[] rh2d = Physics2D.RaycastAll(EC.transform.position, target, MoveDistance);
        //Debug.Log("开始移动");
        foreach (var rh in rh2d)
        {
            if (rh.transform.gameObject.layer == 10 || rh.transform.gameObject.layer == 12)
            {
                target = (Vector2)EC.transform.position + target.normalized * (Vector2.Distance(EC.transform.position, rh.point) - 2);
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
