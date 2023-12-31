//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

namespace ByteDance.Union
{
    /// <summary>
    /// The listener for full screen video Ad.
    /// </summary>
    public interface IFullScreenVideoAdListener
    {
        /// <summary>
        /// Invoke when load Ad error.
        /// </summary>
        void OnError(int code, string message);

        /// <summary>
        /// Invoke when the Ad load success.
        /// </summary>
        void OnFullScreenVideoAdLoad(FullScreenVideoAd ad);

        /// <summary>
        /// The Ad loaded locally, user can play local video directly.
        /// </summary>
        void OnFullScreenVideoCached();
        
        /**
         * 广告视频本地加载完成的回调，接入方可以在这个回调后直接播放本地视频
         */
        void OnFullScreenVideoCached(FullScreenVideoAd ad);
    }
}