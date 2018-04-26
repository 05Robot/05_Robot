using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Script;


/*********************************************************************
****	作者 冰块药丸
****	时间 18/4/15
****	描述 核心子类
**********************************************************************/
namespace Assets.Script.Nomono
{
 
    public class FireCore : BaseCore
    {
        //测试数据
        public static FireCore fc = new FireCore(100,100,10,10,1,15);

        public FireCore(int bsp, int bmp, int ihp, int imp, uint level, uint cp): base( bsp,  bmp,  ihp,  imp,  level,  cp,CoreElement.Fire)
        {
           
        }
        public override void CoreAttack()
        {

        }
    }
}
