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

        public float AttentionDistence=10;
        public uint ButtleDamage=100;
        public uint ButtleFlyDistance=50;
        public uint ButtleSpeed=30;
        public float ShootCD=3;
        public float MoveCD=5;
        public float MoveDistance=5;
        protected bool IsShootCD = false;
        protected bool IsMoveCD = false;


        public abstract void UpdateLogic();
        public abstract void Attack(Vector2 v2);
        public abstract void Move(Vector2 target,float speed);
        /// <summary>
        /// 专门为冲撞设置
        /// </summary>
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
                if (!EC.Contral && isFly == false)
                    break;
                Debug.Log("ai 目标" + target);
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
