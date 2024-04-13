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
    //用单例来设计
    public static GameManager Instance;
    public Animation gunAnimation;
    //文本组件
    public bool superMode; //超级模式
    private int currentScore; //分数
    //游戏时间 - 计时器
    private float gameTime; //游戏时间
    private float shootTimer; //射击计时器

    //震动参数
    private VibrateShortOption vibrateShort = new VibrateShortOption() { type  = "medium"};
    private VibrateShortOption superVibrateShort = new VibrateShortOption() { type = "heavy" };




    private void Awake()
    {
        Instance = this;
    }

    //状态机
    private GameState gameState;
    public GameState GameState
    {
        //枚举数据
        get => gameState;
        set
        {
            gameState = value;
            switch (gameState)
            {
                case GameState.Menu:
                    superMode = false;
                    UIManager.Instance.EnterMenu();                    
                    DuckManager.Instance.CreatMenuDuck(); //召唤菜单鸭
                    break; 
                case GameState.ReadyGo:
                    UIManager.Instance.EnterReadyGo();
                    AudioManager.Instance.PlayReadyGoClip();
                    //延迟时间2秒 有问题啊 - 这里变量有问题 -我直接使用死的值来运行 
                    //Invoke(nameof(StartGame), ConfigManager.Instance.readyGoAnimationTime);
                    Invoke(nameof(StartGame), 2f); 
                    break;
                case GameState.Game: //*
                    currentScore = 0; //得分
                    gameTime = ConfigManager.Instance.maxGameTime; //一盘最长时间，60开始
                    //这里生成？
                    UIManager.Instance.EnterGame();
                    UIManager.Instance.UpdateScore(currentScore);
                    DuckManager.Instance.EnterGame();
                    break;
                case GameState.GameOver:
                    DuckManager.Instance.StopGame();
                    UIManager.Instance.GameOver();
                    //到时候显示出来就行
                    break;
            }
        }
    }

    //延迟实现
    IEnumerator Start()
    {
        AudioManager.Instance.PlayShowSceneClip();
        //延迟生肖1.5秒
        yield return new WaitForSeconds(1.5f);
        GameState = GameState.Menu;
    }
    //每一帧的
    private void Update()
    {
        switch (GameState)
        {
            case GameState.Menu:
                //判断是否消失
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
                    //避免出现负数
                    gameTime = 0;
                    UIManager.Instance.UpdateTime((int)gameTime);
                    GameState = GameState.GameOver;
                    return;
                }
                
                //更新时间
                UIManager.Instance.UpdateTime((int)gameTime);
                //这里有点问题
                //选择武器cd时间
                if (superMode == true) ConfigManager.Instance.superModeShootCD = 0.1f;
                //Debug.Log(ConfigManager.Instance.superModeShootCD);
                float shootCD = superMode ? ConfigManager.Instance.superModeShootCD : ConfigManager.Instance.shootCD; //这里的三目表达式有问题
                UIManager.Instance.UpdateGunCd(shootTimer / shootCD); //模式
                if (shootTimer <= 0 && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    shootTimer = shootCD;
                    //确定有无射击到
                    DuckController duck = RayCastDuck();
                    //鸭子存在 - 鸭子没死
                    if(duck!=null && !duck.isDie)
                    {
                        currentScore += duck.isTargetDuck ? 5 : 1;
                        GunShoot(duck);
                        duck.Die();
                        //更新分数
                        UIManager.Instance.UpdateScore(currentScore);
                        //震动
                        if (superMode) WX.VibrateShort(superVibrateShort);
                        else WX.VibrateShort(vibrateShort);                       
                       
                    }
                    else{
                        //没射击到 -就让鸭子嘲笑
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

    //这里是为了延迟启动game
    private void StartGame()
    {
        GameState = GameState.Game;
    }


    //判断射击鸭子 - 用射线判断
    private DuckController RayCastDuck()
    {
        if(Input.touchCount>0)
        {
            //判断一个触摸
            UnityEngine.Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //创建屏幕发出射线 - 这里需要触摸屏幕
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
        //抢的动画
        gunAnimation.transform.localScale = new Vector3(pos.x > 0 ? 1 : -1, 1, 1);
        //播放动画
        gunAnimation.Play("Gun");
        AudioManager.Instance.PlayShotGunClip();
    }


    //处理抢射击
    private void GunShoot(DuckController duckController)
    {
        GunShoot(duckController.transform.position + ConfigManager.Instance.gunOffset);
        AudioManager.Instance.PlayHitDuckClip();
    }



}
