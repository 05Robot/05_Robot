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
    [Serializable]
    public class PlayerRobot:BaseRobot
    {

        public BaseCore Core { get;set; }
        /// <summary>
        /// 对应玩家控制器的实例
        /// </summary>
        
        private PlayerRobotContral PRC;

        public float SpecialSpeed=40;

        public  PlayerRobot(PlayerRobotContral PRC,BaseCore core,float moveSpeed)
        {
            Core = core;
            MoveSpeed = moveSpeed;
            CurrentHp = MaxHp = core.CurrentHpPoint;
            CurrentMp = MaxMp = core.TotalPoint-core.CurrentHpPoint;
           

            SecondAction += RecoverMp;
            SyncHpMp();
            
        }

        public override void GetDamage(int MPDamage,int HPDmage)
        {
            base.GetDamage(MPDamage,HPDmage);
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
            PRC.SetDelay(0.2f);
          

        }

        public override void RecoverMp()
        {
            base.RecoverMp();
            SyncHpMp();
        }

       public void SyncHpMp()
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
