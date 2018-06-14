using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMuzzle : MonoBehaviour
{
    public float[] muzzle_Y;
    public SpriteRenderer ThisGun;


    private Vector3 newPos;
    void Start()
    {
        newPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }
    void Update()
    {
        newPos.y = !ThisGun.flipY ? muzzle_Y[0] : muzzle_Y[1];
        transform.localPosition = newPos;
    }
}
