using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    //单例一个类声明一个对象
    public static ConfigManager Instance;
    //层级 - 用列表存储
    public List<DuckLayerConfig> duckLayerConfigs = new List<DuckLayerConfig>();
    //鸭子精灵
    public List<DuckConfig> duckConfigList = new List<DuckConfig>();
    //横向移动范围
    public Vector2 horizaotalRange = new Vector2(-25f, 25f);
    //存储坐标 - 记住右边是构造函数 -才是你需要输入值的地方
    public Vector2 menuDuckPosition = new Vector2(0, -5.52f);
    //抢偏移量
    public Vector3 gunOffset = new Vector3(0,6.28f);

    //鸭子速度
    public float duckMoveSpeed = 8;
    public float readyGoAnimationTime = 2;
    public float maxGameTime = 60;
    //往上的距离
    public float duckUpDistance = 8;

    //超级模式
    public float superModeDuckInterval = 0.2f;
    public float DuckInterval = 1f;
    //生成概率 - 用动画曲线表示
    public AnimationCurve spawnCurve;

    //这个是出现概率 - 目标呀
    [Range(0f,1f)]
    public float targetDuckProbability = 0.4f;

    public float shootCD = 1f;
    public float superModeShootCD;


    private void Awake()
    {
        Instance = this;
    }
    //随机鸭子
    public DuckConfig GetRandomDuckConfig()
    {
        //随机生成？
        //1-4的鸭子*
        return duckConfigList[UnityEngine.Random.Range(0, duckConfigList.Count)];
    }
    //随机层级*
    public DuckLayerConfig GetRandomDuckLayerConfig()
    {
        return duckLayerConfigs[UnityEngine.Random.Range(0, duckLayerConfigs.Count)];
    }
    //随机目标鸭*
    public bool GetRandomTargetDuck()
    {
        return UnityEngine.Random.Range(0,1f) < targetDuckProbability;
    }

    //坐标
    public float GetRandomMovePointX()
    {
        //-25f, 25f
        //在上面
        return UnityEngine.Random.Range(horizaotalRange.x,horizaotalRange.y);
    }
    //坐标
    public float GetRandomMovePointY(bool isUp) //要在默认的Layer的坐标 *
    {
        //这里要细做
        if (isUp) return UnityEngine.Random.Range(duckUpDistance / 3, duckUpDistance);
        else return UnityEngine.Random.Range(0, duckUpDistance / 3);
    }
    //获取鸭信息 *
    public DuckSpawnInfo GetRadnomDuckSpawnInfo()
    {
        //随机鸭 - 随机层
        DuckConfig duckConfig = GetRandomDuckConfig();
        DuckLayerConfig layerConfig = GetRandomDuckLayerConfig();
        //加分鸭
        bool isTarget = GetRandomTargetDuck();
        Vector2 point = new Vector2(GetRandomMovePointX(), layerConfig.poxY);
        //自构造函数
        DuckSpawnInfo duckSpawnInfo = new DuckSpawnInfo(duckConfig,layerConfig,point,GetRandomTargetDuck());
        return duckSpawnInfo;
    }



}

//这个是给类赋值的吗
//下面两个类 - 决定鸭子的层级和类别
[Serializable]
public class DuckConfig //区分不同鸭子的类 *
{    
    public Sprite sprite;
}


[Serializable]
public class DuckLayerConfig //区分不同鸭子的层级 *
{    
    public string sortingLayer;
    public float poxY;
}





//鸭子信息 *
[Serializable]
public struct DuckSpawnInfo
{
    public DuckConfig config;
    public DuckLayerConfig layer;
    public Vector2 spawnPoint;
    public bool isTargetDuck;
    //构造函数
    public DuckSpawnInfo(DuckConfig config, DuckLayerConfig layer, Vector2 spawnPoint, bool isTargetDuck)
    {
        this.config = config;
        this.layer = layer;
        this.spawnPoint = spawnPoint;
        this.isTargetDuck = isTargetDuck;
    }
}