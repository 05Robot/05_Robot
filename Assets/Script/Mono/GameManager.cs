using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


/*********************************************************************
****	作者 冰块药丸
****	时间 18/4/15
****	描述 整个游戏状态的管理者，游戏加载的控制者
**********************************************************************/
namespace Assets.Script
{
    class GameManager:Singleton<GameManager>
    {
        public enum GameStatu
        {
            Pause,
            Normal,
            GameOver

        }

        private GameStatu CurrentStatu;
        /// <summary>
        /// 游戏数据重启
        /// </summary>
        public void GameReStart()
        {
            CurrentStatu = GameStatu.Normal;
        }

        public void ChangeGameStatu(GameStatu gs)
        {
            if (gs!= CurrentStatu)
            {
                switch (gs)
                {
                    case GameStatu.Pause:
                        break;
                    case GameStatu.Normal:
                        break;
                    case GameStatu.GameOver:
                        FindObjectOfType<PlayerRobotContral>().SetContral(false);
                        //todo 显示gameover ui
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("gs", gs, null);
                }
            }
        }


    }
}
