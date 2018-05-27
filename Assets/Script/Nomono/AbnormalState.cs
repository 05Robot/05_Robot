using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using UnityEngine.Video;


/*********************************************************************
****	作者
****	时间
****	描述 异常状态类及其子类
**********************************************************************/
namespace Assets.Script.Nomono
{
   public abstract class AbnormalState
   {

       protected AbnormalState(BaseRobot br,string stataName, int time)
       {
           BR = br;
           StataName = stataName;
           keep_time =time;
            //todo 可能存在问题
       
            
        }

       protected BaseRobot BR;
       public string StataName;
       protected int keep_time;

        /// <summary>
        /// 每秒调用事件
        /// </summary>
       public abstract void StatuSecondEvent();

       public void Romove()
       {
           BR.AbnormalStateList.Remove(this);
           BR.SecondAction -= StatuSecondEvent;
       }
     

     
    }

    public class AbnormalState_Burn : AbnormalState
    {
        private const float DAMAGE_MAXHP_PRE_SECOND = 0.05f;

        public AbnormalState_Burn(BaseRobot br,int time) : base( br,"燃烧",time)
        {
        }

        public override void StatuSecondEvent()
        {
            keep_time--;
           BR.GetDamage((int)DAMAGE_MAXHP_PRE_SECOND * BR.MaxHp, (int)DAMAGE_MAXHP_PRE_SECOND * BR.MaxHp);
            if (keep_time == 0)
                Romove();

        }
    }
    public class AbnormalState_Frozen : AbnormalState
    {
        private const float FROZEN_SPEED_PRASENT = 0.5f;
        private float temp_speed;
        public AbnormalState_Frozen(BaseRobot br, int time) : base(br, "冰冻", time)
        {
            temp_speed = BR.MoveSpeed;
            BR.MoveSpeed= FROZEN_SPEED_PRASENT* br.MoveSpeed;
        }

        public override void StatuSecondEvent()
        {
            keep_time--;
            if (keep_time == 0)
            {
                BR.MoveSpeed = temp_speed;
                Romove();

            }

        }
    }



}
