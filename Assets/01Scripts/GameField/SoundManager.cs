using System.Collections.Generic;
using UnityEngine;

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
    public void PlayBGM(eTYPE_BGM type, float volum = 1.0f, bool isloop = true)
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
}
