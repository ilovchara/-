using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�����ֻ�ǹ������ֵ�
public class AudioManager : MonoBehaviour
{
    //����Ч����
    public static AudioManager Instance;
    public AudioSource audioSource;
    public AudioClip[] hitDuckClips;
    public AudioClip showSceneClip;
    public AudioClip readyGoClip;
    public AudioClip shotGunClip;
    public AudioClip memuDuckClip;
    public AudioClip memuDuckReadyClip;
    public AudioClip duckGobackClip;
    public AudioClip unHitDuckClip;

    private void Awake()
    {
        Instance = this;
    }

    //�����ǵ��� - �����ɶ��˼
    public void PlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayHitDuckClip()
    {
        PlayOneShot(hitDuckClips[Random.Range(0, hitDuckClips.Length)]);
    }

    public void PlayShowSceneClip()
    {
        PlayOneShot(showSceneClip);
    }

    public void PlayReadyGoClip()
    {
        PlayOneShot(readyGoClip);
    }

    public void PlayShotGunClip()
    {
        PlayOneShot(shotGunClip);
    }

    public void PlayMemuDuckClip()
    {
        PlayOneShot(memuDuckClip);
    }

    public void PlayMemuDuckReadyClip()
    {
        PlayOneShot(memuDuckReadyClip);
    }

    public void PlayDuckGobackClip()
    {
        PlayOneShot(duckGobackClip);
    }

    public void PlayUnHitDuckClip()
    {
        PlayOneShot(unHitDuckClip);
    }

}
