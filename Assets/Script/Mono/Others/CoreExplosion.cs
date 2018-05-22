using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 只需对象池提取即可，无需再回收（自动回收）
/// </summary>
public class CoreExplosion : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(UnSpawn());
    }
    private IEnumerator  UnSpawn()
    {
        yield return new WaitForSeconds(1.50f);
        ObjectPool.Instance.Unspawn(gameObject);
    }
}
