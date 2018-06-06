using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Mono;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]

public class RoomContral : MonoBehaviour
{
    private BoxCollider2D BC2D;
    private AudioSource AS;

    

    [Header("根据每波敌人的数量，按照前面开始排列")]
    [Rename("每波敌人的数量")]
    public List<int> EnemyBatch=new List<int>();
    [Rename("敌人的具体实例")]
    public List<EnemyContral> EnemyList=new List<EnemyContral>();
    
    /// <summary>
    /// 房间是否被清理了
    /// </summary>
    [HideInInspector]
    public bool IsClear = false;

    private bool IsBattle = false;
    private int CurrrentUnit=0;
    private int CurrrentBatch = 0;

    public int CurrentLiveCount = 0;
    // Use this for initialization
    void Awake()
    {
        BC2D = GetComponent<BoxCollider2D>();
        AS = GetComponent<AudioSource>();
    }
    void Start ()
    {
      
       
        foreach (var enemy in EnemyList)
        {
            enemy.gameObject.SetActive(false);
        }

    }
	
	// Update is called once per frame
	void Update () {
	    if (IsBattle)
	    {
	        if (CurrentLiveCount <= 0)
	        {
	            if (!GetEnemyBatch())
	            {
	                IsClear = true;
	                IsBattle = false;
	                CurrentLiveCount = 0;
	                EndBattle();
	            }

	        }
        }
	    
	}

    void OnTriggerExit2D(Collider2D c2d)
    {
        #region 敌人

        EnemyContral EC = c2d.GetComponent<EnemyContral>();
        if (EC != null)
        {
            EC.GetBackToYourPosition();
        }

        #endregion

    }

    void OnTriggerEnter2D(Collider2D c2d)
    {

        #region 玩家事件
        if (c2d.gameObject.layer == 10)
        {
            if (!IsClear)
            {
                StartBattle();
             
            }
        }
        #endregion


    }

    void StartBattle()
    {
        IsBattle = true;
        FindObjectOfType<PlayerRobotContral>().AS.Pause();
        AS.Play();
        GetEnemyBatch();


    }
    void EndBattle()
    {
        IsBattle = false;
        FindObjectOfType<PlayerRobotContral>().AS.UnPause();
        AS.Stop();

    }
    
    public bool GetEnemyBatch()
    {
        if (CurrrentBatch >= EnemyBatch.Count)
            return false;
        int i = CurrrentUnit;
        CurrentLiveCount = EnemyBatch[CurrrentBatch];
        for (; i < CurrrentUnit+EnemyBatch[CurrrentBatch]; i++)
        {
            EnemyList[i].gameObject.SetActive(true);
            EnemyList[i].AiStart = true;
        }
        //下一次
        CurrrentUnit = i + 1;
        CurrrentBatch++;
        return true;

    }
    //public void OnDrawGizmos()
    //{
    //    Gizmos.color=new Color(0.8f,1,0.8f);
    //    Gizmos.DrawCube((Vector2)transform.position+BC2D.offset,BC2D.size);
    //}
}


