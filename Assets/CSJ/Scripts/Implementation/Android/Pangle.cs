using System.Collections.Generic;
using ByteDance.Union.Mediation;

namespace ByteDance.Union
{
#if !UNITY_EDITOR && UNITY_ANDROID
    using UnityEngine;

    public class Pangle
    {
        public delegate void PangleInitializeCallBack(bool success, string message);

        private static AndroidJavaObject activity;

        public static void Init(SDKConfiguration sdkConfiguration)
        {
            Debug.Log("CSJM_Unity "+"Pangle Init" );
            var activity = GetActivity();
            var runnable = new AndroidJavaRunnable(() => initTTSdk(sdkConfiguration));
            activity.Call("runOnUiThread", runnable);
        }

        private static void initTTSdk(SDKConfiguration sdkConfiguration)
        {
            Debug.Log("CSJM_Unity "+"Pangle initTTSdk " );
            AndroidJavaObject adConfigBuilder = new AndroidJavaObject("com.bytedance.sdk.openadsdk.TTAdConfig$Builder");
            Debug.Log("CSJM_Unity "+"Pangle InitializeSDK 开始设置config");
            adConfigBuilder.Call<AndroidJavaObject>("appId", sdkConfiguration.appId)
                .Call<AndroidJavaObject>("useTextureView", sdkConfiguration.useTextureView)
                .Call<AndroidJavaObject>("appName", sdkConfiguration.appName)
                .Call<AndroidJavaObject>("allowShowNotify", sdkConfiguration.allowShowNotify)
                .Call<AndroidJavaObject>("debug", sdkConfiguration.debug)
                .Call<AndroidJavaObject>("directDownloadNetworkType", sdkConfiguration.directDownloadNetworkType)
                .Call<AndroidJavaObject>("themeStatus", sdkConfiguration.themeStatus)
                .Call<AndroidJavaObject>("supportMultiProcess", sdkConfiguration.supportMultiProcess)
                .Call<AndroidJavaObject>("useMediation", sdkConfiguration.useMediation)
                .Call<AndroidJavaObject>("data",
                    "[{\"name\":\"unity_version\",\"value\":\"" + AdConst.PangleSdkVersion + "\"}]"); // unity版本号
            
            PrivacyConfiguration configuration = sdkConfiguration.privacyConfiguration;
            if (configuration != null)
            {
                adConfigBuilder.Call<AndroidJavaObject>("customController",
                    Utils.MakeCustomController(configuration));
            }

            MediationConfig mediationConfig = sdkConfiguration.mediationConfig;
            if (mediationConfig != null)
            {
                adConfigBuilder.Call<AndroidJavaObject>("setMediationConfig",
                    Utils.MakeMediationConfig(mediationConfig));
            }
            AndroidJavaObject adConfig = adConfigBuilder.Call<AndroidJavaObject>("build");
            var jc = new AndroidJavaClass("com.bytedance.sdk.openadsdk.TTAdSdk");
            jc.CallStatic("init", GetActivity(), adConfig);
        }

        public static void Start(PangleInitializeCallBack callback)
        {
            var activity = GetActivity();
            var runnable = new AndroidJavaRunnable(() =>
            {
                var sdkInitCallback = new SdkInitCallback(callback);
                var jc = new AndroidJavaClass("com.bytedance.sdk.openadsdk.TTAdSdk");
                jc.CallStatic("start", sdkInitCallback);
            });
            activity.Call("runOnUiThread", runnable);
        }
        
        /// <summary>
        /// Gets the unity main activity.
        /// </summary>
        private static AndroidJavaObject GetActivity()
        {
            if (activity == null)
            {
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
            return activity;
        }
        
        private sealed class SdkInitCallback : AndroidJavaProxy
        {
            private readonly PangleInitializeCallBack listener;

            public SdkInitCallback(
                PangleInitializeCallBack listener)
                : base("com.bytedance.sdk.openadsdk.TTAdSdk$Callback")
            {
                this.listener = listener;
            }

            public void fail(int code, string message)
            {
                listener(false, message);
            }

            public void success()
            {
                listener(true, "sdk 初始化成功");
            }
        }

    }
#endif
}
