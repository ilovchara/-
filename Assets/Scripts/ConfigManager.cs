using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    //����һ��������һ������
    public static ConfigManager Instance;
    //�㼶 - ���б�洢
    public List<DuckLayerConfig> duckLayerConfigs = new List<DuckLayerConfig>();
    //Ѽ�Ӿ���
    public List<DuckConfig> duckConfigList = new List<DuckConfig>();
    //�����ƶ���Χ
    public Vector2 horizaotalRange = new Vector2(-25f, 25f);
    //�洢���� - ��ס�ұ��ǹ��캯�� -��������Ҫ����ֵ�ĵط�
    public Vector2 menuDuckPosition = new Vector2(0, -5.52f);
    //��ƫ����
    public Vector3 gunOffset = new Vector3(0,6.28f);

    //Ѽ���ٶ�
    public float duckMoveSpeed = 8;
    public float readyGoAnimationTime = 2;
    public float maxGameTime = 60;
    //���ϵľ���
    public float duckUpDistance = 8;

    //����ģʽ
    public float superModeDuckInterval = 0.2f;
    public float DuckInterval = 1f;
    //���ɸ��� - �ö������߱�ʾ
    public AnimationCurve spawnCurve;

    //����ǳ��ָ��� - Ŀ��ѽ
    [Range(0f,1f)]
    public float targetDuckProbability = 0.4f;

    public float shootCD = 1f;
    public float superModeShootCD;


    private void Awake()
    {
        Instance = this;
    }
    //���Ѽ��
    public DuckConfig GetRandomDuckConfig()
    {
        //������ɣ�
        //1-4��Ѽ��*
        return duckConfigList[UnityEngine.Random.Range(0, duckConfigList.Count)];
    }
    //����㼶*
    public DuckLayerConfig GetRandomDuckLayerConfig()
    {
        return duckLayerConfigs[UnityEngine.Random.Range(0, duckLayerConfigs.Count)];
    }
    //���Ŀ��Ѽ*
    public bool GetRandomTargetDuck()
    {
        return UnityEngine.Random.Range(0,1f) < targetDuckProbability;
    }

    //����
    public float GetRandomMovePointX()
    {
        //-25f, 25f
        //������
        return UnityEngine.Random.Range(horizaotalRange.x,horizaotalRange.y);
    }
    //����
    public float GetRandomMovePointY(bool isUp) //Ҫ��Ĭ�ϵ�Layer������ *
    {
        //����Ҫϸ��
        if (isUp) return UnityEngine.Random.Range(duckUpDistance / 3, duckUpDistance);
        else return UnityEngine.Random.Range(0, duckUpDistance / 3);
    }
    //��ȡѼ��Ϣ *
    public DuckSpawnInfo GetRadnomDuckSpawnInfo()
    {
        //���Ѽ - �����
        DuckConfig duckConfig = GetRandomDuckConfig();
        DuckLayerConfig layerConfig = GetRandomDuckLayerConfig();
        //�ӷ�Ѽ
        bool isTarget = GetRandomTargetDuck();
        Vector2 point = new Vector2(GetRandomMovePointX(), layerConfig.poxY);
        //�Թ��캯��
        DuckSpawnInfo duckSpawnInfo = new DuckSpawnInfo(duckConfig,layerConfig,point,GetRandomTargetDuck());
        return duckSpawnInfo;
    }



}

//����Ǹ��ำֵ����
//���������� - ����Ѽ�ӵĲ㼶�����
[Serializable]
public class DuckConfig //���ֲ�ͬѼ�ӵ��� *
{    
    public Sprite sprite;
}


[Serializable]
public class DuckLayerConfig //���ֲ�ͬѼ�ӵĲ㼶 *
{    
    public string sortingLayer;
    public float poxY;
}





//Ѽ����Ϣ *
[Serializable]
public struct DuckSpawnInfo
{
    public DuckConfig config;
    public DuckLayerConfig layer;
    public Vector2 spawnPoint;
    public bool isTargetDuck;
    //���캯��
    public DuckSpawnInfo(DuckConfig config, DuckLayerConfig layer, Vector2 spawnPoint, bool isTargetDuck)
    {
        this.config = config;
        this.layer = layer;
        this.spawnPoint = spawnPoint;
        this.isTargetDuck = isTargetDuck;
    }
}