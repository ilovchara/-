using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DuckController : MonoBehaviour
{
    public new Animation animation;
    //生成碰撞
    public PolygonCollider2D polygonCollider;
    //层级
    public SortingGroup sortingGroup;
    //是否需要显示靶子
    public Transform duckTargetIcon;
    //渲染器
    public SpriteRenderer duckSpriteRenderer;
    //游戏鸭
    private bool isGameDuck;
    //鸭子之后的东西
    public bool isDie = false;


    private List<Vector2> physicsShape = new List<Vector2>();


    private void Init(DuckConfig duckConfig)
    {
        isDie = false;
        duckSpriteRenderer.sprite = duckConfig.sprite;        
        duckSpriteRenderer.transform.localRotation = Quaternion.identity;
        //不管需不需要菜单？
        SetColliderShape(duckSpriteRenderer.sprite);
    }
    
    //碰撞边缘 *
    private void SetColliderShape(Sprite sprite)
    {
        //区分精灵形状
        sprite.GetPhysicsShape(0, physicsShape);
        polygonCollider.SetPath(0,physicsShape);
    }
    //鸭子死亡
    public void Die()
    {
        isDie=true;
        animation.Play("Hit");

        //关闭所有协程
        StopAllCoroutines();
        //调用协程
        StartCoroutine(DoExit());
    }

    private IEnumerator DoExit()
    {
        AudioManager.Instance.PlayDuckGobackClip();
        float targetPosY = transform.position.y - 7;
        while(transform.position.y > targetPosY)
        {
            transform.position += new Vector3(0, -Time.deltaTime * ConfigManager.Instance.duckMoveSpeed, 0);
            yield return null;
        }
        DuckManager.Instance.RecycleDuck(this);
    }

    #region Menu

    //初始化 - 什么鸭子 + 什么坐标
    public void InitMenuDuck(DuckConfig config , Vector2 targetPos)
    {
        //专属音效
        AudioManager.Instance.PlayMemuDuckClip();
        isGameDuck = false;
        isTargetDuck = false; //刚开始搞错了 - 这个应该是菜单鸭才对
        //初始化
        Init(config);
        transform.position = targetPos + new Vector2(0, -5f); //先降低到地下，用协程回归
        //影长
        duckTargetIcon.gameObject.SetActive(false);
        //layer
        
        sortingGroup.sortingLayerName = "Duck2"; //这里我犯了错误 -层级的命名没有对应
        StartCoroutine(MoveToMenuPosition(targetPos.y));
    }

    //协程
    private IEnumerator MoveToMenuPosition(float targetPosY)
    {
        while(transform.position.y<targetPosY)
        {
            transform.position += new Vector3(0, Time.deltaTime*ConfigManager.Instance.duckMoveSpeed, 0);
            yield return null;

        }
        AudioManager.Instance.PlayMemuDuckReadyClip();
        transform.position = new Vector3 (transform.position.x,targetPosY,0);

    }


    #endregion


    #region game

    public bool isTargetDuck = false;
    private Vector2 spawnPoint;
    //判断那边移动
    private bool isLeft;
    private bool isUp;
    //给两个属性
    private float targetPosX;
    private float targetPosY;
    #region 属性
    public float TargetPosX
    {
        get => TargetPosX;
        set
        {
            targetPosX = value;
            //往左走
            isLeft = transform.position.x > targetPosX;
            transform.localScale = new Vector3(isLeft ? -1 : 1, 1, 1);
        }
    }

    public float TargetPosY
    {
        get => TargetPosY;
        set
        {
            targetPosY = value;
            isUp = transform.position.y < targetPosY;
        }
    }
    #endregion

    //初始化鸭子*
    public void InitGameDuck(DuckSpawnInfo spawnInfo)
    {
        //特殊鸭子
        if (spawnInfo.isTargetDuck)
        {
            AudioManager.Instance.PlayMemuDuckReadyClip();
        }
        //初始化
        Init(spawnInfo.config);
        spawnPoint = spawnInfo.spawnPoint;
        //游戏鸭
        isGameDuck = true;
        isTargetDuck = spawnInfo.isTargetDuck; //获取信息
        duckTargetIcon.gameObject.SetActive(isTargetDuck);
        //初始化数组
        sortingGroup.sortingLayerName = spawnInfo.layer.sortingLayer;
        transform.position = spawnInfo.spawnPoint;

        //错误是少用了属性
        TargetPosX = ConfigManager.Instance.GetRandomMovePointX();
        //初始向上
        TargetPosY = ConfigManager.Instance.GetRandomMovePointY(true) + spawnPoint.y;

    }

    private void Update()
    {
        if (!isGameDuck || isDie) return;
        Move();  
    }

    private void Move()
    {
        //往左走 但是 这里超出范围了
        if(isLeft && transform.position.x <= targetPosX)
        {
            TargetPosX = ConfigManager.Instance.GetRandomMovePointX();
        }
        //往右走 但是 这个
        else if(!isLeft && transform.position.x >= targetPosX)
        {
            TargetPosX = ConfigManager.Instance.GetRandomMovePointX();
        }

        if (isUp && transform.position.y >= targetPosY)
        {
            TargetPosY = ConfigManager.Instance.GetRandomMovePointY(!isUp) + spawnPoint.y;

        }
        else if (!isUp && transform.position.y <= targetPosY)
        {
            TargetPosY = ConfigManager.Instance.GetRandomMovePointY(!isUp) + spawnPoint.y;
        }


        Vector2 dir = Vector2.zero;
        dir.x = isLeft ? -1 : 1;
        dir.y = isUp ? 1 : -1; //这里偏移量标记错误
        transform.Translate(dir.normalized * ConfigManager.Instance.duckMoveSpeed*Time.deltaTime);

    }



    #endregion  
}
