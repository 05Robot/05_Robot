using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine;

public class DoorContral : MonoBehaviour
{
    public Func<bool> condition = null;
    public Sprite OpenDoor;
    private BoxCollider2D BC2D;
    private SpriteRenderer SR;
    private RoomContral RC;
    // Use this for initialization
    void Start()
    {
        BC2D = GetComponent<BoxCollider2D>();
        RC = transform.parent.parent.GetComponent<RoomContral>();
        SR = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {


    }

    void OnTriggerEnter2D(Collider2D c2d)
    {
        if (RC.IsClear&&BC2D.enabled)
        {
            if (condition != null&&condition()==false)
            {
                return;
            }

            BC2D.enabled = false;
            SR.sprite = OpenDoor;

        }

    }

    bool DownDoorOpen()
    {
        if (FindObjectOfType<PlayerRobotContral>().transform.position.y<transform.position.y)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool AmethystDoorOpen()
    {
        if (FindObjectOfType<PlayerRobotContral>()._mPlayerRobot.Core.Element==BaseCore.CoreElement.Amethyst)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
