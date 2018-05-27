using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script.Mono;
using UnityEngine;


/*********************************************************************
****	作者 冰块药丸 
****	时间 18/4/18
****	描述 敌人机器人基类
**********************************************************************/
namespace Assets.Script.Nomono
{
    [Serializable]
   public class EnemyRobot:BaseRobot
    {


        public string Name;
        [HideInInspector]
        public EnemyContral EC;

        public override void Dead()
        {
            EC.Dead();

        }

        public override void Critical()
        {
            base.Critical();
        }

        public override void RecoverMp()
        {
            base.RecoverMp();
        }


        public EnemyRobot(string name,int hp,int mp,float speed)
        {
            Name = name;
            CurrentHp = MaxHp = hp;
            CurrentMp = MaxMp = mp;
            MoveSpeed = speed;
        }

       

        

    }
}
