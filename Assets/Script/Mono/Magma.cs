using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magma : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnTriggerExit2D(Collider2D c2d)
    {
        if (c2d.GetComponent<DropContral>() != null) 
        {
           
            c2d.transform.parent.GetComponent<PlayerRobotContral>()._mPlayerRobot.Dead();
           
        }
    }

    void OnTriggerEnter2D(Collider2D c2d)
    {

        if (c2d.transform.gameObject.layer == 10)
        {

           

        }

    }
}
