//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

namespace ByteDance.Union
{
#if !UNITY_EDITOR && UNITY_IOS

    /// <summary>
    /// The advertisement native object for iOS.
    /// </summary>
    public sealed class AdNative
    {
        /// <summary>
        /// Load the native Ad asynchronously and notice on listener.
        /// </summary>
        public void LoadNativeAd(AdSlot adSlot, INativeAdListener listener, bool callbackOnMainThead = true)
        {
            NativeAd.LoadNativeAd(adSlot, listener, callbackOnMainThead);
        }

        /// <summary>
        /// Load the splash Ad asynchronously and notice on listener with
        /// specify timeout.
        /// </summary>
        public BUSplashAd LoadSplashAd_iOS(AdSlot adSlot, ISplashAdListener listener, int timeOut, bool callbackOnMainThead = true)
        {
            return BUSplashAd.LoadSplashAd(adSlot, listener, timeOut, callbackOnMainThead);
        }

        /// <summary>
        /// Load the splash Ad asynchronously and notice on listener.
        /// </summary>
        public BUSplashAd LoadSplashAd_iOS(AdSlot adSlot, ISplashAdListener listener, bool callbackOnMainThead = true)
        {
            return BUSplashAd.LoadSplashAd(adSlot, listener, -1, callbackOnMainThead);
        }

        /// <summary>
        /// Load the reward video Ad asynchronously and notice on listener.
        /// </summary>
        public void LoadRewardVideoAd(AdSlot adSlot, IRewardVideoAdListener listener, bool callbackOnMainThead = true)
        {
            RewardVideoAd.LoadRewardVideoAd(adSlot, listener, callbackOnMainThead);
        }

        /// <summary>
        /// Load the full screen video Ad asynchronously and notice on listener.
        /// </summary>
        public void LoadFullScreenVideoAd(AdSlot adSlot, IFullScreenVideoAdListener listener, bool callbackOnMainThead = true)
        {
            FullScreenVideoAd.LoadFullScreenVideoAd(adSlot, listener, callbackOnMainThead, false);
        }

        public void LoadExpressRewardAd(AdSlot adSlot, IRewardVideoAdListener listener, bool callbackOnMainThead = true)
        {
            ExpressRewardVideoAd.LoadRewardVideoAd(adSlot, listener, callbackOnMainThead);
        }

        public void LoadExpressFullScreenVideoAd(AdSlot adSlot, IFullScreenVideoAdListener listener, bool callbackOnMainThead = true)
        {
            FullScreenVideoAd.LoadFullScreenVideoAd(adSlot, listener, callbackOnMainThead, true);
        }

        public void LoadNativeExpressAd(AdSlot adSlot, IExpressAdListener listener, bool callbackOnMainThead = true)
        {
            ExpressAd.LoadExpressAdAd(adSlot, listener, callbackOnMainThead);
        }

        public void LoadExpressBannerAd(AdSlot adSlot, IExpressBannerAdListener listener, bool callbackOnMainThead = true)
        {
            ExpressBannerAd.LoadExpressAd(adSlot, listener, callbackOnMainThead);
        }

        public void LoadFeedAd(AdSlot adSlot, IFeedAdListener listener, bool callbackOnMainThread = true)
        {
            FeedAd.LoadFeedAd(adSlot, listener, callbackOnMainThread);
        }

        public void LoadDrawFeedAd(AdSlot adSlot, IDrawFeedAdListener listener, bool callbackOnMainThread = true)
        {
            DrawFeedAd.LoadDrawFeedAd(adSlot, listener, callbackOnMainThread);
        }

        public void LoadSplashAd(AdSlot adSlot, ISplashAdListener listener, int timeOut, bool callbackOnMainThread = true)
        {
            listener.OnSplashLoadFail(0, "Not Support on this platform");
        }

        public void LoadSplashAd(AdSlot adSlot, ISplashAdListener listener, bool callbackOnMainThread = true)
        {
            listener.OnSplashLoadFail(0, "Not Support on this platform");
        }
    }
#endif
}