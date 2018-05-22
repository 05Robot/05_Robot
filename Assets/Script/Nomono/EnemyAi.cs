using System;
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
    public abstract class EnemyAi
    {
        public EnemyContral EC;
        public float AttentionDistence { get; set; }
        public uint ButtleDamage;
        public uint ButtleFlyDistance;
        public uint ButtleSpeed;
        public abstract void Update();
        public abstract void Attack(Vector2 v2);
    }

    public class SampleAI : EnemyAi
    {

        public override void Update()
        {
            //每帧判断玩家与本单位的距离，判断是否可以射击
            PlayerRobotContral prc = GameObject.FindObjectOfType<PlayerRobotContral>();
            if (Mathf.Abs(Vector2.Distance(EC.transform.position, prc.transform.position)) < AttentionDistence)
            {
                if (EC.CheckCd())//如果没有cd
                {
                    Attack(prc.transform.position);
                }


            }

        }

        public override void Attack(Vector2 v2)
        {
            GameObject buttle = ObjectPool.Instance.Spawn("AWM_Buttle");
            buttle.transform.Translate(EC.transform.position);
            buttle.transform.LookAt(v2);
            buttle.GetComponent<Bullet>().BulletStart(ButtleSpeed,ButtleFlyDistance ,ButtleDamage);

        }
    }
}
