using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Assets.Script.Nomono;
using Chronos;
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
    public class EnemyContral : MonoBehaviour
    {

        public string Name;
        public int MaxHp = 1000;
        public int MaxMp = 1000;
        public float MoveSpeed = 3;
        public EnemyRobot ER;
        public EnemyAi EAI;
        [Rename("左面图")]
        public Sprite LeftSprite;
        [Rename("右面图")]
        public Sprite RightSprite;
        private PlayerRobotContral prc;
        /// <summary>
        /// 是否可以控制
        /// </summary>
        /// 
        private bool _isContral = true;
        [HideInInspector]
        public bool IsDead = false;
        private Vector2 Startpoint;
        [HideInInspector]
        public bool Contral
        {
            get { return _isContral; }
            set { _isContral = value; }
        }
        [HideInInspector]
        public bool AiStart = false;
        public float SecondsToGoBack = 5;


        private SpriteRenderer SR;
        void Awake()
        {
            ER = new EnemyRobot(name, MaxHp, MaxMp, MoveSpeed);
            ER.EC = this;
        }
        // Use this for initialization
        void Start()
        {
            EAI = GetComponent<EnemyAi>();
            SR = GetComponent<SpriteRenderer>();
            EAI.EC = this;
            Startpoint = transform.position;
            AiStart = true;

            prc = GameObject.FindObjectOfType<PlayerRobotContral>();
        }

        // Update is called once per frame
        void Update()
        {
            if (AiStart)
            {
                if (Contral)
                {
                    if (prc.transform.position.x < transform.position.x)
                        SR.sprite = LeftSprite;
                    else
                        SR.sprite = RightSprite;

                    EAI.UpdateLogic();

                }
            }
        }

        public void GetDamage(int mpdamage, int hpdamage)
        {
            ER.GetDamage(mpdamage, hpdamage);
            Debug.Log("EnemyDamega");
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
            EAI.Move(Startpoint, ER.MoveSpeed);

        }

        IEnumerator BeOutOfContral(float time)
        {
            Contral = false;
            yield return new WaitForSeconds(time);
            Contral = true;
        }
        public void Dead()
        {
            GetComponent<BoxCollider2D>().enabled = false;
            //播放死亡动画
            GetComponent<Animator>().Play("Dead");
            AiStart = false;
            Contral = false;
            IsDead = true;
            transform.parent.parent.GetComponent<RoomContral>().CurrentLiveCount--;
            //敌人死亡，核心爆炸
            DeadCoreExplode();
        }


        /// <summary>
        /// 敌人死亡，核心爆炸
        /// 2018.6.10 ZMK添加
        /// </summary>
        private void DeadCoreExplode()
        {
            if (!GameManager.Instance.PRC._mPlayerRobot.IsUseCore) return;
            GameObject CoreExplode = null;
            switch (GameManager.Instance.PRC._mPlayerRobot.Core.Element)
            {
                case BaseCore.CoreElement.Primary:
                    CoreExplode = ObjectPool.Instance.Spawn("1.NormalCoreImpact");
                    break;
                case BaseCore.CoreElement.Fire:
                    CoreExplode = ObjectPool.Instance.Spawn("1.FireCoreImpact");
                    break;
                case BaseCore.CoreElement.Amethyst:
                    CoreExplode = ObjectPool.Instance.Spawn("1.AmethystCoreImpact");
                    break;
                case BaseCore.CoreElement.Ice:
                    CoreExplode = ObjectPool.Instance.Spawn("1.ForzenCoreImpact");
                    break;
            }
            CoreExplode.transform.position = transform.position;
        }

        public void ClearWeapon()
        {
            GetComponentInChildren<EnemyWeponContral>().gameObject.SetActive(false);
        }

        #region 接口
        void StartAi(bool isStart)
        {
            AiStart = isStart;
        }
        /// <summary>
        /// 真实伤害
        /// </summary>
        /// <param name="hpdamage"></param>
        public void GetRealDamage(int hpdamage)
        {
            ER.GetReallyDamage(hpdamage);
        }
        /// <summary>
        /// 设置硬直
        /// </summary>
        public void SetDelay(float seconds, int delayconfficient)
        {
            if (delayconfficient > ER.DelayCoefficient)
            {
                StartCoroutine(BeOutOfContral(seconds));
            }
        }
        /// <summary>
        /// 设置击退
        /// </summary>
        /// <param name="distance"></param>
        public void SetKnockback(Vector2 diriction, float distance, int delayconfficient)
        {
            if (delayconfficient > ER.DelayCoefficient)
            {
                transform.Translate(((Vector2)transform.position - diriction).normalized * distance, Space.World);
            }

        }

        #endregion

        public Timeline Time
        {
            get { return GetComponent<Timeline>(); }
        }
    }
}
