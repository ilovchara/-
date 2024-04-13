using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

public enum GameState
{
    Menu,
    ReadyGo,
    Game,
    GameOver
};



public class GameManager : MonoBehaviour
{
    //�õ��������
    public static GameManager Instance;
    public Animation gunAnimation;
    //�ı����
    public bool superMode; //����ģʽ
    private int currentScore; //����
    //��Ϸʱ�� - ��ʱ��
    private float gameTime; //��Ϸʱ��
    private float shootTimer; //�����ʱ��

    //�𶯲���
    private VibrateShortOption vibrateShort = new VibrateShortOption() { type  = "medium"};
    private VibrateShortOption superVibrateShort = new VibrateShortOption() { type = "heavy" };




    private void Awake()
    {
        Instance = this;
    }

    //״̬��
    private GameState gameState;
    public GameState GameState
    {
        //ö������
        get => gameState;
        set
        {
            gameState = value;
            switch (gameState)
            {
                case GameState.Menu:
                    superMode = false;
                    UIManager.Instance.EnterMenu();                    
                    DuckManager.Instance.CreatMenuDuck(); //�ٻ��˵�Ѽ
                    break; 
                case GameState.ReadyGo:
                    UIManager.Instance.EnterReadyGo();
                    AudioManager.Instance.PlayReadyGoClip();
                    //�ӳ�ʱ��2�� �����Ⱑ - ������������� -��ֱ��ʹ������ֵ������ 
                    //Invoke(nameof(StartGame), ConfigManager.Instance.readyGoAnimationTime);
                    Invoke(nameof(StartGame), 2f); 
                    break;
                case GameState.Game: //*
                    currentScore = 0; //�÷�
                    gameTime = ConfigManager.Instance.maxGameTime; //һ���ʱ�䣬60��ʼ
                    //�������ɣ�
                    UIManager.Instance.EnterGame();
                    UIManager.Instance.UpdateScore(currentScore);
                    DuckManager.Instance.EnterGame();
                    break;
                case GameState.GameOver:
                    DuckManager.Instance.StopGame();
                    UIManager.Instance.GameOver();
                    //��ʱ����ʾ��������
                    break;
            }
        }
    }

    //�ӳ�ʵ��
    IEnumerator Start()
    {
        AudioManager.Instance.PlayShowSceneClip();
        //�ӳ���Ф1.5��
        yield return new WaitForSeconds(1.5f);
        GameState = GameState.Menu;
    }
    //ÿһ֡��
    private void Update()
    {
        switch (GameState)
        {
            case GameState.Menu:
                //�ж��Ƿ���ʧ
                DuckController menuDuck = RayCastDuck();
                if(menuDuck != null && !menuDuck.isDie)
                {
                    GunShoot(menuDuck);
                    menuDuck.Die();
                    GameState = GameState.ReadyGo;
                }
                break;
            case GameState.ReadyGo:               
                break;
            case GameState.Game:
                shootTimer -= Time.deltaTime;
                gameTime -= Time.deltaTime;
                if (gameTime <= 0)
                {
                    //������ָ���
                    gameTime = 0;
                    UIManager.Instance.UpdateTime((int)gameTime);
                    GameState = GameState.GameOver;
                    return;
                }
                
                //����ʱ��
                UIManager.Instance.UpdateTime((int)gameTime);
                //�����е�����
                //ѡ������cdʱ��
                if (superMode == true) ConfigManager.Instance.superModeShootCD = 0.1f;
                //Debug.Log(ConfigManager.Instance.superModeShootCD);
                float shootCD = superMode ? ConfigManager.Instance.superModeShootCD : ConfigManager.Instance.shootCD; //�������Ŀ���ʽ������
                UIManager.Instance.UpdateGunCd(shootTimer / shootCD); //ģʽ
                if (shootTimer <= 0 && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    shootTimer = shootCD;
                    //ȷ�����������
                    DuckController duck = RayCastDuck();
                    //Ѽ�Ӵ��� - Ѽ��û��
                    if(duck!=null && !duck.isDie)
                    {
                        currentScore += duck.isTargetDuck ? 5 : 1;
                        GunShoot(duck);
                        duck.Die();
                        //���·���
                        UIManager.Instance.UpdateScore(currentScore);
                        //��
                        if (superMode) WX.VibrateShort(superVibrateShort);
                        else WX.VibrateShort(vibrateShort);                       
                       
                    }
                    else{
                        //û����� -����Ѽ�ӳ�Ц
                        if (!superMode) { 
                            AudioManager.Instance.PlayUnHitDuckClip();
                        }
                        GunShoot(Input.GetTouch(0).position);
                    }

                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    //������Ϊ���ӳ�����game
    private void StartGame()
    {
        GameState = GameState.Game;
    }


    //�ж����Ѽ�� - �������ж�
    private DuckController RayCastDuck()
    {
        if(Input.touchCount>0)
        {
            //�ж�һ������
            UnityEngine.Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //������Ļ�������� - ������Ҫ������Ļ
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit2D hitInfo = Physics2D.Raycast(ray.origin, Vector2.zero,Mathf.Infinity);
                if(hitInfo.collider != null && hitInfo.collider.TryGetComponent(out DuckController duckController))
                {
                    return duckController;
                }
            }
        }
        return null;
    }
   
    private void GunShoot(Vector3 pos)
    {
        gunAnimation.transform.position = pos;
        //���Ķ���
        gunAnimation.transform.localScale = new Vector3(pos.x > 0 ? 1 : -1, 1, 1);
        //���Ŷ���
        gunAnimation.Play("Gun");
        AudioManager.Instance.PlayShotGunClip();
    }


    //���������
    private void GunShoot(DuckController duckController)
    {
        GunShoot(duckController.transform.position + ConfigManager.Instance.gunOffset);
        AudioManager.Instance.PlayHitDuckClip();
    }



}
