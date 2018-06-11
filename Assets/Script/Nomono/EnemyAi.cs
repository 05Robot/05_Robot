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
  
    public abstract class EnemyAi:MonoBehaviour
    {
        [HideInInspector]
        public EnemyContral EC;
        [Rename("警戒范围")]
        public float AttentionDistence=10;
        [Rename("伤害")]
        public uint ButtleDamage=100;
        [Rename("攻击距离")]
        public uint ButtleFlyDistance=50;
        [Rename("子弹速度")]
        public uint ButtleSpeed=30;
        [Rename("攻击间隔时间")]
        public float ShootCD=3;
        [Rename("移动间隔时间")]
        public float MoveCD=5;
        [Rename("最大移动距离")]
        public float MoveDistance=5;
        protected bool IsShootCD = false;
        protected bool IsMoveCD = false;
        protected PlayerRobotContral prc;


        void Start()
        {
            prc = FindObjectOfType<PlayerRobotContral>();
        }
        public abstract void UpdateLogic();
        public abstract void Attack(Vector2 v2);
        public abstract void Move(Vector2 target,float speed);
        /// <summary>
        /// 专门为冲撞设置
        /// </summary>
        [HideInInspector]
        public bool isFly = false;

        public IEnumerator WaiteForShootCD()
        {
            IsShootCD = true;
            yield return new WaitForSeconds(ShootCD);
            IsShootCD = false;
        }
        public IEnumerator WaiteForMoveCD(Vector2 target, float speed)
        {
            IsMoveCD = true;

            // DateTime dt = DateTime.Now;
            while (Vector2.Distance(EC.transform.position, target) > 1)
            {
                if (!EC.Contral || isFly == true)
                    break;
                //Debug.Log("ai 目标" + target);
                EC.transform.position = Vector3.MoveTowards(EC.transform.position, target, speed * Time.deltaTime);
                //if ((DateTime.Now - dt).Seconds > 5)
                //    break;
                yield return 0;
            }
            //Debug.Log("enemy move over");
            yield return new WaitForSeconds(MoveCD);
            IsMoveCD = false;
        }
    }
  

   
}
