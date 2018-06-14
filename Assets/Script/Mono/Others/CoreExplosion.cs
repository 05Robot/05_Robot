using System.Collections;
using System.Collections.Generic;
using Assets.Script.Mono;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
/// <summary>
/// 只需对象池提取即可，无需再回收（自动回收）
/// </summary>
public class CoreExplosion : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;//检测层
    [SerializeField] private CoreAttribute m_CoreAttribute;
    private void OnEnable()
    {
        StartCoroutine(UnSpawn());

        //不同的核心产生不同的效果
        switch (m_CoreAttribute)
        {
            case CoreAttribute.Initial:
                //3个范围内200伤害
                EnemyDamage(CurrentAoeCollider2D(3), 200);
                break;
            case CoreAttribute.Fire:
                //3个范围内200伤害
                EnemyDamage(CurrentAoeCollider2D(3), 200);
                break;
            case CoreAttribute.Amethyst:
                //5个范围内400伤害
                EnemyDamage(CurrentAoeCollider2D(5), 400);
                break;
            case CoreAttribute.Frozen:
                //3个范围内200伤害
                EnemyDamage(CurrentAoeCollider2D(3), 200);
                break;
        }
    }
    private IEnumerator  UnSpawn()
    {
        yield return new WaitForSeconds(1.50f);
        ObjectPool.Instance.Unspawn(gameObject);
    }



    /// <summary>
    /// 敌人伤害
    /// </summary>
    private void EnemyDamage(Collider2D[] AllCollider2D, int DamageNums)
    {
        for (int i = 0; i < AllCollider2D.Length; i++)
        {
            EnemyContral hitEnemyContral = null;
            switch (AllCollider2D[i].transform.gameObject.layer)
            {
                //敌人护盾
                case 18:
                    hitEnemyContral = AllCollider2D[i].transform.GetComponent<ShieldProtect>().GetEnemyControl();
                    break;
                //敌人内部
                case 11:
                    hitEnemyContral = AllCollider2D[i].transform.GetComponent<EnemyContral>();
                    break;
                //紫水晶与零件箱
                case 19:
                case 20:
                    AllCollider2D[i].transform.GetComponent<HitCheckBase>().Broken();
                    break;
            }

            if (hitEnemyContral != null)
            {
                hitEnemyContral.GetDamage(DamageNums, DamageNums);
                //硬直
                hitEnemyContral.SetDelay(2, 4);
                //击退
                hitEnemyContral.SetKnockback(transform.position, 0.5f, 4);
            }
        }

        var shakePreset = ProCamera2DShake.Instance.ShakePresets[2];
        ProCamera2DShake.Instance.Shake(shakePreset);
    }
    /// <summary>
    /// 圆型AOE检测
    /// </summary>
    /// <param name="Range">检测范围</param>
    /// <returns>检测结果</returns>
    private Collider2D[] CurrentAoeCollider2D(int Range)
    {
        return Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), Range, layerMask);//Physics.OverlapSphere()：球形范围内的碰撞器
    }
}
