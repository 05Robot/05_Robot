
using System;

namespace Assets.Script
{
    /*********************************************************************
    ****	作者 冰块药丸 
    ****	时间 2018/4/14
    ****	描述 核心基类
    **********************************************************************/

    public abstract class BaseCore
    {
        //public BaseCore ()
        //{
        //    BaseHp = 100;
        //    BaseMp = 100;
        //    _incrementMp = 100;
        //    _incrementHp = 100;
        //    CoreLevel = 1;
        //    HpPoint = 2;
        //    MpPoint = 2;
        //}

        public BaseCore(int bsp,int bmp,int ihp,int imp,uint level,uint cp,CoreElement ce)
        {
            BaseHp = bsp;
            BaseMp = bmp;
            _incrementHp = ihp;
            _incrementMp = imp;
            CoreLevel = level;
            CorePoint = RemaindPoint = cp;
        }


        public enum CoreElement
        {
            Fire,
        }
        public enum CoreStatu
        {
           Normal,
           Injured,
           WillDead
        }

        public readonly CoreElement coreElement;
        public readonly int BaseHp;
        public readonly int BaseMp;
        private readonly int _incrementHp;
        private readonly int _incrementMp;

        public CoreStatu coreStatu=CoreStatu.Normal;
        public uint CoreLevel;
        public uint CorePoint;
        //todo 最多分配点和等级有关
        public uint HpPoint=0;
        public uint MpPoint=0;
        public uint WeaponPoint=0;
        public uint ItemPoint=0;
        public uint RemaindPoint;

        public int GetMaxHp()
        {
            return (int) (BaseHp + HpPoint*_incrementHp);
        }
        public int GetMaxMp()
        {
            return (int)(BaseMp + MpPoint * _incrementMp);
        }
        public virtual void CoreAttack()
        {
            
        }

    }
}
