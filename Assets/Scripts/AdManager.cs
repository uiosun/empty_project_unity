using System;
using System.Collections;
using System.Collections.Generic;
using _Ad;
using UnityEngine;
using UnityEngine.Events;

// 回合管理器，在游戏存档管理器后加载
public class AdManager : MonoBehaviour
{
    public static AdManager instance;

    // 广告加载器
    private AdFunctional _ad = new AdFunctional();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        _ad.InitAd();

        Debug.Log("AdManager 创建完毕");
        DontDestroyOnLoad(instance);
    }

    public void PlayRewardAd(Action callback = null)
    {
        _ad.ShowRewardAd(callback);
    }
}