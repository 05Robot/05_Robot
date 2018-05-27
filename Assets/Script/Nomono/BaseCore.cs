
using System;
using System.Collections.Generic;

namespace Assets.Script
{
    /*********************************************************************
    ****	作者 冰块药丸 
    ****	时间 2018/4/14
    ****	描述 核心基类
    **********************************************************************/

    public abstract class BaseCore
    {
       
        public enum CoreElement
        {
            Primary,
            Fire,
            Amethyst,
            Ice

        }
        public BaseCore(int tp,int maxhp,int minhp,string name,CoreElement ce,string discription)
        {
            if (minhp>maxhp||maxhp>tp)
            {
                throw new Exception("核心初始化数据异常");
            }
           
            TotalPoint = tp;
            MaxHp = maxhp;
            MinHp = minhp;
            Discription = discription;
            CurrentHpPoint = (MaxHp + MinHp) / 2;
            Name = name;
            Element = ce;
            BaseCore.CoreList.Add(this);
        }
        /// <summary>
        /// 处理玩家已经获得的核心
        /// </summary>
        public static List<BaseCore> CoreList=new List<BaseCore>();



        public readonly CoreElement Element;
        public readonly string Name;
        public readonly string Discription;
        public readonly int TotalPoint;
        protected readonly int MaxHp;
        protected readonly int MinHp;
        public int CurrentHpPoint;




        public bool SetHpPoint(int hppoint)
        {
            if (hppoint <= MaxHp && hppoint >= MinHp)
            {
                CurrentHpPoint = hppoint;
                return true; 
                
            }
            else
                return false;
        }

        
       

      

    }
}
