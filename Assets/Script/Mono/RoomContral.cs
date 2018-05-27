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
    private AudioSource AS_BGMPlayer;
    List<EnemyContral> EnemyList=new List<EnemyContral>();
    /// <summary>
    /// 房间是否被清理了
    /// </summary>
    public bool RoomHasClear=false;

    public bool IsBattle = false;
    // Use this for initialization
    void Start ()
    {
        BC2D = GetComponent<BoxCollider2D>();
        AS_BGMPlayer = GetComponent<AudioSource>();
        BC2D.isTrigger = true;

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerExit2D(Collider2D c2d)
    {
        EnemyContral EC = c2d.GetComponent<EnemyContral>();
        if (EC!=null)
        {
             EC.GetBackToYourPosition();
        }
    }

    public void BattleStart()
    {
        IsBattle = true;
        //todo 播放音乐
        foreach (var ec in EnemyList)
        {
            ec.AiStart = true;
        }
    }
}
