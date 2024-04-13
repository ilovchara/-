using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

public class UIManager : MonoBehaviour
{
    //����
    public static UIManager Instance;

    public Animation ReadyGoAnimation;
    //�÷ְ�����
    public Text scoreText; //��
    public Text timeText; //ʱ��
    public Image gunCDMask; //cd

    //����ģʽ - �����Ǹ���ť
    public Button superModeButton;
    //private WXRewardedVideoAd videoAd; - ������

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

        //videoAd = new WXRewardedVideoAd(new WXCreateBannerAdParam() { }); //����Ҫ��id
        EnterMenu();
    }



    //����ģʽ
    private void SuperModeButtonClick()
    {
        //����棿 - ����Ͳ������ǿ��� - ֮�������
        GameManager.Instance.superMode = true;
        
    }

    //�����Ǹ������ذ� *
    public void EnterMenu()
    {
        //��Ϸ�ر�
        gameOverPanel.gameObject.SetActive(false);
        ReadyGoAnimation.gameObject.SetActive(false);
        //��������
        scoreText.transform.parent.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        //������������� - ԭ��������������
        gunCDMask.transform.parent.gameObject.SetActive(false);
        superModeButton.gameObject.SetActive(true);


    }

    public void EnterReadyGo()
    {
        //��ʾ����
        ReadyGoAnimation.gameObject.SetActive(true);
        //������
        ReadyGoAnimation.Play("ReadyGo");
    }
    //�÷ְ���ʾ *
    public void EnterGame()
    {
        
        scoreText.transform.parent.gameObject.SetActive(true);
        timeText.gameObject.SetActive(true);
        gunCDMask.transform.parent.gameObject.SetActive(true);
        //�����������ģʽ�İ�ť
        superModeButton.gameObject.SetActive(false);

    }
    //����ת���ı� - int - string *
    public void UpdateScore(int score)
    {
        
        scoreText.text = score.ToString();
    }

    //����cd����
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
        //����
        gameOverScoreText.text = scoreText.text;
        gameOverPanel.gameObject.SetActive(true);

    }

    //�ر���Ϸ
    private void GameOverCloseButtonClick()
    {
        GameManager.Instance.GameState = GameState.Menu;
    }

    private void GameOverShareButtonClick()
    {
        //���� - ��΢�ŵ�
        WX.ShareAppMessage(new ShareAppMessageOption() { title = $"����ѼѼ����л����{gameOverScoreText.text}�ķ���,��Ҳ�����Կ��ɣ�" });
    }

}
