using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*********************************************************************
****	作者 冰块药丸 
****	时间 18/4/14
****	描述 玩家机器人
**********************************************************************/
namespace Assets.Script.Nomono
{

    public class PlayerRobot:BaseRobot
    {

        public BaseCore Core { get;set; }
        /// <summary>
        /// 对应玩家控制器的实例
        /// </summary>
        public PlayerRobotContral PRC;

        public float SpecialSpeed=40;

        public  PlayerRobot(BaseCore core,float moveSpeed)
        {
            Core = core;
            MoveSpeed = moveSpeed;
            CurrentHp = MaxHp = core.GetMaxHp();
            CurrentMp = MaxMp = core.GetMaxMp();
            core.coreStatu=BaseCore.CoreStatu.Normal;

            
        }

        public override void GetDamage(int damage)
        {
            base.GetDamage(damage);
            if (Core.coreStatu != BaseCore.CoreStatu.Injured && CurrentHp > 0.25*MaxHp && CurrentHp <= 0.5*MaxHp)
            {
               
                Core.coreStatu = BaseCore.CoreStatu.Injured;
                //todo 受伤debuff
            }
            else if (Core.coreStatu != BaseCore.CoreStatu.WillDead && CurrentHp > 0 && CurrentHp <= 0.25*MaxHp)
            {
                Core.coreStatu = BaseCore.CoreStatu.WillDead;
            }
        }

        public override void Dead()
        {
           GameManager.Instance.ChangeGameStatu(GameManager.GameStatu.GameOver);
        }

        public override void Critical()
        {
            //todo 击退效果
            
        }
    }
}
