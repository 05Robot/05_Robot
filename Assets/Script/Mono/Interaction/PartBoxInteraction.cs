using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartBoxInteraction : HitCheckBase
{

    [Header("零件预设")]
    [SerializeField]private GameObject _PartPrefab1;
    [SerializeField] private int min1, max1;
    [SerializeField]private GameObject _PartPrefab2;
    [SerializeField] private int min2, max2;

    /// <summary>
    /// 碎开
    /// </summary>
    public override void Broken()
    {
        int nums1 = Random.Range(min1, max1);
        for (int i = 0; i < nums1; i++)
        {
            float y = Mathf.Abs(Random.insideUnitCircle.y);
            Vector3 pos1 = new Vector3(transform.position.x + Random.insideUnitCircle.x * 5,
                transform.position.y - y * 2, transform.position.z);
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            GameObject newPart = ObjectPool.Instance.Spawn(_PartPrefab1.name);
            newPart.transform.position = transform.position;
            newPart.transform.rotation = rotation;
            newPart.GetComponent<Part>().StartPart(pos1);
        }

        int nums2 = Random.Range(min2, max2);
        for (int i = 0; i < nums2; i++)
        {
            float y = Mathf.Abs(Random.insideUnitCircle.y);
            Vector3 pos2 = new Vector3(transform.position.x + Random.insideUnitCircle.x * 5,
                transform.position.y - y * 2, transform.position.z);
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            GameObject newPart = ObjectPool.Instance.Spawn(_PartPrefab2.name);
            newPart.transform.position = transform.position;
            newPart.transform.rotation = rotation;
            newPart.GetComponent<Part>().StartPart(pos2);

        }
        Destroy(gameObject);
    }


}
