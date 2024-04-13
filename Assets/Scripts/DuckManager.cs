using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckManager : MonoBehaviour
{
    //����һ��������һ������
    public static DuckManager Instance;
    //Ԥ����
    public GameObject duckPrefab;
    //��ջ�洢Ѽ��״̬��
    private Stack<DuckController> duckPool = new Stack<DuckController>();
    //�Ѿ��ڳ����е�Ѽ��
    private List<DuckController> currentDuckList = new List<DuckController>();


    private void Awake()
    {
        Instance = this;
    }
    //�����˵�Ѽ
    public void CreatMenuDuck()
    {
        //��ȡ�����Ķ���Ѽ��
        DuckController duckController = GetDuck();
        //�������
        DuckConfig config = ConfigManager.Instance.GetRandomDuckConfig();
        //������
        duckController.InitMenuDuck(config, ConfigManager.Instance.menuDuckPosition);

        //��������Ѽ���б�
        currentDuckList.Add(duckController);
    }

    private DuckController GetDuck()
    {
        //TryPeek�ж϶����Ƿ��ж��� - ����bool���� *
        //����û�� - �����µ�
        if (!duckPool.TryPop(out DuckController duckController))
        {
            //û���ҵ� - ʵ����һ�� -��ȡ���
            duckController = GameObject.Instantiate(duckPrefab).GetComponent<DuckController>();
        }
        //
        duckController.gameObject.SetActive(true);
        return duckController;
    }
    //������� *
    public void RecycleDuck(DuckController duckController)
    {
        //Ѽ������
        duckController.gameObject.SetActive(false);
        //������ѹ�룿
        duckPool.Push(duckController);
        //���
        currentDuckList.Remove(duckController);

    }

    //���Ѽ��
    public void CleanAllDuck()
    {
        //ѭ�����е�Ѽ��
        //������
        for(int i = currentDuckList.Count-1; i>=0;i--)
        {
            //����
            RecycleDuck(currentDuckList[i]);
        }
        //û������
        //currentDuckList.Clear();
    }

    //������Ϸ*
    public void EnterGame()
    {
        //ȷ��Ҫ��Ѽ��
        StartCoroutine(SpawnGameDuckEveryInterval());
        StartCoroutine(SpawnGameDuckAvoidZero());
    }
    //ȷ��Ѽ�Ӳ�Ϊ0 *
    IEnumerator SpawnGameDuckAvoidZero()
    {
        while (true) 
        {
            yield return null;
            //����ģʽ�£�������3*
            if (GameManager.Instance.superMode && currentDuckList.Count <= 3)
            {
                CreateGameDuck();
            }
            else if(currentDuckList.Count == 0){
                CreateGameDuck();
            }
        }


    }

    //Э��
    IEnumerator SpawnGameDuckEveryInterval()
    {
        float currTime = 0;
        //ˢ��ʱ��
        float spawnDuckInterval = GameManager.Instance.superMode ? ConfigManager.Instance.superModeDuckInterval : ConfigManager.Instance.DuckInterval;
        //�ó���
        WaitForSeconds waitForSeconds = new WaitForSeconds(spawnDuckInterval);
        while (true)
        {
            yield return waitForSeconds;
            currTime += spawnDuckInterval;
            if (currentDuckList.Count < 15)
            {
                float randomValue = UnityEngine.Random.Range(0, 1f);
                //����ȡ����
                if(randomValue < ConfigManager.Instance.spawnCurve.Evaluate(currTime/ConfigManager.Instance.maxGameTime))
                {
                    CreateGameDuck();
                }
            }
        }
    }

    //����Ѽ��
    private void CreateGameDuck()
    {
        //�������
        DuckSpawnInfo spawnInfo = ConfigManager.Instance.GetRadnomDuckSpawnInfo();
        //��ȡѼ��
        DuckController controller = GetDuck();
        //��ʼ����Ϸѽ - ��ȡ������Ϣ
        controller.InitGameDuck(spawnInfo);
        currentDuckList.Add(controller);

    }


    public void StopGame()
    {
        //���վ���
        StopAllCoroutines();
        CleanAllDuck();
    }
}
