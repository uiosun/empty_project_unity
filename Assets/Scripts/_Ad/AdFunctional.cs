using System;
using ByteDance.Union;
using UnityEngine;
// using Newtonsoft.Json;

namespace _Ad
{
    public class AdFunctional
    {
        private AdNative adNative;
#if UNITY_IOS
    private ExpressRewardVideoAd expressRewardAd; // for iOS
#endif
        public RewardVideoAd rewardAd; // 激励视频
        public static bool callbackOnMainThread = true;

        private static int mNowPlayAgainCount = 0;
        private static int mNextPlayAgainCount = 0;
        private Action callbackComplete;

        private AdNative AdNative
        {
            get
            {
                if (this.adNative == null)
                {
                    this.adNative = SDK.CreateAdNative();
                }
#if UNITY_ANDROID
                SDK.RequestPermissionIfNecessary();
#endif
                return this.adNative;
            }
        }

        public void InitAd()
        {
#if UNITY_IOS
        PangleConfiguration configuration = PangleConfiguration.Createinstance();
        configuration.appID = AdConstant.APP_ID;
#endif
            // todo 如何初始化？
            // Pangle.InitializeSDK((bool success, string message) =>
            // {
            //     Debug.Log("AD 初始化：" + success + "-----" + message);
            //     UIManager.instance.AlertNew("", "AD 初始化：" + success + "-----" + message);
            //     LoadRewardAd();
            // });
        }

        // 加载激励视频
        public void LoadRewardAd()
        {
            Debug.Log("Load ExpressReward");
#if UNITY_IOS
        if (this.expressRewardAd != null)
        {
            this.expressRewardAd.Dispose();
            this.expressRewardAd = null;
        }
#else
            if (this.rewardAd != null)
            {
                this.rewardAd.Dispose();
                this.rewardAd = null;
            }
#endif

            var adSlot = new AdSlot.Builder()
                .SetCodeId(AdConstant.REWARD_VIDEO_EXPRESS_CODE)
                .SetSupportDeepLink(true)
                .SetImageAcceptedSize(Screen.width, Screen.height)
                .SetUserID("user123") // 用户id,必传参数
                .SetMediaExtra("media_extra") // 附加参数，可选
                .SetOrientation(AdOrientation.Horizontal) // 必填参数，期望视频的播放方向
                .Build();
#if UNITY_IOS
        AdNative.LoadExpressRewardAd(adSlot, new ExpressRewardVideoAdListener(this), callbackOnMainThread);
#else
            AdNative.LoadRewardVideoAd(adSlot, new RewardVideoAdListener(this), callbackOnMainThread);
#endif
        }

        // 展示激励视频
        public void ShowRewardAd(Action callback = null)
        {
            // 为保障播放流畅，建议在视频加载完成后展示
            if (rewardAd == null)
            {
                const string msg = "未检测到加载好的广告，现在尝试加载新广告。\n\n请确认联网，并在最多 30 秒后再试";
                LoadRewardAd();
                UIManager.instance.AlertNew("", msg, UIManager.instance.BackIndex, UIManager.instance.BackIndex);
                Debug.Log("<Ad Log>..." + msg);
                return;
            }

            callbackComplete = callback;
            rewardAd.ShowRewardVideoAd();
        }

        private sealed class RewardVideoAdListener : IRewardVideoAdListener
        {
            private AdFunctional _ad;

            public RewardVideoAdListener(AdFunctional instance)
            {
                _ad = instance;
            }

            public void OnError(int code, string message)
            {
                var errMsg =
                    $"广告OnRewardVideoAdLoadError: code {code}; message {message}";
                Debug.LogError("<Ad Log>..." + errMsg);
                UIManager.instance.AlertNew("", $"广告加载失败，请截图并告知开发组：\n{errMsg}", UIManager.instance.BackIndex, UIManager.instance.BackIndex);
            }

            public void OnRewardVideoAdLoad(object ad)
            {
                Debug.Log("<Ad Log>..." + "OnRewardVideoAdLoad");
            }

            public void OnRewardVideoAdCached()
            {
                Debug.Log("<Ad Log>..." + "OnRewardVideoCached");
            }

            public void OnRewardVideoAdLoad(RewardVideoAd ad)
            {
#if UNITY_ANDROID
                var info = ad.GetMediaExtraInfo();
                Debug.Log(
                    $"OnRewardVideoAdLoad info:  expireTimestamp={info["expireTimestamp"]},materialMetaIsFromPreload={info["materialMetaIsFromPreload"]}");
#endif
                Debug.Log("<Ad Log>..." + "OnRewardVideoAdLoad");

                ad.SetRewardAdInteractionListener(
                    new RewardAdInteractionListener(_ad, (() => { _ad.LoadRewardAd(); })), callbackOnMainThread);
                ad.SetAgainRewardAdInteractionListener(
                    new RewardAgainAdInteractionListener(_ad), callbackOnMainThread);
                ad.SetDownloadListener(
                    new AppDownloadListener(_ad), callbackOnMainThread);
#if UNITY_ANDROID
                ad.SetRewardPlayAgainController(new RewardAdPlayAgainController());
#endif
                _ad.rewardAd = ad;
            }

            public void OnExpressRewardVideoAdLoad(ExpressRewardVideoAd ad)
            {
            }

            public void OnRewardVideoCached()
            {
                Debug.Log("<Ad Log>..." + "OnRewardVideoCached");
#if UNITY_IOS
            _ad.rewardAd.setAuctionPrice(1.0);
            _ad.rewardAd.win(1.0);
            _ad.rewardAd.Loss(1.0,"102",bidder:"ylh");
#endif
            }

            public void OnRewardVideoCached(RewardVideoAd ad)
            {
                Debug.Log(
                    $"OnRewardVideoCached RewardVideoAd ad");
            }
        }

        private sealed class RewardAdPlayAgainController : IRewardAdPlayAgainController
        {
            public void GetPlayAgainCondition(int nextPlayAgainCount, Action<PlayAgainCallbackBean> callback)
            {
                Debug.Log("Reward GetPlayAgainCondition");
                mNextPlayAgainCount = nextPlayAgainCount;
                var bean = new PlayAgainCallbackBean(true, "金币", nextPlayAgainCount + "个");
                callback?.Invoke(bean);
            }
        }

        private sealed class RewardAdInteractionListener : IRewardAdInteractionListener
        {
            private Action _callback = null;
            private AdFunctional _ad;

            public RewardAdInteractionListener(AdFunctional ad, Action callback = null)
            {
                _ad = ad;
                _callback = callback;
            }

            public void OnAdShow()
            {
                Debug.Log("<Ad Log>..." + "rewardVideoAd show");
            }

            public void OnAdVideoBarClick()
            {
                Debug.Log("<Ad Log>..." + "rewardVideoAd bar click");
            }

            public void OnAdClose()
            {
                Debug.Log("<Ad Log>..." + "rewardVideoAd close");

                if (this._ad.rewardAd != null)
                {
                    this._ad.rewardAd.Dispose();
                    this._ad.rewardAd = null;
                }
#if UNITY_IOS
            if (this._ad.expressRewardAd != null) {
                this._ad.expressRewardAd.Dispose();
                this._ad.expressRewardAd = null;
            }
#endif
            }

            public void OnVideoSkip()
            {
                Debug.Log("<Ad Log>..." + "rewardVideoAd skip");
            }

            public void OnVideoComplete()
            {
                Debug.Log("<Ad Log>..." + "rewardVideoAd complete");

                _ad.callbackComplete?.Invoke();
                _callback?.Invoke();
            }

            public void OnVideoError()
            {
                string err = $"rewardVideoAd error ";
                Debug.Log("<Ad Log>..." + err);

                UIManager.instance.AlertNew("", $"广告播放失败，请截图并告知开发组：\n{err}", UIManager.instance.BackIndex, UIManager.instance.BackIndex);
            }

            public void OnRewardVerify(
                bool rewardVerify, int rewardAmount, string rewardName, int rewardType, float rewardPropose)
            {
                Debug.Log("<Ad Log>..." + "verify:" + rewardVerify + " amount:" + rewardAmount +
                          " name:" + rewardName + " rewardType:" + rewardType + " rewardPropose:" + rewardPropose);
            }

            public void OnRewardArrived(bool isRewardValid, int rewardType, IRewardBundleModel extraInfo)
            {
                var logString = "OnRewardArrived verify:" + isRewardValid + " rewardType:" + rewardType + " extraInfo:" +
                                extraInfo +
                                $"";
                Debug.Log("<Ad Log>..." + logString);
            }
        }

        private sealed class RewardAgainAdInteractionListener : IRewardAdInteractionListener
        {
            private AdFunctional _ad;

            public RewardAgainAdInteractionListener(AdFunctional _ad)
            {
                this._ad = _ad;
            }

            public void OnAdShow()
            {
                string msg = "Callback --> 第 " + mNowPlayAgainCount + " 次再看 rewardPlayAgain show";
                Debug.Log("<Ad Log>..." + msg);
            }

            public void OnAdVideoBarClick()
            {
                Debug.Log("<Ad Log>..." + "Callback --> 第 " + mNowPlayAgainCount + " 次再看 rewardPlayAgain bar click");
            }

            public void OnAdClose()
            {
                Debug.Log("OnAdClose");
            }

            public void OnVideoSkip()
            {
                Debug.Log("<Ad Log>..." + "Callback --> 第 " + mNowPlayAgainCount +
                          " 次再看 rewardPlayAgain has OnVideoSkip");
            }


            public void OnVideoComplete()
            {
                Debug.Log("<Ad Log>..." + "Callback --> 第 " + mNowPlayAgainCount + " 次再看 rewardPlayAgain complete");
            }

            public void OnVideoError()
            {
                Debug.Log("<Ad Log>..." + "Callback --> 第 " + mNowPlayAgainCount + " 次再看 rewardPlayAgain error");
            }

            public void OnRewardVerify(
                bool rewardVerify, int rewardAmount, string rewardName, int rewardType, float rewardPropose)
            {
                Debug.Log("<Ad Log>..." + "again verify:" + rewardVerify + " amount:" + rewardAmount +
                          " name:" + rewardName + " rewardType:" + rewardType + " rewardPropose:" + rewardPropose +
                          " nowPlayAgainCount: " + mNowPlayAgainCount);
            }

            public void OnRewardArrived(bool isRewardValid, int rewardType, IRewardBundleModel extraInfo)
            {
                var logString = "again OnRewardArrived verify:" + isRewardValid + " rewardType:" + rewardType +
                                " extraInfo:" + extraInfo +
                                $"";
                Debug.Log("<Ad Log>..." + logString);
            }
        }

        private sealed class AppDownloadListener : IAppDownloadListener
        {
            private AdFunctional _ad;

            public AppDownloadListener(AdFunctional _ad)
            {
                this._ad = _ad;
            }

            public void OnIdle()
            {
            }

            public void OnDownloadActive(
                long totalBytes, long currBytes, string fileName, string appName)
            {
                Debug.Log("<Ad Log>..." + "下载中，点击下载区域暂停");
            }

            public void OnDownloadPaused(
                long totalBytes, long currBytes, string fileName, string appName)
            {
                Debug.Log("<Ad Log>..." + "下载暂停，点击下载区域继续");
            }

            public void OnDownloadFailed(
                long totalBytes, long currBytes, string fileName, string appName)
            {
                Debug.Log("<Ad Log>..." + "下载失败，点击下载区域重新下载");
            }

            public void OnDownloadFinished(
                long totalBytes, string fileName, string appName)
            {
                Debug.Log("<Ad Log>..." + "下载完成");
            }

            public void OnInstalled(string fileName, string appName)
            {
                Debug.Log("<Ad Log>..." + "安装完成，点击下载区域打开");
            }
        }
    }
}