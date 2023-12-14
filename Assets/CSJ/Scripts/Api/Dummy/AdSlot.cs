//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

#if UNITY_EDITOR || (!UNITY_ANDROID && !UNITY_IOS)

namespace ByteDance.Union
{

    using ByteDance.Union.Mediation;
    /// <summary>
    /// The slot of a advertisement.
    /// </summary>
    public sealed class AdSlot
    {
        /// <summary>
        /// The builder used to build an Ad slot.
        /// </summary>
        public class Builder
        {
            /// <summary>
            /// Sets the code ID.
            /// </summary>
            public Builder SetCodeId(string codeId) { return this; }

            /// <summary>
            /// Sets the image accepted size. 单位px
            /// </summary>
            public Builder SetImageAcceptedSize(int width, int height) { return this; }

            /// <summary>
            /// Sets the size of the express view accepted in dp.
            /// </summary>
            /// <returns>The Builder.</returns>
            /// <param name="width">Width.</param>
            /// <param name="height">Height.</param>
            public Builder SetExpressViewAcceptedSize(float width, float height) { return this; }

            /// <summary>
            /// Sets a value indicating wheteher the Ad support deep link.
            /// </summary>
            public Builder SetSupportDeepLink(bool support) { return this; }

            /// <summary>
            /// Ises the express ad.
            /// </summary>
            /// <returns>The express ad.</returns>
            /// <param name="isExpressAd">If set to <c>true</c> is express ad.</param>
            public Builder IsExpressAd(bool isExpressAd) { return this; }

            /// <summary>
            /// Sets the Ad count.
            /// </summary>
            public Builder SetAdCount(int count) { return this; }

            /// <summary>
            /// Sets the Native Ad type.
            /// </summary>
            public Builder SetNativeAdType(AdSlotType type) { return this; }

            /// <summary>
            /// Sets the reward name.
            /// </summary>
            public Builder SetRewardName(string name) { return this; }

            /// <summary>
            /// Sets the reward amount.
            /// </summary>
            public Builder SetRewardAmount(int amount) { return this; }

            /// <summary>
            /// Sets the user ID.
            /// </summary>
            public Builder SetUserID(string id) { return this; }

            /// <summary>
            /// Sets the Ad orientation.
            /// </summary>
            public Builder SetOrientation(AdOrientation orientation) { return this; }

            /// <summary>
            /// Sets the extra media for Ad.
            /// </summary>
            public Builder SetMediaExtra(string extra) { return this; }

            /// <summary>
            /// Sets the express banner intervalTime.
            /// </summary>
            public Builder SetSlideIntervalTime(int intervalTime) { return this; }

            public Builder SetAdLoadType(AdLoadType adLoadType) { return this; }
            /// <summary>
            /// Sets the Mediation Ad slot.
            /// </summary>
            public Builder SetMediationAdSlot(MediationAdSlot mAdSlot) { return this; }
            /// <summary>
            /// Build the Ad slot.
            /// </summary>
            public AdSlot Build() { return new AdSlot(); }
        }
    }
}

#endif
