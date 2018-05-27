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


        public abstract void UpdateFixed();
        public abstract void Attack(Vector2 v2);
        public abstract void Move(Vector2 target);
    }
  
   
}
