using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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

            SecondAction += RecoverMp;
            SyncHpMp();
            
        }

        public override void GetDamage(int MPDamage,int HPDmage)
        {
            base.GetDamage(MPDamage,HPDmage);
            //Debug.Log("hp:"+CurrentHp+"  mp:"+CurrentMp);
            if (Core.coreStatu != BaseCore.CoreStatu.Injured && CurrentHp > 0.25*MaxHp && CurrentHp <= 0.5*MaxHp)
            {
               
                Core.coreStatu = BaseCore.CoreStatu.Injured;
                //todo 受伤debuff
            }
            else if (Core.coreStatu != BaseCore.CoreStatu.WillDead && CurrentHp > 0 && CurrentHp <= 0.25*MaxHp)
            {
                Core.coreStatu = BaseCore.CoreStatu.WillDead;
            }
            SyncHpMp();
        }

        public override void Dead()
        {
           GameManager.Instance.ChangeGameStatu(GameManager.GameStatu.GameOver);
        }

        public override void Critical()
        {
            SecondAction -= RecoverMp;
            PRC.StartCoroutine(MPCD());
            //todo 击退效果

        }

        protected override void RecoverMp()
        {
            base.RecoverMp();
            SyncHpMp();
        }

        void SyncHpMp()
        {
            UiManager.Instance.SyncHPMp((float)CurrentHp/(float)MaxHp,(float)CurrentMp/(float)MaxMp);
        }

        IEnumerator MPCD()
        {
            Debug.Log("进入冷却");
            yield return new WaitForSeconds(10);
            SecondAction += RecoverMp;
            CurrentMp = MaxMp;
            SyncHpMp();
            IsUseCore = false;
        } 
    }
}
