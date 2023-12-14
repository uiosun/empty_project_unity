// ------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
// ------------------------------------------------------------------------------

#if !UNITY_EDITOR && UNITY_IOS

namespace ByteDance.Union.Mediation
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Newtonsoft.Json;
    using UnityEngine;

    public sealed class MediationFullScreenManager : IDisposable
    {
        private IntPtr ad;
        internal MediationFullScreenManager(IntPtr ad)
        {
            this.ad = ad;
        }

        public void Dispose()
        {
            
        }

        bool isReady()
        {
            return UnionPlatform_expressFullScreenMediationisReady(ad);
        }

        public List<MediationAdLoadInfo> GetAdLoadInfo()
        {
            List<MediationAdLoadInfo> infos = new List<MediationAdLoadInfo>();
            string json = UnionPlatform_expressFullScreenMediationGetAdLoadInfoList(ad);
            if (json == null) {
               return infos;
            }
            List<string> listJson = JsonConvert.DeserializeObject<List<string>>(json);
            foreach (var item in listJson)
            {
                // json反序列化
                MediationAdLoadInfo info = JsonConvert.DeserializeObject<MediationAdLoadInfo>(item);
                infos.Add(info);
            }
            return infos;
        }

        public List<MediationAdEcpmInfo> GetCacheList()
        {
            List<MediationAdEcpmInfo> infos = new List<MediationAdEcpmInfo>();
            string json = UnionPlatform_expressFullScreenMediationCacheRitList(ad);
            if (json == null)
            {
                return infos;
            }
            List<string> listJson = JsonConvert.DeserializeObject<List<string>>(json);
            foreach (var item in listJson)
            {
                // json反序列化
                MediationAdEcpmInfo info = JsonConvert.DeserializeObject<MediationAdEcpmInfo>(item);
                infos.Add(info);
            }
            return infos;
        }

        public List<MediationAdEcpmInfo> GetMultiBiddingEcpm()
        {
            List<MediationAdEcpmInfo> infos = new List<MediationAdEcpmInfo>();
            string json = UnionPlatform_expressFullScreenMediationMultiBiddingEcpmInfos(ad);
            if (json == null)
            {
                return infos;
            }
            List<string> listJson = JsonConvert.DeserializeObject<List<string>>(json);
            foreach (var item in listJson)
            {
                // json反序列化
                MediationAdEcpmInfo info = JsonConvert.DeserializeObject<MediationAdEcpmInfo>(item);
                infos.Add(info);
            }
            return infos;
        }

        public MediationAdEcpmInfo GetBestEcpm()
        {
            string json = UnionPlatform_expressFullScreenMediationGetCurrentBestEcpmInfo(ad);
            if (json == null)
            {
                return null;
            }
            // json反序列化
            MediationAdEcpmInfo info = JsonConvert.DeserializeObject<MediationAdEcpmInfo>(json);
            return info;
        }


        public MediationAdEcpmInfo GetShowEcpm()
        {
            string json = UnionPlatform_expressFullScreenMediationGetShowEcpmInfo(ad);
            if (json == null)
            {
                return null;
            }
            // json反序列化
            MediationAdEcpmInfo info = JsonConvert.DeserializeObject<MediationAdEcpmInfo>(json);
            return info;
        }

        public void destroy()
        {
            
        }

        [DllImport("__Internal")]
        private static extern bool UnionPlatform_expressFullScreenMediationisReady(IntPtr ad);
        [DllImport("__Internal")]
        private static extern string UnionPlatform_expressFullScreenMediationGetShowEcpmInfo(IntPtr ad);
        [DllImport("__Internal")]
        private static extern string UnionPlatform_expressFullScreenMediationGetCurrentBestEcpmInfo(IntPtr ad);
        [DllImport("__Internal")]
        private static extern string UnionPlatform_expressFullScreenMediationMultiBiddingEcpmInfos(IntPtr ad);
        [DllImport("__Internal")]
        private static extern string UnionPlatform_expressFullScreenMediationCacheRitList(IntPtr ad);
        [DllImport("__Internal")]
        private static extern string UnionPlatform_expressFullScreenMediationGetAdLoadInfoList(IntPtr ad);
    }
}

#endif