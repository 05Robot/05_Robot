using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


/*********************************************************************
****	作者 冰块药丸
****	时间 18/4/15
****	描述 核心子类
**********************************************************************/
namespace Assets.Script.Nomono
{
    public class FireCore : BaseCore
    {
        public FireCore(int bsp, int bmp, int ihp, int imp, uint level, uint cp): base( bsp,  bmp,  ihp,  imp,  level,  cp,CoreElement.Fire)
        {
           
        }
        public override void CoreAttack()
        {

        }
    }
}
