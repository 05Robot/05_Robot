using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Assets.Script.Nomono;
using JetBrains.Annotations;
using Timer = System.Timers.Timer;


/*********************************************************************
****	作者 冰块药丸
****	时间 2018/4/14
****	描述 机器人基类
**********************************************************************/
namespace Assets.Script
{
    [Serializable]
    public abstract class BaseRobot
    {
        


        public int MaxHp;
        public int MaxMp;
        public int CurrentHp;
        public int CurrentMp;
        public float MoveSpeed;
        //普通机器人不需要关联核心
        /// <summary>
        /// 判断是否是在消耗核心值
        /// </summary>
        public bool IsUseCore = false;
        /// <summary>
        /// 判断是否是在使用mp，为mp回复做准备
        /// </summary>
        public bool IsConsumeMp = false;
        //todo 硬直系统
        public int MaxHardStraight;
        /// <summary>
        /// 单位异常状态集合
        /// </summary>
        public List<AbnormalState> AbnormalStateList=new List<AbnormalState>(); 

     
        private const Int32 MPRecoverInterval = 1000;//1s
        private Timer MPRecoverTimer=new Timer(MPRecoverInterval);

        public BaseRobot()
        {
          

            //todo 注意调试下
            MPRecoverTimer.Elapsed += RecoverMp;
            MPRecoverTimer.AutoReset = true;
            MPRecoverTimer.Enabled = true;
            MPRecoverTimer.Start();
         


        }
        /// <summary>
        /// 受到伤害
        /// </summary>
        /// <param name="damage"></param>
        public virtual void GetDamage(int damage)
        {
            if (!IsUseCore)//mp
            {
                if (CurrentMp - damage > 0)
                {
                    CurrentMp -= damage;
                }
                else
                {
                    CurrentMp = 0;
                    IsUseCore = true;
                    Critical();
                }
              //todo 调用控制器重置 
            }
            else//使用核心
            {
                CurrentHp -= damage;
                if (damage <= 0)
                    Dead();
                
            }
           
        }
        public virtual void Dead()
        {
           
        }
        /// <summary>
        /// 临界状态，当mp为0时触发的事件
        /// </summary>
        public virtual void Critical()
        {
            
        }

        /// <summary>
        /// 在control里面，做一个timer，到达指定时间执行这个函数
        /// </summary>
        private  void RecoverMp(object source, ElapsedEventArgs e)
        {
            //可能出现maxmp未定义
            if (!IsConsumeMp)
            {
                CurrentMp += (int) (0.05*MaxMp);
                if (IsUseCore)
                {
                    CurrentMp = MaxMp;
                }
            }
            else IsConsumeMp = false;
        }
        

       
    }
}
