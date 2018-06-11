using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Script.Mono;
using Assets.Script.Nomono;
using Chronos;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HockContral : MonoBehaviour
{
    public float HockCD = 3f;
    public float Distance = 20f;
    public float Speed = 10f;
  
    public Vector2 StartPoint;
    public Vector2 Direction;
    public GameObject target = null;
    public float FreezLenth=2f;
    private SpriteRenderer SR;
    private BoxCollider2D BC2D;
    private PlayerRobotContral PRC;
    private EnemyContral EC;
    private bool isEnable = false;
    private bool isRetern = false;
    // Use this for initialization
    void Start()
    {
        PRC = FindObjectOfType<PlayerRobotContral>();
        SR = GetComponent<SpriteRenderer>();
        BC2D = GetComponent<BoxCollider2D>();
        SetEnable(false);
        

    }

    // Update is called once per frame
    void Update()
    {
        if (isEnable)
        {
            if (isRetern == false)
            {
               
                if (Vector2.Distance(transform.position, StartPoint) > Distance)
                {
                   
                    isRetern = true;
                    return;
                }
                transform.Translate(Direction * Speed * Time.deltaTime, Space.World);
            }
            else  
            {
                
                 if (target==null)
                {
                    if (Vector2.Distance(transform.position, StartPoint) < FreezLenth)//视为回到原点
                    {
                       
                    
                        isRetern = false;
                        SetEnable(false);

                    }
                    else
                    {
                        transform.Translate(-Direction * Speed * Time.deltaTime, Space.World);
                    }
                   
                }
                else if(target.layer==11)
                {
                    if (Vector2.Distance(transform.position, StartPoint) < FreezLenth)
                    {
                      
                        EC.Contral = true;
                        SetEnable(false);
                        isRetern = false;
                    }
                    else
                    {
                        transform.Translate(-Direction * Speed * Time.deltaTime, Space.World);
                        target.transform.Translate(-Direction * Speed * Time.deltaTime, Space.World);
                    }
                }
                else if(target.layer == 12)
                {
                    if (Vector2.Distance(PRC.transform.position,this.transform.position)< FreezLenth)
                    {
                       
                       
                        isRetern = false;
                        SetEnable(false);
                    }
                    else
                        PRC.transform.Translate(Direction * Speed * Time.deltaTime, Space.World);
                    
                }
                
            }
           

        }


    }

    public void SetEnable(bool enable)
    {
        SR.enabled = enable;
        BC2D.enabled = enable;
        isEnable = enable;

        if (enable==false)
        {
            EC = null;
            target = null;
        }


    }

    /// <summary>
    /// 传入目标方向
    /// </summary>
    /// <param name="v2"></param>
    public void StartHock(Vector2 v2)
    {
        target = null;
        PRC.Contral = false;
        //Debug.Log(PRC.Contral);
        transform.position = PRC.transform.position;
        StartPoint = transform.position;
        //if (v2.x< transform.position.x)
        //{
        //    Direction = ((Vector2)transform.position - v2).normalized;
        //}
        //else
        //{
        //    Direction = (  v2-(Vector2)transform.position).normalized;
        //}
        Direction = (v2-(Vector2)transform.position).normalized;
        transform.rotation = transform.rotation.LookTo2D(transform.position, v2);
        SetEnable(true);
        StartCoroutine(WaiteHock());
        Debug.Log(transform.position+"    "+v2);

    }

    void OnTriggerEnter2D(Collider2D c2d)
    {
        //敌人
        if (c2d.gameObject.layer==11)
        {
            target = c2d.gameObject;
            EC = target.GetComponent<EnemyContral>();
            EC.Contral = false;
            isRetern = true;
            BC2D.enabled = false;
            

        }
        //地形
        else if (c2d.gameObject.layer == 12)
        {

            target = c2d.gameObject;
           
            isRetern = true;
            BC2D.enabled = false;
        }
       
       
    }
   
    IEnumerator WaiteHock()
    {
        //todo attion
        yield return new WaitWhile( ()=>isEnable);
        PRC.Contral = true;
        
    }

    //public Timeline Time
    //{
    //    get { return GetComponent<Timeline>(); }
    //}

}
