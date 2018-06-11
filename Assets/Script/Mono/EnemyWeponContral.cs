using System.Collections;
using System.Collections.Generic;
using Assets.Script.Mono;
using Assets.Script.Nomono;
using UnityEngine;

public class EnemyWeponContral : MonoBehaviour
{
    private EnemyContral EC;
    private SpriteRenderer SR;
    private PlayerRobotContral PRC;

    public bool isFilpX = false;
	// Use this for initialization
	void Start ()
	{
	    EC = transform.parent.GetComponent<EnemyContral>();
	    PRC = FindObjectOfType<PlayerRobotContral>();
	    SR = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	    if (EC.transform.position.x>PRC.transform.position.x)
	    {
	        if (transform.localPosition.x>0)
	        {
	            transform.localPosition=new Vector2(-1*transform.localPosition.x,transform.localPosition.y);
	            SR.flipY = true;
	            if (isFilpX)
	            {
	                SR.flipX = true;
                }
	        }

	    }
	    else if (EC.transform.position.x < PRC.transform.position.x)
	    {
	        if (transform.localPosition.x < 0)
	        {
	            transform.localPosition = new Vector2(-1 * transform.localPosition.x, transform.localPosition.y);
	            SR.flipY = false;
	            if (isFilpX)
	            {
	                SR.flipX = false;
	            }
            }
        }

	  //  transform.rotation=transform.rotation.LookTo2D(transform.position,PRC.transform.position);

	}

   public void Attack()
    {
        if (PRC.transform.position.x<EC.transform.position.x)
        {
        
            GetComponent<Animator>().Play("EnemyWeapon_left");
            
        }
        else
        {
            GetComponent<Animator>().Play("EnemyWeapon_Right");
    
        }
    }
}
