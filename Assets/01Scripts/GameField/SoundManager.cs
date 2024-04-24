using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
public class SoundManager : Singleton<SoundManager>
{
    public enum eTYPE_BGM
    {
        Field = 0,
        Deongun,
        GolemBoss
    }
    [SerializeField] AudioClip[] bgmClips;

    public enum eTYPE_EFFECT
    {
        walk,
        sword1,
        sword2,
        sword3,
        sword4,
        punch,
        explosion,
        confirmMenu,
        cancleMenu,
        twinleMenu
    }
    [SerializeField] AudioClip[] EffectClips;

    AudioSource bgmPlayer;
    List<AudioSource> _ltEffPlayers;


    private void Awake()
    {
        bgmPlayer = GetComponent<AudioSource>();
        _ltEffPlayers = new List<AudioSource>();
    }

    void LateUpdate()
    {
        foreach (AudioSource item in _ltEffPlayers)
        {
            if (item.isPlaying == false)
            {
                _ltEffPlayers.Remove(item);
                Destroy(item.gameObject);
                break;
            }
        }
    }
    private void PlayBGM(eTYPE_BGM type, float volum = 1.0f, bool isloop = true)
    {
        if (bgmPlayer.isPlaying && bgmPlayer.clip == bgmClips[(int)type])
        {
            return;
        }

        bgmPlayer.clip = bgmClips[(int)type];
        bgmPlayer.volume = volum;
        bgmPlayer.loop = isloop;

        bgmPlayer.Play();
    }

    public void PlayEffect_OnMng(eTYPE_EFFECT type, float volume = 1.0f, bool loop = false)
    {
        GameObject go = new GameObject("EffectClips");      
        go.transform.SetParent(transform);
        AudioSource AS = go.AddComponent<AudioSource>();
        AS.clip = EffectClips[(int)type];
        AS.volume = volume;
        AS.loop = loop;

        AS.Play();
        //재생이 끝난 오디오는 파괴를 위해, 리스트로 보냄
        _ltEffPlayers.Add(AS);

    }

    public void PlayEffect(GameObject obj, eTYPE_EFFECT type, float volume = 1.0f, bool loop = false)
    {
        GameObject go = new GameObject("EffectClips");
        go.transform.SetParent(obj.transform);
        go.transform.localPosition = Vector3.zero;


        AudioSource AS = go.AddComponent<AudioSource>();
        AS.clip = EffectClips[(int)type];
        AS.volume = volume;
        AS.loop = loop;

        AS.Play();
        _ltEffPlayers.Add(AS);
    }
    public async UniTaskVoid PlayEffect(GameObject obj, eTYPE_EFFECT type, float volume, float time, bool loop)
    {
        // AudioSource를 생성하고 설정합니다.
        GameObject go = new GameObject("EffectClips");
        go.transform.SetParent(obj.transform);
        go.transform.localPosition = Vector3.zero;

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.clip = EffectClips[(int)type];
        audioSource.volume = volume;
        audioSource.loop = loop;

        // 사운드를 재생합니다.
        audioSource.Play();

        // 지정된 시간이 경과할 때까지 대기합니다.
        int delayTimeMillis = Mathf.Min((int)(time * 1000), (int)(audioSource.clip.length * 1000));
        await UniTask.Delay(delayTimeMillis);

        // 사운드를 중지하고 GameObject를 제거합니다.
        audioSource.Stop();
        Destroy(go);
    }


    #region 브금처리

    public void BgmSoundSetting(string name)
    {
        switch (name)
        {
            case "GameField":
                PlayBGM(eTYPE_BGM.Field);
                break;
            case "Dungeon_1":
                PlayBGM(eTYPE_BGM.Deongun);
                break;
            case "GolemBoss":
                PlayBGM(eTYPE_BGM.GolemBoss);
                break;
        }
    }
    public void BgmSoundSetting(eTYPE_BGM type)
    {
        switch (type)
        {
            case eTYPE_BGM.Field:
                PlayBGM(eTYPE_BGM.Field);
                break;
            case eTYPE_BGM.Deongun:
                PlayBGM(eTYPE_BGM.Deongun);
                break;
            case eTYPE_BGM.GolemBoss:
                PlayBGM(eTYPE_BGM.GolemBoss);
                break;
        }
    }

    #endregion
}
