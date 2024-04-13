using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckManager : MonoBehaviour
{
    //单例一个类声明一个对象
    public static DuckManager Instance;
    //预制体
    public GameObject duckPrefab;
    //用栈存储鸭子状态？
    private Stack<DuckController> duckPool = new Stack<DuckController>();
    //已经在场景中的鸭子
    private List<DuckController> currentDuckList = new List<DuckController>();


    private void Awake()
    {
        Instance = this;
    }
    //创建菜单鸭
    public void CreatMenuDuck()
    {
        //获取创建的对象鸭子
        DuckController duckController = GetDuck();
        //随机生成
        DuckConfig config = ConfigManager.Instance.GetRandomDuckConfig();
        //动画？
        duckController.InitMenuDuck(config, ConfigManager.Instance.menuDuckPosition);

        //生成纳入鸭子列表
        currentDuckList.Add(duckController);
    }

    private DuckController GetDuck()
    {
        //TryPeek判断顶部是否有对象 - 返回bool类型 *
        //池子没有 - 创建新的
        if (!duckPool.TryPop(out DuckController duckController))
        {
            //没有找到 - 实例化一个 -获取组件
            duckController = GameObject.Instantiate(duckPrefab).GetComponent<DuckController>();
        }
        //
        duckController.gameObject.SetActive(true);
        return duckController;
    }
    //垃圾清除 *
    public void RecycleDuck(DuckController duckController)
    {
        //鸭子隐藏
        duckController.gameObject.SetActive(false);
        //这里是压入？
        duckPool.Push(duckController);
        //清除
        currentDuckList.Remove(duckController);

    }

    //清除鸭子
    public void CleanAllDuck()
    {
        //循环所有的鸭子
        //到这来
        for(int i = currentDuckList.Count-1; i>=0;i--)
        {
            //调用
            RecycleDuck(currentDuckList[i]);
        }
        //没有清理
        //currentDuckList.Clear();
    }

    //加入游戏*
    public void EnterGame()
    {
        //确保要有鸭子
        StartCoroutine(SpawnGameDuckEveryInterval());
        StartCoroutine(SpawnGameDuckAvoidZero());
    }
    //确保鸭子不为0 *
    IEnumerator SpawnGameDuckAvoidZero()
    {
        while (true) 
        {
            yield return null;
            //超级模式下，至少有3*
            if (GameManager.Instance.superMode && currentDuckList.Count <= 3)
            {
                CreateGameDuck();
            }
            else if(currentDuckList.Count == 0){
                CreateGameDuck();
            }
        }


    }

    //协程
    IEnumerator SpawnGameDuckEveryInterval()
    {
        float currTime = 0;
        //刷新时间
        float spawnDuckInterval = GameManager.Instance.superMode ? ConfigManager.Instance.superModeDuckInterval : ConfigManager.Instance.DuckInterval;
        //拿出来
        WaitForSeconds waitForSeconds = new WaitForSeconds(spawnDuckInterval);
        while (true)
        {
            yield return waitForSeconds;
            currTime += spawnDuckInterval;
            if (currentDuckList.Count < 15)
            {
                float randomValue = UnityEngine.Random.Range(0, 1f);
                //曲线取数据
                if(randomValue < ConfigManager.Instance.spawnCurve.Evaluate(currTime/ConfigManager.Instance.maxGameTime))
                {
                    CreateGameDuck();
                }
            }
        }
    }

    //创建鸭子
    private void CreateGameDuck()
    {
        //随机生成
        DuckSpawnInfo spawnInfo = ConfigManager.Instance.GetRadnomDuckSpawnInfo();
        //获取鸭子
        DuckController controller = GetDuck();
        //初始化游戏呀 - 获取生成信息
        controller.InitGameDuck(spawnInfo);
        currentDuckList.Add(controller);

    }


    public void StopGame()
    {
        //回收就行
        StopAllCoroutines();
        CleanAllDuck();
    }
}
