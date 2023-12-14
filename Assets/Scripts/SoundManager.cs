using System;
using System.Collections;
using System.Collections.Generic;
using _Common;
using Index;
using UnityEngine;
using Random = UnityEngine.Random;

// 音频管理器（请勿在此依赖音频资源，可能导致大量的内存占用）
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        Debug.Log("SoundManager 创建完毕");
        DontDestroyOnLoad(instance);
    }

    // 播放器
    public AudioSource efxSource;
    public AudioSource musicSource; // 组件设置了自动播放

    // 音乐波动
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    // 播放一次
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    // 随机音效
    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        
        efxSource.pitch = randomPitch; // 设置音高
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}