using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using Chronos;
using UnityEngine;

public class Part : MonoBehaviour
{
    public Timeline Time
    {
        get { return GetComponent<Timeline>(); }
    }
    [Rename("代表的零件数目")] public int PartNums;
    private Vector3 DirectionVector3;
    //private bool MoveToPlayer;
    private bool m_MoveToDirection;

    /// <summary>
    /// 开启零件
    /// </summary>
    public void StartPart(Vector3 DirectionVector3)
    {
        this.DirectionVector3 = DirectionVector3;
        //MoveToPlayer = false;
        m_MoveToDirection = false;
        StartCoroutine(MoveToDirectionUp());
    }

    /// <summary>
    /// 向上移动
    /// </summary>
    IEnumerator MoveToDirectionUp()
    {
        Vector3 Direction = transform.position + Vector3.up;
        while (true)
        {
            yield return null;
            transform.position = transform.position + Vector3.up * Time.deltaTime * 5;
            if ((Vector3.Distance(transform.position, Direction) < 0.5f))
            {
                break;
            }
        }

        StartCoroutine(MoveToDirection(DirectionVector3, 1.0f));
    }

    /// <summary>
    /// 移动到指定位置
    /// </summary>
    IEnumerator MoveToDirection(Vector3 pos, float Speed)
    {
        Vector3 dir = (pos - transform.position).normalized;
        while (true)
        {
            yield return null;
            transform.Translate(dir * Time.deltaTime * Speed * 5, Space.World);
            if (Vector3.Distance(transform.position, pos) < 0.5f )
            {
                break;
            }
        }

        m_MoveToDirection = true;
    }

    /// <summary>
    /// 移动到玩家位置
    /// </summary>
    IEnumerator MoveToPlayDirection(GameObject player, float Speed)
    {
        while (true)
        {
            yield return null;
            Vector3 dir = (player.transform.position - transform.position).normalized;
            transform.Translate(dir * Time.deltaTime * Speed * 5, Space.World);
            //transform.Translate((player.transform.position - transform.position) * Time.deltaTime * Speed);
            if (Vector3.Distance(transform.position, player.transform.position) < 0.1f)
            {
                break;
            }
        }

        WeaponManager.Instance.PartNums += PartNums;
        ObjectPool.Instance.Unspawn(gameObject);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_MoveToDirection) return;
        if (other.gameObject.layer == 15)
        {
            StopAllCoroutines();
            //MoveToPlayer = true;
            StartCoroutine(MoveToPlayDirection(GameManager.Instance.PRC.gameObject, 2.5f));
        }
    }
}
