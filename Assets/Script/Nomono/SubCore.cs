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

    public   class FireCore : BaseCore
    {

        public FireCore() : base(1600,1400,1000,CoreElement.Fire)
        {
          
        }
    }
    public class IceCore : BaseCore
    {

        public IceCore() : base(1400, 1000, 300, CoreElement.Ice)
        {

        }
    }
    public class PrimaryCore : BaseCore
    {

        public PrimaryCore() : base(2000, 1500, 500, CoreElement.Primary)
        {

        }
    }
    public class AmethystCore : BaseCore
    {

        public AmethystCore() : base(2300, 1800, 1300, CoreElement.Amethyst)
        {

        }
    }
}
