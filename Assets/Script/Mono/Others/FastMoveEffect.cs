using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastMoveEffect : MonoBehaviour {
    void OnEnable()
    {
        Invoke("UnSpawn",1.0f);
    }

    void UnSpawn()
    {
        ObjectPool.Instance.Unspawn(gameObject);
    }
}
