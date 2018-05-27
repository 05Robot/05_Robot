using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Nomono
{
  public  static class ExtentionFunction
    {
        /// <summary>
        /// 2D中以x右边为正，朝向某个方向看返回需要旋转的四元素
        /// </summary>
        /// <param name="quaternion"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Quaternion LookTo2D(this Quaternion quaternion,Vector2 from,Vector2 to)
        {
            
            Vector3 direction = (from - to).normalized;
            if (to.y >from.y)
            {
                return Quaternion.Euler(0, 0, -Vector2.Angle(Vector2.right, direction));
            }
            else
            {
                return Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, direction));
            }
        }
    }
}
