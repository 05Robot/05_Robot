using System.Collections;
using System.Collections.Generic;
using Assets.Script.Mono;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DangerRoomContral : MonoBehaviour
{
    private BoxCollider2D BC2D;
	// Use this for initialization
	void Start ()
	{
	    BC2D = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerExit2D(Collider2D c2d)
    {
        if (c2d.transform.gameObject.layer==10)
        {
            
            c2d.GetComponent<CapsuleCollider2D>().enabled = false;
            c2d.transform.Find("DropTrriger").gameObject.SetActive(true);

        } 
    }

    void OnTriggerEnter2D(Collider2D c2d)
    {

        if (c2d.transform.gameObject.layer == 10)
        {

            c2d.GetComponent<CapsuleCollider2D>().enabled = true;
            c2d.transform.Find("DropTrriger").gameObject.SetActive(false);

        }

    }
}
