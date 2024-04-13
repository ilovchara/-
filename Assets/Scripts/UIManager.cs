using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class UIManager : MonoBehaviour
{
    //单例
    public static UIManager Instance;

    public Animation ReadyGoAnimation;
    //得分板的组件
    public Text scoreText; //分
    public Text timeText; //时间
    public Image gunCDMask; //cd

    //超级模式 - 这里是个按钮
    public Button superModeButton;
    //private WXRewardedVideoAd videoAd; - 看广告的

    public Transform gameOverPanel;
    public Button gameOverShareButton;
    public Button gameOverCloseButton;

    public Text gameOverScoreText;


    private void Awake()
    {
        Instance = this;

        superModeButton.onClick.AddListener(SuperModeButtonClick);
        gameOverShareButton.onClick.AddListener(GameOverShareButtonClick);
        gameOverCloseButton.onClick.AddListener(GameOverCloseButtonClick);

        //videoAd = new WXRewardedVideoAd(new WXCreateBannerAdParam() { }); //这里要填id
        EnterMenu();
    }



    //超级模式
    private void SuperModeButtonClick()
    {
        //看广告？ - 这里就不让他们看了 - 之后再添加
        GameManager.Instance.superMode = true;
        
    }

    //这里是父类隐藏啊 *
    public void EnterMenu()
    {
        //游戏关闭
        gameOverPanel.gameObject.SetActive(false);
        ReadyGoAnimation.gameObject.SetActive(false);
        //分数隐藏
        scoreText.transform.parent.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        //这里出现问题了 - 原因是我拉错东西了
        gunCDMask.transform.parent.gameObject.SetActive(false);
        superModeButton.gameObject.SetActive(true);


    }

    public void EnterReadyGo()
    {
        //显示出来
        ReadyGoAnimation.gameObject.SetActive(true);
        //播动画
        ReadyGoAnimation.Play("ReadyGo");
    }
    //得分板显示 *
    public void EnterGame()
    {
        
        scoreText.transform.parent.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        gunCDMask.transform.parent.gameObject.SetActive(true);
        //隐藏这个超级模式的按钮
        superModeButton.gameObject.SetActive(false);

    }
    //分数转化文本 - int - string *
    public void UpdateScore(int score)
    {
        
        scoreText.text = score.ToString();
    }

    //武器cd遮罩
    public void UpdateGunCd(float value)
    {
        gunCDMask.fillAmount = value;
    }


    public void UpdateTime(int time)
    {
        timeText.text = time.ToString();
    }

    public void GameOver()
    {
        //分数
        gameOverScoreText.text = scoreText.text;
        gameOverPanel.gameObject.SetActive(true);

    }

    //关闭游戏
    private void GameOverCloseButtonClick()
    {
        GameManager.Instance.GameState = GameState.Menu;
    }

    private void GameOverShareButtonClick()
    {
        //分享 - 用微信的
        WX.ShareAppMessage(new ShareAppMessageOption() { title = $"我在鸭鸭射击中获得了{gameOverScoreText.text}的分数,你也来试试看吧！" });
    }

}
