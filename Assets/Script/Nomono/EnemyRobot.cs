using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        public string Name { get; set; }
        public int ShootCD { get; set; }
        public int Damage { get; set; }
        public float ButtleSpeed { get; set; }


        public EnemyRobot(string name,int hp,int mp,float speed)
        {
            CurrentHp = MaxHp = hp;
            CurrentMp = MaxMp = mp;
            MoveSpeed = speed;
        }

       

        

    }
}
