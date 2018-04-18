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
       const float DAMAGE_MAXHP_PRE_SECOND = 0.05f;
       protected Timer timer;
       protected AbnormalState(string stataName, int restTime)
       {
           StataName = stataName;
           this.restTime = restTime;
            //todo 可能存在问题
            timer = new Timer(1000);
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Elapsed += TimeEvent;
            timer.Start();
        }

       public string StataName;
       private int restTime;
       public abstract void AddStataEffect();
       public abstract void RemoveStataEffect();
       protected abstract void TimeEvent(object source, ElapsedEventArgs e);
        public void ResetTime(int time)
       {
           restTime = time;
       }

     
    }

    public class AS_Burn : AbnormalState
    {
        public AS_Burn( int restTime) : base("燃烧", restTime)
        {

        }

        public override void AddStataEffect()
        {
          
        }

     
      
        public override void RemoveStataEffect()
        {
           
        }

        protected override void TimeEvent(object source, ElapsedEventArgs e)
        {
           
        }
    }


   
}
