using System.Collections;
using System.Collections.Generic;
using Assets.Script.Nomono;
using Chronos;
using UnityEngine;

public class FightAi : EnemyAi
{
    [Rename("攻击判定延迟")]
    public float AttakeDelay=0.5f;
    

    public override void UpdateLogic()
    {
        if (Mathf.Abs(Vector2.Distance(EC.transform.position, prc.transform.position)) <= AttentionDistence)
        {
            if (!IsShootCD)
            {
                Attack(prc.transform.position);
                EC.StartCoroutine(WaiteForShootCD());
            }
            if (!IsMoveCD)
            {
                
                //直接靠近玩家
                Vector2 target =prc.transform.position+ (transform.position - prc.transform.position).normalized*(ButtleFlyDistance - 1f);
               
                Move(target, EC.ER.MoveSpeed);
            }
        }
    }

    public override void Attack(Vector2 v2)
    {
        if (Vector2.Distance(prc.transform.position, transform.position) <= ButtleFlyDistance)
        {
            Debug.Log(1111);
             StartCoroutine(Attack());
        }
       
    }

    public override void Move(Vector2 target, float speed)
    {
        //射线检测是否有障碍物
        RaycastHit2D[] rh2d = Physics2D.RaycastAll(EC.transform.position, target, MoveDistance);
        //Debug.Log("开始移动");
        foreach (var rh in rh2d)
        {
            if (rh.transform.gameObject.layer == 10 || rh.transform.gameObject.layer == 12)
            {
                target = (Vector2)EC.transform.position + target.normalized * (Vector2.Distance(EC.transform.position, rh.point) - 1);
                EC.StartCoroutine(WaiteForMoveCD(target, speed));
                break;
            }
        }
        EC.StartCoroutine(WaiteForMoveCD(target, speed));

    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(AttakeDelay);
        if (Vector2.Distance(prc.transform.position, transform.position)<= ButtleFlyDistance)
            prc.GetDamage((int)ButtleDamage, (int)ButtleDamage);
        
       



    }

    public Timeline Time
    {
        get { return GetComponent<Timeline>(); }
    }
}
