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
    [RequireComponent(typeof(EnemyAi))]
    public class EnemyContral:MonoBehaviorBase
    {
        public string Name;
        public int MaxHp=1000;
        public int MaxMp=1000;
        public float MoveSpeed=3;
        public EnemyRobot ER;
        public EnemyAi EAI;
    
        /// <summary>
        /// 是否可以控制
        /// </summary>
        /// 
        private bool _isContral = true;

        private Vector2 Startpoint;
        [HideInInspector]
        public bool Contral
        {
            get { return _isContral; }
            set { _isContral = value; }
        }
        [HideInInspector]
        public bool AiStart = false;
        public float SecondsToGoBack=5;



        void Awake()
        {
            ER=new EnemyRobot(name,MaxHp,MaxMp,MoveSpeed);
            ER.EC = this;
        }
        // Use this for initialization
        void Start()
        {
            EAI = GetComponent<EnemyAi>();
            EAI.EC = this;
            Startpoint = transform.position;
            AiStart = true;
        }

        // Update is called once per frame
         void FixedUpdate()
        {
            if (AiStart)
            {
                if (Contral)
                {
                    EAI.UpdateFixed();

                }
            }
        }

        public void GetDamage(int mpdamage, int hpdamage)
        {
            ER.GetDamage(mpdamage, hpdamage);
        }
        /// <summary>
        /// ai走出房间时调用
        /// </summary>
        public void GetBackToYourPosition()
        {
            StartCoroutine(WaiteToGoBack());
        }

        IEnumerator WaiteToGoBack()
        {
            yield return new WaitForSeconds(SecondsToGoBack);
            EAI.Move(Startpoint);

        }

        public void Dead()
        {
            GetComponent<BoxCollider2D>().enabled = false;
            //播放死亡动画
            AiStart = false;
            Contral = false;
        }

        void StartAi()
        {
            AiStart = true;
        }

    }
}
