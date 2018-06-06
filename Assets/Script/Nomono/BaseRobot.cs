using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Assets.Script.Nomono;
using JetBrains.Annotations;
using UnityEngine;


/*********************************************************************
****	作者 冰块药丸
****	时间 2018/4/14
****	描述 机器人基类
**********************************************************************/
namespace Assets.Script
{
    
    public abstract class BaseRobot
    {



        [HideInInspector]
        public int MaxHp;
        [HideInInspector]
        public int MaxMp;
        [HideInInspector]
        public int CurrentHp;
        [HideInInspector]
        public int CurrentMp;
        [HideInInspector]
        public int DelayCoefficient=3;
        [HideInInspector]
        public Action SecondAction;
        [HideInInspector]
        public float MoveSpeed;
        /// <summary>
        /// 判断是否是在消耗核心值
        /// </summary>
        [HideInInspector]
        public bool IsUseCore = false;
        /// <summary>
        /// 判断是否是在使用mp，为mp回复做准备
        /// </summary>
        [HideInInspector]
        public bool IsConsumeMp = false;
      
        /// <summary>
        /// 单位异常状态集合
        /// </summary>
        public List<AbnormalState> AbnormalStateList=new List<AbnormalState>(); 

     
        public  int MPRecoverInterval = 1;//1s
      

        public BaseRobot()
        {



        
         


        }
        /// <summary>
        /// 如果受到伤害
        /// </summary>
        /// <param name="damage"></param>
        public virtual void GetDamage(int MpDamage,int HpDamage )
        {
            if (!IsUseCore)
            {
                if (CurrentMp - MpDamage > 0)
                {
                    CurrentMp -= MpDamage;
                }
                else
                {
                    CurrentMp = 0;
                    IsUseCore = true;
                    Critical();
                }
                IsConsumeMp = true;
                //todo 调用控制器重置 
            }
            else
            {
                CurrentHp -= HpDamage;
                if (CurrentHp <= 0)
                    Dead();
            }



        }
      
        public virtual void GetReallyDamage(int damage)
        {
            CurrentHp -= damage;
            if (damage <= 0)
                Dead();
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
        public virtual void RecoverMp()
        {
            //可能出现maxmp未定义
            if (!IsConsumeMp)
            {
              
                CurrentMp += (int) (0.05*MaxMp);

                if (CurrentMp>MaxMp)
                    CurrentMp = MaxMp;
                
              //  Debug.Log("mp: " + CurrentMp);
            }
            else IsConsumeMp = false;
        }

        public IEnumerator SecondEvent()
        {
            while (true)
            {
                if (SecondAction!=null)
                {
                    SecondAction();
                }
               
                yield return new WaitForSeconds(1);
            }
        }

        public void AddAbnormalState(AbnormalState AS)
        {
            AbnormalStateList.Add(AS);
            SecondAction += AS.StatuSecondEvent;
        }



    }
}
