using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DuckController : MonoBehaviour
{
    public new Animation animation;
    //������ײ
    public PolygonCollider2D polygonCollider;
    //�㼶
    public SortingGroup sortingGroup;
    //�Ƿ���Ҫ��ʾ����
    public Transform duckTargetIcon;
    //��Ⱦ��
    public SpriteRenderer duckSpriteRenderer;
    //��ϷѼ
    private bool isGameDuck;
    //Ѽ��֮��Ķ���
    public bool isDie = false;


    private List<Vector2> physicsShape = new List<Vector2>();


    private void Init(DuckConfig duckConfig)
    {
        isDie = false;
        duckSpriteRenderer.sprite = duckConfig.sprite;        
        duckSpriteRenderer.transform.localRotation = Quaternion.identity;
        //�����費��Ҫ�˵���
        SetColliderShape(duckSpriteRenderer.sprite);
    }
    
    //��ײ��Ե *
    private void SetColliderShape(Sprite sprite)
    {
        //���־�����״
        sprite.GetPhysicsShape(0, physicsShape);
        polygonCollider.SetPath(0,physicsShape);
    }
    //Ѽ������
    public void Die()
    {
        isDie=true;
        animation.Play("Hit");

        //�ر�����Э��
        StopAllCoroutines();
        //����Э��
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

    //��ʼ�� - ʲôѼ�� + ʲô����
    public void InitMenuDuck(DuckConfig config , Vector2 targetPos)
    {
        //ר����Ч
        AudioManager.Instance.PlayMemuDuckClip();
        isGameDuck = false;
        isTargetDuck = false; //�տ�ʼ����� - ���Ӧ���ǲ˵�Ѽ�Ŷ�
        //��ʼ��
        Init(config);
        transform.position = targetPos + new Vector2(0, -5f); //�Ƚ��͵����£���Э�̻ع�
        //Ӱ��
        duckTargetIcon.gameObject.SetActive(false);
        //layer
        
        sortingGroup.sortingLayerName = "Duck2"; //�����ҷ��˴��� -�㼶������û�ж�Ӧ
        StartCoroutine(MoveToMenuPosition(targetPos.y));
    }

    //Э��
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
    //�ж��Ǳ��ƶ�
    private bool isLeft;
    private bool isUp;
    //����������
    private float targetPosX;
    private float targetPosY;
    #region ����
    public float TargetPosX
    {
        get => TargetPosX;
        set
        {
            targetPosX = value;
            //������
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

    //��ʼ��Ѽ��*
    public void InitGameDuck(DuckSpawnInfo spawnInfo)
    {
        //����Ѽ��
        if (spawnInfo.isTargetDuck)
        {
            AudioManager.Instance.PlayMemuDuckReadyClip();
        }
        //��ʼ��
        Init(spawnInfo.config);
        spawnPoint = spawnInfo.spawnPoint;
        //��ϷѼ
        isGameDuck = true;
        isTargetDuck = spawnInfo.isTargetDuck; //��ȡ��Ϣ
        duckTargetIcon.gameObject.SetActive(isTargetDuck);
        //��ʼ������
        sortingGroup.sortingLayerName = spawnInfo.layer.sortingLayer;
        transform.position = spawnInfo.spawnPoint;

        //����������������
        TargetPosX = ConfigManager.Instance.GetRandomMovePointX();
        //��ʼ����
        TargetPosY = ConfigManager.Instance.GetRandomMovePointY(true) + spawnPoint.y;

    }

    private void Update()
    {
        if (!isGameDuck || isDie) return;
        Move();  
    }

    private void Move()
    {
        //������ ���� ���ﳬ����Χ��
        if(isLeft && transform.position.x <= targetPosX)
        {
            TargetPosX = ConfigManager.Instance.GetRandomMovePointX();
        }
        //������ ���� ���
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
        dir.y = isUp ? 1 : -1; //����ƫ������Ǵ���
        transform.Translate(dir.normalized * ConfigManager.Instance.duckMoveSpeed*Time.deltaTime);

    }



    #endregion  
}
