using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Assets.Script.Nomono;
using UnityEngine;
using Timer = System.Timers.Timer;


/*********************************************************************
****	作者
****	时间
****	描述 敌人控制器
**********************************************************************/
namespace Assets.Script.Mono
{
    public class EnemyContral:MonoBehaviorBase
    {
    
     
        public EnemyRobot ER;
        public SampleAI EAI;
   
        /// <summary>
        /// 是否可以控制
        /// </summary>
        /// 
        private bool IsContraled = true;

        /// <summary>
        /// 检查是否是射击cd
        /// </summary>
      


        void Awake()
        {
           
        }
        // Use this for initialization
        void Start()
        {
       
            EAI.EC = this;
        }

        // Update is called once per frame
         void FixedUpdate()
        {
            
            if (IsContraled)
            {
                EAI.Update();
                
            }
        }

        public void GetDamage(int mpdamage, int hpdamage)
        {
            ER.GetDamage(mpdamage, hpdamage);
        }




    }
}
