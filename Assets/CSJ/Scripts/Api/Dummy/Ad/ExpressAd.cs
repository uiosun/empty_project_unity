﻿//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)

namespace ByteDance.Union
{
    using ByteDance.Union.Mediation;
    using UnityEngine;
    /// <summary>
    /// The express Ad.
    /// </summary>
    public sealed class ExpressAd : IClientBidding
    {

        public int index;

        internal ExpressAd(AndroidJavaObject expressAd) { }
        /// <summary>
        /// Gets the interaction type.
        /// </summary>
        public int GetInteractionType() { return 0; }

        public AndroidJavaObject handle { get { return null; } }

        /// <summary>
        /// Sets the interaction listener for this Ad.
        /// </summary>
        public void SetExpressInteractionListener(IExpressAdInteractionListener listener, bool callbackOnMainThread = true) { }

        /// <summary>
        /// Sets the dislike callback.
        /// </summary>
        /// <param name="dislikeCallback">Dislike callback.</param>
        public void SetDislikeCallback(IDislikeInteractionListener dislikeCallback, bool callbackOnMainThread = true) { }

        /// <summary>
        /// Sets the download listener.
        /// </summary>
        public void SetDownloadListener(IAppDownloadListener listener, bool callbackOnMainThread = true) { }

        /// <summary>
        /// Set this Ad not allow sdk count down.
        /// </summary>
        public void SetNotAllowSdkCountdown() { }

        /// <summary>
        /// show the  express Ad
        /// <param name="x">the x of th ad</param>
        /// <param name="y">the y of th ad</param>
        /// </summary>
		public void ShowExpressAd(float x, float y) { }

        /// <inheritdoc/>
        public void Dispose() { }

        /// <summary>
        /// Sets the slide interval time.
        /// </summary>
        /// <param name="intervalTime">Interval time.</param>
        public void SetSlideIntervalTime(int intervalTime) { }

        public void setAuctionPrice(double price) { }

        public void win(double price) { }

        public void Loss(double price, string reason, string bidder) { }

        public MediationNativeManager GetMediationManager() { return null; }

    }

}

#endif