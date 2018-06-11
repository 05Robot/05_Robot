using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmethystInteraction : HitCheckBase
{

    [Header("爆破图片")]
    [SerializeField]private Sprite _explodeSprite;
    private SpriteRenderer _AmethystSpriteRenderer;


    /// <summary>
    /// 碎开
    /// </summary>
    public override void Broken()
    {
        _AmethystSpriteRenderer = GetComponent<SpriteRenderer>();
        _AmethystSpriteRenderer.sprite = _explodeSprite;

        Destroy(GetComponent<Collider2D>());
    }


    /*//碰撞器参数
    private Vector2 Pos, Size;
    private float width, height;
    void Start()
    {
        Pos = new Vector2(transform.position.x, transform.position.y);
        width = ((BoxCollider2D)SelfCollider2D).size.x;
        height = ((BoxCollider2D)SelfCollider2D).size.y;
        Size = new Vector2(width, height);
        _AmethystSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Physics2D.OverlapBoxAll(Pos, Size, 0);
    }*/

}
