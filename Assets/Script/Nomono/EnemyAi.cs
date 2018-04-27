using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script.Mono;
using UnityEngine;



/*********************************************************************
****	作者 冰块药丸
****	时间 18/4/18
****	描述 敌人的AI基类和子类
**********************************************************************/
namespace Assets.Script.Nomono
{
    [Serializable]
    public abstract class EnemyAi
    {
        [HideInInspector]
        public EnemyContral EC;
        
        public float AttentionDistence;
        public uint ButtleDamage;
        public uint ButtleFlyDistance;
        public uint ButtleSpeed;
        public float ShootCD;
        public float MoveCD;
        public float MoveDistance;
        protected bool IsShootCD = false;
        protected bool IsMoveCD = false;


        public abstract void Update();
        public abstract void Attack(Vector2 v2);
        public abstract void Move();
    }
    [Serializable]
    public class SampleAI : EnemyAi
    {

        public override void Update()
        {
            //每帧判断玩家与本单位的距离，判断是否可以射击
            PlayerRobotContral prc = GameObject.FindObjectOfType<PlayerRobotContral>();
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
                Move();
            }

        }

        public override void Attack(Vector2 v2)
        {
            GameObject buttle = ObjectPool.Instance.Spawn("AWM_Buttle");
         
            buttle.transform.Translate(EC.transform.position);
            buttle.transform.LookAt(v2);
            buttle.GetComponent<Buttle>().BulletStart(ButtleSpeed,ButtleFlyDistance ,ButtleDamage);

        }

        public override void Move()
        {
           //随机找一点
            float pos_x=UnityEngine.Random.Range(-MoveDistance,MoveDistance);
            float pos_y = UnityEngine.Random.Range(-MoveDistance, MoveDistance);
            Vector2 target=new Vector2(EC.transform.position.x+pos_x, EC.transform.position.y+pos_y);
            //射线检测是否有障碍物
            RaycastHit2D[] rh2d = Physics2D.RaycastAll(EC.transform.position, target, MoveDistance);
            Debug.Log("开始移动");
            foreach (var rh in rh2d)
            {
                if (rh.transform.gameObject.layer==10|| rh.transform.gameObject.layer==12)
                {
                    target = (Vector2)EC.transform.position + target.normalized * (Vector2.Distance(EC.transform.position, rh.point) - 1);
                    EC.StartCoroutine(WaiteForMoveCD(target));
                    break;
                }
            }
            EC.StartCoroutine(WaiteForMoveCD(target));


        }

        IEnumerator WaiteForShootCD()
        {
            IsShootCD = true;
            yield return new WaitForSeconds(ShootCD);
            IsShootCD = false;
        }
        IEnumerator WaiteForMoveCD(Vector2 target )
        {
            IsMoveCD = true;


            while (Vector2.Distance(EC.transform.position, target) > 1)
            {
                EC.transform.position = Vector3.MoveTowards(EC.transform.position, target, EC.ER.MoveSpeed * Time.fixedDeltaTime);
                yield return 0;
            }
            Debug.Log("enemy move over");
            yield return new WaitForSeconds(MoveCD);
            IsMoveCD = false;
        }
    }
}
