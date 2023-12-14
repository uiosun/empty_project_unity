﻿//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

namespace ByteDance.Union
{
#if !UNITY_EDITOR && UNITY_IOS
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using ByteDance.Union.Mediation;
    using UnityEngine;

    /// <summary>
    /// The reward video Ad.
    /// </summary>
    public sealed class ExpressBannerAd : IDisposable, IClientBidding
    {
        private static int loadContextID = 0;
        private static Dictionary<int, IExpressBannerAdListener> loadListeners = new Dictionary<int, IExpressBannerAdListener>();

        private static int interactionContextID = 0;
        private static Dictionary<int, IExpressBannerInteractionListener> interactionListeners = new Dictionary<int, IExpressBannerInteractionListener>();

        private delegate void ExpressAd_OnLoad(IntPtr expressAd, int context);
        private delegate void ExpressAd_OnLoadError(int code, string message, int context);
        private delegate void ExpressAd_WillVisible(int context);
        private delegate void ExpressAd_DidClick(int context);
        private delegate void ExpressAd_DidClose(int context);
        private delegate void ExpressAd_DidRemove(int context);

        private delegate void ExpressAd_RenderSuccess(IntPtr expressAd, int context);
        private delegate void ExpressAd_RenderFailed(int code, string message, int context);




        private IntPtr expressAd;
        private bool disposed;

        private static bool _callbackOnMainThead;
        internal ExpressBannerAd(IntPtr expressAd)
        {
            this.expressAd = expressAd;
        }

        ~ExpressBannerAd()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            UnionPlatform_ExpressBannerAd_Dispose(this.expressAd);
            this.disposed = true;
        }

        public void ShowExpressAd(float originX, float originY)
        {
            UnionPlatform_ExpressBannersAd_Show(this.expressAd, originX, originY);
        }

        internal static void LoadExpressAd(AdSlot slot, IExpressBannerAdListener listener, bool callbackOnMainThead)
        {
            _callbackOnMainThead = callbackOnMainThead;
            var context = loadContextID++;
            loadListeners.Add(context, listener);
            AdSlotStruct adSlot = AdSlotBuilder.getAdSlot(slot);
            UnionPlatform_ExpressBannersAd_Load(
                ref adSlot,
                ExpressAd_OnLoadMethod,
                ExpressAd_OnLoadErrorMethod,
                context);
        }

        public void SetDislikeCallback(IDislikeInteractionListener dislikeCallback, bool callbackOnMainThread = true)
        {
        }

        /// <summary>
        /// Sets the download listener.
        /// </summary>
        public void SetDownloadListener(IAppDownloadListener listener, bool callbackOnMainThread = true) { }

        /// <summary>
        /// Sets the interaction listener for this Ad.
        /// </summary>
        public void SetExpressInteractionListener(IExpressBannerInteractionListener listener, bool callbackOnMainThead = true)
        {
            _callbackOnMainThead = callbackOnMainThead;
            var context = interactionContextID++;
            interactionListeners.Add(context, listener);

            Debug.Log("CSJM_Unity " + "chaors unity interactionContextID:" + interactionContextID);
            UnionPlatform_ExpressBannersAd_SetInteractionListener(
                this.expressAd,
                ExpressAd_RenderSuccessMethod,
                ExpressAd_RenderFailedMethod,
                ExpressAd_WillVisibleMethod,
                ExpressAd_DidClickMethod,
                ExpressAd_OnAdDidCloseMethod,
                ExpressAd_DidRemoveMethod,
                context);
        }

        [DllImport("__Internal")]
        private static extern void UnionPlatform_ExpressBannersAd_Load(
            ref AdSlotStruct adSlot,
            ExpressAd_OnLoad onLoad,
            ExpressAd_OnLoadError onLoadError,
            int context);

        [DllImport("__Internal")]
        private static extern void UnionPlatform_ExpressBannersAd_Show(
            IntPtr expressAd,
            float originX,
            float originY);


        [DllImport("__Internal")]
        private static extern void UnionPlatform_ExpressBannersAd_SetInteractionListener(
            IntPtr expressAd,
            ExpressAd_RenderSuccess renderSuccess,
            ExpressAd_RenderFailed renderFailed,
            ExpressAd_WillVisible willVisible,
            ExpressAd_DidClick didClick,
            ExpressAd_DidClose didClose,
            ExpressAd_DidRemove didRemove,
            int context);

        [DllImport("__Internal")]
        private static extern void UnionPlatform_ExpressBannerAd_Dispose(
            IntPtr expressAdPtr);


        [DllImport("__Internal")]
        private static extern bool UnionPlatform_bannerHaveMediationManager(IntPtr expressAdPtr);

        [AOT.MonoPInvokeCallback(typeof(ExpressAd_OnLoad))]
        private static void ExpressAd_OnLoadMethod(IntPtr expressAd, int context)
        {
            Debug.Log("CSJM_Unity " + "OnExpressBannerAdLoad");
            UnityDispatcher.PostTask(() =>
            {
                ;
                IExpressBannerAdListener listener;
                if (loadListeners.TryGetValue(context, out listener))
                {
                    loadListeners.Remove(context);
                    listener.OnBannerAdLoad(new ExpressBannerAd(expressAd));
                }
                else
                {
                    Debug.LogError("CSJM_Unity " +
                        "The ExpressAd_OnLoad can not find the context.");
                }
            }, _callbackOnMainThead);
        }

        [AOT.MonoPInvokeCallback(typeof(ExpressAd_OnLoadError))]
        private static void ExpressAd_OnLoadErrorMethod(int code, string message, int context)
        {
            Debug.Log("CSJM_Unity " + "onExpressAdError " + message);
            UnityDispatcher.PostTask(() =>
            {
                IExpressBannerAdListener listener;
                if (loadListeners.TryGetValue(context, out listener))
                {
                    listener.OnError(code, message);
                    loadListeners.Remove(context);
                }
                else
                {
                    Debug.LogError("CSJM_Unity " +
                        "The ExpressAd_OnLoadError can not find the context.");
                }
            }, _callbackOnMainThead);
        }

        [AOT.MonoPInvokeCallback(typeof(ExpressAd_RenderFailed))]
        private static void ExpressAd_RenderFailedMethod(int code, string message, int context)
        {
            Debug.Log("CSJM_Unity " + "express OnAdViewRenderError,type:ExpressBannerAd");
            UnityDispatcher.PostTask(() =>
            {
                IExpressBannerInteractionListener listener;
                if (interactionListeners.TryGetValue(context, out listener))
                {
                    listener.OnAdViewRenderError(code, message);
                    interactionListeners.Remove(context);
                }
                else
                {
                    Debug.LogError("CSJM_Unity " +
                        "The ExpressAd_RenderFailed can not find the context.");
                }
            });
        }

        [AOT.MonoPInvokeCallback(typeof(ExpressAd_RenderSuccess))]
        private static void ExpressAd_RenderSuccessMethod(IntPtr expressAd, int context)
        {
            UnityDispatcher.PostTask(() =>
            {
                ;
                IExpressBannerInteractionListener listener;
                if (interactionListeners.TryGetValue(context, out listener))
                {
                    interactionListeners.Remove(context);
                    listener.OnAdViewRenderSucc(0, 0);
                }
                else
                {
                    Debug.LogError("CSJM_Unity " +
                        "The ExpressAd_RenderSuccessMethod can not find the context.");
                }
            });
        }

        [AOT.MonoPInvokeCallback(typeof(ExpressAd_WillVisible))]
        private static void ExpressAd_WillVisibleMethod(int context)
        {
            UnityDispatcher.PostTask(() =>
            {
                IExpressBannerInteractionListener listener;
                if (interactionListeners.TryGetValue(context, out listener))
                {
                    listener.OnAdShow();
                }
                else
                {
                    Debug.LogError("CSJM_Unity " +
                        "The ExpressAd_WillVisible can not find the context.");
                }
            }, _callbackOnMainThead);
        }

        [AOT.MonoPInvokeCallback(typeof(ExpressAd_DidClick))]
        private static void ExpressAd_DidClickMethod(int context)
        {
            UnityDispatcher.PostTask(() =>
            {
                IExpressBannerInteractionListener listener;
                if (interactionListeners.TryGetValue(context, out listener))
                {
                    listener.OnAdClicked();
                }
                else
                {
                    Debug.LogError("CSJM_Unity " +
                        "The ExpressAd_DidClick can not find the context.");
                }
            }, _callbackOnMainThead);
        }

        [AOT.MonoPInvokeCallback(typeof(ExpressAd_DidRemove))]
        private static void ExpressAd_DidRemoveMethod(int context)
        {
            UnityDispatcher.PostTask(() =>
            {
                IExpressBannerInteractionListener listener;
                if (interactionListeners.TryGetValue(context, out listener))
                {
                    listener.onAdRemoved();
                }
                else
                {
                    Debug.LogError("CSJM_Unity " +
                        "The onAdRemoved can not find the context.");
                }
            }, _callbackOnMainThead);
        }

        [AOT.MonoPInvokeCallback(typeof(ExpressAd_DidClose))]
        private static void ExpressAd_OnAdDidCloseMethod(int context)
        {
            //Debug.Log("CSJM_Unity "+"chaors ExpressAd_OnAdDislikeMethod")
            UnityDispatcher.PostTask(() =>
            {
                IExpressBannerInteractionListener listener;
                if (interactionListeners.TryGetValue(context, out listener))
                {
                    listener.OnAdClose();
                }
                else
                {
                    Debug.LogError("CSJM_Unity " +
                        "The ExpressAd_OnAdDidCloseMethod can not find the context.");
                }
            }, _callbackOnMainThead);
        }

        public void setAuctionPrice(double price)
        {
            ClientBidManager.SetAuctionPrice(this.expressAd, price);
        }

        public void win(double price)
        {
            ClientBidManager.Win(this.expressAd, price);
        }

        public void Loss(double price, string reason, string bidder)
        {
            ClientBidManager.Loss(this.expressAd, price, reason, bidder);
        }

        private MediationBannerManager mediationManager;
        public MediationBannerManager GetMediationManager()
        {
            bool haveMediationManager = UnionPlatform_bannerHaveMediationManager(this.expressAd);
            if (haveMediationManager == false)
            {
                return null;
            }
            else
            {
                if (mediationManager == null)
                {
                    mediationManager = new MediationBannerManager(this.expressAd);
                }
                return mediationManager;
            }
        }
    }
#endif
}
