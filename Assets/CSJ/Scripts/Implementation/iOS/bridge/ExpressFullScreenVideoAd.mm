//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

#import <BUAdSDK/BUAdSDK.h>
#import "UnityAppController.h"
#import "AdSlot.h"
#import "BUToUnityAdManager.h"

static const char* AutonomousStringCopy_ExpressFullscreen(const char* string)
{
    if (string == NULL) {
        return NULL;
    }
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

// Load Status
typedef void(*FullScreenVideoAd_OnError)(int code, const char* message, int context);
typedef void(*FullScreenVideoAd_OnFullScreenVideoAdLoad)(void* fullScreenVideoAd, int context);
typedef void(*FullScreenVideoAd_OnFullScreenVideoCached)(int context);

// InteractionListener callbacks.
typedef void(*FullScreenVideoAd_OnAdShow)(int context);
typedef void(*FullScreenVideoAd_OnAdVideoBarClick)(int context);
typedef void(*FullScreenVideoAd_OnAdVideoClickSkip)(int context);
typedef void(*FullScreenVideoAd_OnAdClose)(int context);
typedef void(*FullScreenVideoAd_OnVideoComplete)(int context);
typedef void(*FullScreenVideoAd_OnVideoError)(int context);

//
@interface ExpressFullScreenVideoAd : NSObject
@end

@interface ExpressFullScreenVideoAd () <BUNativeExpressFullscreenVideoAdDelegate,BUAdObjectProtocol>
@property (nonatomic, strong) BUNativeExpressFullscreenVideoAd *fullScreenVideoAd;

@property (nonatomic, assign) int loadContext;
@property (nonatomic, assign) FullScreenVideoAd_OnError onError;
@property (nonatomic, assign) FullScreenVideoAd_OnFullScreenVideoAdLoad onFullScreenVideoAdLoad;
@property (nonatomic, assign) FullScreenVideoAd_OnFullScreenVideoCached onFullScreenVideoCached;

@property (nonatomic, assign) int interactionContext;
@property (nonatomic, assign) FullScreenVideoAd_OnAdShow onAdShow;
@property (nonatomic, assign) FullScreenVideoAd_OnAdVideoBarClick onAdVideoBarClick;
@property (nonatomic, assign) FullScreenVideoAd_OnAdVideoClickSkip onAdVideoClickSkip;
@property (nonatomic, assign) FullScreenVideoAd_OnAdClose onAdClose;
@property (nonatomic, assign) FullScreenVideoAd_OnVideoComplete onVideoComplete;
@property (nonatomic, assign) FullScreenVideoAd_OnVideoError onVideoError;



@end

@implementation ExpressFullScreenVideoAd

#pragma mark - BUFullscreenVideoAdDelegate
- (void)nativeExpressFullscreenVideoAdDidLoad:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    self.onFullScreenVideoAdLoad((__bridge void*)self, self.loadContext);
}

- (void)nativeExpressFullscreenVideoAd:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd didFailWithError:(NSError *_Nullable)error {
    self.onError((int)error.code, AutonomousStringCopy_ExpressFullscreen([[error localizedDescription] UTF8String]), self.loadContext);
}

- (void)nativeExpressFullscreenVideoAdViewRenderSuccess:(BUNativeExpressFullscreenVideoAd *)rewardedVideoAd {
}

- (void)nativeExpressFullscreenVideoAdViewRenderFail:(BUNativeExpressFullscreenVideoAd *)rewardedVideoAd error:(NSError *_Nullable)error {
    self.onError((int)error.code, AutonomousStringCopy_ExpressFullscreen([[error localizedDescription] UTF8String]), self.loadContext);
}

- (void)nativeExpressFullscreenVideoAdDidDownLoadVideo:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    self.onFullScreenVideoCached(self.loadContext);
}

- (void)nativeExpressFullscreenVideoAdWillVisible:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    self.onAdShow(self.interactionContext);
}

- (void)nativeExpressFullscreenVideoAdDidVisible:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
}

- (void)nativeExpressFullscreenVideoAdDidClick:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    self.onAdVideoBarClick(self.interactionContext);
}

- (void)nativeExpressFullscreenVideoAdDidClickSkip:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    self.onAdVideoClickSkip(self.interactionContext);
}

- (void)nativeExpressFullscreenVideoAdWillClose:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
}

- (void)nativeExpressFullscreenVideoAdDidClose:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd {
    self.onAdClose(self.interactionContext);
}

- (void)nativeExpressFullscreenVideoAdDidPlayFinish:(BUNativeExpressFullscreenVideoAd *)fullscreenVideoAd didFailWithError:(NSError *_Nullable)error {
    if (error) {
        self.onVideoError(self.interactionContext);
    } else {
        self.onVideoComplete(self.interactionContext);
    }
}
- (id<BUAdClientBiddingProtocol>)adObject {
    return self.fullScreenVideoAd;
}
@end

#if defined (__cplusplus)
extern "C" {
#endif

void UnionPlatform_ExpressFullScreenVideoAd_Load(
    AdSlotStruct *adSlot,
    FullScreenVideoAd_OnError onError,
    FullScreenVideoAd_OnFullScreenVideoAdLoad onFullScreenVideoAdLoad,
    FullScreenVideoAd_OnFullScreenVideoCached onFullScreenVideoCached,
    int context) {

    BUAdSlot *slot1 = [[BUAdSlot alloc] init];
    slot1.ID = [[NSString alloc] initWithCString:adSlot->slotId encoding:NSUTF8StringEncoding];
    if (adSlot->viewWidth > 0 && adSlot->viewHeight > 0) {
        CGSize adSize = CGSizeMake(adSlot->viewWidth, adSlot->viewHeight);
        slot1.adSize = adSize;
    }
    slot1.mediation.bidNotify = adSlot->m_bidNotify;
    slot1.mediation.scenarioID = [[NSString alloc] initWithCString:adSlot->m_cenarioId encoding:NSUTF8StringEncoding];
    slot1.mediation.mutedIfCan = adSlot->m_isMuted;
    BUNativeExpressFullscreenVideoAd* fullScreenVideoAd = [[BUNativeExpressFullscreenVideoAd alloc] initWithSlot:slot1];
                                                           
    ExpressFullScreenVideoAd* instance = [[ExpressFullScreenVideoAd alloc] init];
    instance.fullScreenVideoAd = fullScreenVideoAd;
    instance.onError = onError;
    instance.onFullScreenVideoAdLoad = onFullScreenVideoAdLoad;
    instance.onFullScreenVideoCached = onFullScreenVideoCached;
    instance.loadContext = context;
    fullScreenVideoAd.delegate = instance;
    [fullScreenVideoAd loadAdData];
    
    NSLog(@"CSJM_Unity  插全屏 设置 adSlot，其中 %@, %@, %@,  %@, %@",
          [NSString stringWithFormat:@"slotId-%@ ，",slot1.ID],
          [NSString stringWithFormat:@"bidNotify-%d ，",slot1.mediation.bidNotify],
          [NSString stringWithFormat:@"scenarioID-%@ ，",slot1.mediation.scenarioID],
          [NSString stringWithFormat:@"mutedIfCan-%d ，",slot1.mediation.mutedIfCan],
          [NSString stringWithFormat:@"adSize-%d-%d ，",adSlot->viewWidth, adSlot->viewHeight]
          );

    (__bridge_retained void*)instance;
}

void UnionPlatform_ExpressFullScreenVideoAd_SetInteractionListener(
    void* fullScreenVideoAdPtr,
    FullScreenVideoAd_OnAdShow onAdShow,
    FullScreenVideoAd_OnAdVideoBarClick onAdVideoBarClick,
    FullScreenVideoAd_OnAdVideoClickSkip onAdVideoClickSkip,
    FullScreenVideoAd_OnAdClose onAdClose,
    FullScreenVideoAd_OnVideoComplete onVideoComplete,
    FullScreenVideoAd_OnVideoError onVideoError,
    int context) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullScreenVideoAdPtr;
    fullScreenVideoAd.onAdShow = onAdShow;
    fullScreenVideoAd.onAdVideoBarClick = onAdVideoBarClick;
    fullScreenVideoAd.onAdVideoClickSkip = onAdVideoClickSkip;
    fullScreenVideoAd.onAdClose = onAdClose;
    fullScreenVideoAd.onVideoComplete = onVideoComplete;
    fullScreenVideoAd.onVideoError = onVideoError;
    fullScreenVideoAd.interactionContext = context;
}

void UnionPlatform_ExpressFullScreenVideoAd_ShowFullScreenVideoAd(void* fullscreenVideoAdPtr) {
    ExpressFullScreenVideoAd* fullscreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenVideoAdPtr;
    [fullscreenVideoAd.fullScreenVideoAd showAdFromRootViewController:GetAppController().rootViewController];}

void UnionPlatform_ExpressFullScreenVideoAd_Dispose(void* fullscreenVideoAdPtr) {
    (__bridge_transfer ExpressFullScreenVideoAd*)fullscreenVideoAdPtr;
}

bool UnionPlatform_expressFullScreenVideoMaterialMetaIsFromPreload(void* fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    BOOL preload = [fullScreenVideoAd.fullScreenVideoAd materialMetaIsFromPreload];
    return preload == YES;
}

long UnionPlatform_expressFullScreenVideoExpireTime(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    return [fullScreenVideoAd.fullScreenVideoAd getExpireTimestamp];
}

bool UnionPlatform_ExpressFullScreenVideoAdHaveMediationManager(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    return fullScreenVideoAd.fullScreenVideoAd.mediation ? true : false;
}

bool UnionPlatform_ExpressFullScreenVideoAdMediationisReady(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    if (fullScreenVideoAd.fullScreenVideoAd.mediation) {
        return fullScreenVideoAd.fullScreenVideoAd.mediation.isReady;
    } else {
        return false;
    }
}

const char* UnionPlatform_ExpressFullScreenVideoAdMediationGetShowEcpmInfo(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    BUMRitInfo *info = fullScreenVideoAd.fullScreenVideoAd.mediation.getShowEcpmInfo;
    if (info) {
        NSString *infoJson = [BUMRitInfo toJson:info];
        return AutonomousStringCopy_ExpressFullscreen([infoJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_ExpressFullScreenVideoAdMediationGetCurrentBestEcpmInfo(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    BUMRitInfo *info = fullScreenVideoAd.fullScreenVideoAd.mediation.getCurrentBestEcpmInfo;
    if (info) {
        NSString *infoJson = [BUMRitInfo toJson:info];
        return AutonomousStringCopy_ExpressFullscreen([infoJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_ExpressFullScreenVideoAdMediationMultiBiddingEcpmInfos(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    NSArray<BUMRitInfo *> * infos = fullScreenVideoAd.fullScreenVideoAd.mediation.multiBiddingEcpmInfos;
    if (infos && infos.count > 0) {
        NSMutableArray *muArr = [[NSMutableArray alloc]initWithCapacity:infos.count];
        for (BUMRitInfo *info in infos) {
            NSString *infoJson = [BUMRitInfo toJson:info];
            [muArr addObject:infoJson];
        }
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:muArr options:NSJSONWritingPrettyPrinted error:nil];
        NSString *strJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return AutonomousStringCopy_ExpressFullscreen([strJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_ExpressFullScreenVideoAdMediationCacheRitList(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    NSArray<BUMRitInfo *> * infos = fullScreenVideoAd.fullScreenVideoAd.mediation.cacheRitList;
    if (infos && infos.count > 0) {
        NSMutableArray *muArr = [[NSMutableArray alloc]initWithCapacity:infos.count];
        for (BUMRitInfo *info in infos) {
            NSString *infoJson = [BUMRitInfo toJson:info];
            [muArr addObject:infoJson];
        }
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:muArr options:NSJSONWritingPrettyPrinted error:nil];
        NSString *strJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return AutonomousStringCopy_ExpressFullscreen([strJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_ExpressFullScreenVideoAdMediationGetAdLoadInfoList(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    NSArray<BUMAdLoadInfo *> * infos = fullScreenVideoAd.fullScreenVideoAd.mediation.getAdLoadInfoList;
    if (infos && infos.count > 0) {
        NSMutableArray *muArr = [[NSMutableArray alloc]initWithCapacity:infos.count];
        for (BUMAdLoadInfo *info in infos) {
            NSString *infoJson = [BUMAdLoadInfo toJson:info];
            [muArr addObject:infoJson];
        }
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:muArr options:NSJSONWritingPrettyPrinted error:nil];
        NSString *strJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return AutonomousStringCopy_ExpressFullscreen([strJson UTF8String]);
    } else {
        return NULL;
    }
}


bool UnionPlatform_expressFullScreenVideoHaveMediationManager(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    return fullScreenVideoAd.fullScreenVideoAd.mediation ? true : false;
}

bool UnionPlatform_expressFullScreenMediationisReady(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    if (fullScreenVideoAd.fullScreenVideoAd.mediation) {
        return fullScreenVideoAd.fullScreenVideoAd.mediation.isReady;
    } else {
        return false;
    }
}

const char* UnionPlatform_expressFullScreenMediationGetShowEcpmInfo(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    BUMRitInfo *info = fullScreenVideoAd.fullScreenVideoAd.mediation.getShowEcpmInfo;
    if (info) {
        NSString *infoJson = [BUMRitInfo toJson:info];
        return AutonomousStringCopy_ExpressFullscreen([infoJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_expressFullScreenMediationGetCurrentBestEcpmInfo(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    BUMRitInfo *info = fullScreenVideoAd.fullScreenVideoAd.mediation.getCurrentBestEcpmInfo;
    if (info) {
        NSString *infoJson = [BUMRitInfo toJson:info];
        return AutonomousStringCopy_ExpressFullscreen([infoJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_expressFullScreenMediationMultiBiddingEcpmInfos(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    NSArray<BUMRitInfo *> * infos = fullScreenVideoAd.fullScreenVideoAd.mediation.multiBiddingEcpmInfos;
    if (infos && infos.count > 0) {
        NSMutableArray *muArr = [[NSMutableArray alloc]initWithCapacity:infos.count];
        for (BUMRitInfo *info in infos) {
            NSString *infoJson = [BUMRitInfo toJson:info];
            [muArr addObject:infoJson];
        }
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:muArr options:NSJSONWritingPrettyPrinted error:nil];
        NSString *strJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return AutonomousStringCopy_ExpressFullscreen([strJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_expressFullScreenMediationCacheRitList(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    NSArray<BUMRitInfo *> * infos = fullScreenVideoAd.fullScreenVideoAd.mediation.cacheRitList;
    if (infos && infos.count > 0) {
        NSMutableArray *muArr = [[NSMutableArray alloc]initWithCapacity:infos.count];
        for (BUMRitInfo *info in infos) {
            NSString *infoJson = [BUMRitInfo toJson:info];
            [muArr addObject:infoJson];
        }
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:muArr options:NSJSONWritingPrettyPrinted error:nil];
        NSString *strJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return AutonomousStringCopy_ExpressFullscreen([strJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_expressFullScreenMediationGetAdLoadInfoList(void * fullscreenAd) {
    ExpressFullScreenVideoAd* fullScreenVideoAd = (__bridge ExpressFullScreenVideoAd*)fullscreenAd;
    NSArray<BUMAdLoadInfo *> * infos = fullScreenVideoAd.fullScreenVideoAd.mediation.getAdLoadInfoList;
    if (infos && infos.count > 0) {
        NSMutableArray *muArr = [[NSMutableArray alloc]initWithCapacity:infos.count];
        for (BUMAdLoadInfo *info in infos) {
            NSString *infoJson = [BUMAdLoadInfo toJson:info];
            [muArr addObject:infoJson];
        }
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:muArr options:NSJSONWritingPrettyPrinted error:nil];
        NSString *strJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return AutonomousStringCopy_ExpressFullscreen([strJson UTF8String]);
    } else {
        return NULL;
    }
}


#if defined (__cplusplus)
}
#endif
