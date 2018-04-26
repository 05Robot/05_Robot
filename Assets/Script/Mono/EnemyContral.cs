using System;
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
        /// <summary>
        /// 子弹预设
        /// </summary>
        public GameObject Buttles;
     
        public EnemyRobot ER;
        public EnemyAi EAI;
        private bool IsShootCD = false;
        /// <summary>
        /// 是否可以控制
        /// </summary>
        private bool IsContraled = true;

        /// <summary>
        /// 检查是否是射击cd
        /// </summary>
        public bool CheckCd()
        {

          
                if (IsShootCD)//没有进入cd
                {
                    return false;
                }
                else
                {
                    //todo 可能计时器存在问题
                    Timer timer = new Timer(ER.ShootCD);
                    timer.Elapsed += SetCD;
                    timer.AutoReset = false;
                    timer.Enabled = true;

                    IsShootCD = true;

                    return true;
                }
         
        }

        public void SetCD(object o, ElapsedEventArgs e)
        {
            IsShootCD = false;
        }

        void Awake()
        {
           
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            base.Update();
            if (IsContraled)
            {
                EAI.Update();
                
            }
        }
    }
}
