//------------------------------------------------------------------------------
// Copyright (c) 2018-2019 Beijing Bytedance Technology Co., Ltd.
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

#import <BUAdSDK/BURewardedVideoAd.h>
#import <BUAdSDK/BURewardedVideoModel.h>
#import "UnityAppController.h"
#import "AdSlot.h"
#import "BUToUnityAdManager.h"

extern const char* AutonomousStringCopy(const char* string);

const char* AutonomousStringCopy(const char* string)
{
    if (string == NULL) {
        return NULL;
    }
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

// IRewardVideoAdListener callbacks.
typedef void(*RewardVideoAd_OnError)(int code, const char* message, int context);
typedef void(*RewardVideoAd_OnRewardVideoAdLoad)(void* rewardVideoAd, int context);
typedef void(*RewardVideoAd_OnRewardVideoCached)(int context);

// IRewardAdInteractionListener callbacks.
typedef void(*RewardVideoAd_OnAdShow)(int context);
typedef void(*RewardVideoAd_OnAdVideoBarClick)(int context);
typedef void(*RewardVideoAd_OnAdClose)(int context);
typedef void(*RewardVideoAd_OnVideoComplete)(int context);
typedef void(*RewardVideoAd_OnVideoError)(int context);
typedef void(*RewardVideoAd_OnVideoSkip)(int context);
typedef void(*ExpressRewardVideoAd_OnRewardVerify)(bool rewardVerify,
                                                   int rewardAmount,
                                                   const char* rewardName,
                                                   int rewardType,
                                                   float rewardPropose,
                                                   int serverErrorCode,
                                                   const char* serverErrorMsg,
                                                   bool isGromoreServersideVerify,
                                                   const char* gromoreExtra,
                                                   const char* transId,
                                                   int reason,
                                                   int errCode,
                                                   const char* errMsg,
                                                   const char* adnName,
                                                   const char* ecpm,
                                                   int context);

// The BURewardedVideoAdDelegate implement.
@interface RewardVideoAd : NSObject
@end

@interface RewardVideoAd () <BUMRewardedVideoAdDelegate,BUAdObjectProtocol>
@property (nonatomic, strong) BURewardedVideoAd *rewardedVideoAd;

@property (nonatomic, assign) int loadContext;
@property (nonatomic, assign) RewardVideoAd_OnError onError;
@property (nonatomic, assign) RewardVideoAd_OnRewardVideoAdLoad onRewardVideoAdLoad;
@property (nonatomic, assign) RewardVideoAd_OnRewardVideoCached onRewardVideoCached;

@property (nonatomic, assign) int interactionContext;
@property (nonatomic, assign) RewardVideoAd_OnAdShow onAdShow;
@property (nonatomic, assign) RewardVideoAd_OnAdVideoBarClick onAdVideoBarClick;
@property (nonatomic, assign) RewardVideoAd_OnAdClose onAdClose;
@property (nonatomic, assign) RewardVideoAd_OnVideoComplete onVideoComplete;
@property (nonatomic, assign) RewardVideoAd_OnVideoError onVideoError;
@property (nonatomic, assign) RewardVideoAd_OnVideoSkip onVideoSkip;
@property (nonatomic, assign) ExpressRewardVideoAd_OnRewardVerify onRewardVerify;

@property (nonatomic, strong) RewardVideoAd *againObject;

@property (nonatomic, assign) BOOL triggerShow;

@end

@implementation RewardVideoAd
- (void)rewardedVideoAdDidLoad:(BURewardedVideoAd *)rewardedVideoAd {
    if (self.onRewardVideoAdLoad) {
        self.onRewardVideoAdLoad((__bridge void*)self, self.loadContext);
    }
}

- (void)rewardedVideoAdVideoDidLoad:(BURewardedVideoAd *)rewardedVideoAd {
    if (self.onRewardVideoCached) {
        self.onRewardVideoCached(self.loadContext);
    }
}

- (void)rewardedVideoAdWillVisible:(BURewardedVideoAd *)rewardedVideoAd {
    if (self.onAdShow) {
        self.onAdShow(self.interactionContext);
    }
    self.triggerShow = YES;
}

- (void)rewardedVideoAdDidVisible:(BURewardedVideoAd *)rewardedVideoAd {
    if (self.triggerShow == NO) {
        if (self.onAdShow) {
            self.onAdShow(self.interactionContext);
        }
    }
}

- (void)rewardedVideoAdDidClose:(BURewardedVideoAd *)rewardedVideoAd {
    if (self.onAdClose) {
        self.onAdClose(self.interactionContext);
    }
}

- (void)rewardedVideoAdDidClick:(BURewardedVideoAd *)rewardedVideoAd {
    if (self.onAdVideoBarClick) {
        self.onAdVideoBarClick(self.interactionContext);
    }
}

- (void)rewardedVideoAd:(BURewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *)error {
    if (self.onError) {
        self.onError((int)error.code, AutonomousStringCopy([[error localizedDescription] UTF8String]), self.loadContext);
    }
}

- (void)rewardedVideoAdDidClickSkip:(BURewardedVideoAd *)rewardedVideoAd {
    if (self.onVideoSkip) {
        self.onVideoSkip(self.interactionContext);
    }
}

- (void)rewardedVideoAdDidPlayFinish:(BURewardedVideoAd *)rewardedVideoAd didFailWithError:(NSError *)error {
    if (error) {
        if (self.onVideoError) {
            self.onVideoError(self.interactionContext);
        }
    } else {
        if (self.onVideoComplete) {
            self.onVideoComplete(self.interactionContext);
        }
    }
}
//typedef void(*ExpressRewardVideoAd_OnRewardVerify)(bool rewardVerify,
//                                                   int rewardAmount,
//                                                   const char* rewardName,
//                                                   int rewardType,
//                                                   float rewardPropose,
//                                                   int serverErrorCode,
//                                                   const char* serverErrorMsg,
//                                                   bool isGromoreServersideVerify,
//                                                   const char* gromoreExtra,
//                                                   const char* transId,
//                                                   int reason,
//                                                   int errCode,
//                                                   const char* errMsg,
//                                                   const char* adnName,
//                                                   int context);
- (void)rewardedVideoAdServerRewardDidFail:(BURewardedVideoAd *)rewardedVideoAd error:(NSError *)error {
    if (self.onRewardVerify) {
        NSString *rewardName = rewardedVideoAd.rewardedVideoModel.rewardName?:@"";
        NSInteger serverErrorCode = error ? error.code : 0;
        NSString *serverErrorMsg = error ? error.userInfo[NSLocalizedDescriptionKey] : @"";
        BOOL isGromoreServersideVerify = rewardedVideoAd.rewardedVideoModel.mediation ? rewardedVideoAd.rewardedVideoModel.mediation.verifyByGroMoreS2S : NO;
        NSString *transId = rewardedVideoAd.rewardedVideoModel.mediation ? rewardedVideoAd.rewardedVideoModel.mediation.tradeId : @"";
        NSString *adnName = rewardedVideoAd.rewardedVideoModel.mediation ? rewardedVideoAd.rewardedVideoModel.mediation.adnName : @"";
        NSString *extra = rewardedVideoAd.rewardedVideoModel.extra?:@"";
        NSString *ecpm = rewardedVideoAd.rewardedVideoModel.mediation ? rewardedVideoAd.rewardedVideoModel.mediation.ecpm : nil;
        self.onRewardVerify(false,
                            (int)rewardedVideoAd.rewardedVideoModel.rewardAmount,
                            (char*)[rewardName cStringUsingEncoding:NSUTF8StringEncoding],
                            (int)rewardedVideoAd.rewardedVideoModel.rewardType,
                            rewardedVideoAd.rewardedVideoModel.rewardPropose,
                            (int)serverErrorCode,
                            (char*)[serverErrorMsg cStringUsingEncoding:NSUTF8StringEncoding],
                            isGromoreServersideVerify,
                            (char*)[extra cStringUsingEncoding:NSUTF8StringEncoding],
                            (char*)[transId cStringUsingEncoding:NSUTF8StringEncoding],
                            0,
                            (int)serverErrorCode,
                            (char*)[serverErrorMsg cStringUsingEncoding:NSUTF8StringEncoding],
                            (char*)[adnName cStringUsingEncoding:NSUTF8StringEncoding],
                            (char*)[ecpm cStringUsingEncoding:NSUTF8StringEncoding],
                            self.interactionContext);
    }
}

- (void)rewardedVideoAdServerRewardDidSucceed:(BURewardedVideoAd *)rewardedVideoAd verify:(BOOL)verify {
    if (self.onRewardVerify) {
        NSString *rewardName = rewardedVideoAd.rewardedVideoModel.rewardName?:@"";
        BOOL isGromoreServersideVerify = rewardedVideoAd.rewardedVideoModel.mediation ? rewardedVideoAd.rewardedVideoModel.mediation.verifyByGroMoreS2S : NO;
        NSString *transId = rewardedVideoAd.rewardedVideoModel.mediation ? rewardedVideoAd.rewardedVideoModel.mediation.tradeId : nil;
        NSString *adnName = rewardedVideoAd.rewardedVideoModel.mediation ? rewardedVideoAd.rewardedVideoModel.mediation.adnName : nil;
        NSString *ecpm = rewardedVideoAd.rewardedVideoModel.mediation ? rewardedVideoAd.rewardedVideoModel.mediation.ecpm : nil;
        self.onRewardVerify(verify,
                            (int)rewardedVideoAd.rewardedVideoModel.rewardAmount,
                            (char*)[rewardName cStringUsingEncoding:NSUTF8StringEncoding],
                            (int)rewardedVideoAd.rewardedVideoModel.rewardType,
                            rewardedVideoAd.rewardedVideoModel.rewardPropose,
                            0,
                            (char*)[@"" cStringUsingEncoding:NSUTF8StringEncoding],
                            isGromoreServersideVerify,
                            (char*)[@"" cStringUsingEncoding:NSUTF8StringEncoding],
                            (char*)[transId cStringUsingEncoding:NSUTF8StringEncoding],
                            0,
                            0,
                            (char*)[@"" cStringUsingEncoding:NSUTF8StringEncoding],
                            (char*)[adnName cStringUsingEncoding:NSUTF8StringEncoding],
                            (char*)[ecpm cStringUsingEncoding:NSUTF8StringEncoding],
                            self.interactionContext);
    }
}

/// 广告展示失败回调
/// @param rewardedVideoAd 广告管理对象
/// @param error 展示失败的原因
- (void)rewardedVideoAdDidShowFailed:(BURewardedVideoAd *_Nonnull)rewardedVideoAd error:(NSError *_Nonnull)error {
    
}

- (id<BUAdClientBiddingProtocol>)adObject {
    return self.rewardedVideoAd;
}
@end

#if defined (__cplusplus)
extern "C" {
#endif

void UnionPlatform_RewardVideoAd_Load(
                                      AdSlotStruct *adSlot,
                                      RewardVideoAd_OnError onError,
                                      RewardVideoAd_OnRewardVideoAdLoad onRewardVideoAdLoad,
                                      RewardVideoAd_OnRewardVideoCached onRewardVideoCached,
                                      int context) {
    BURewardedVideoModel *model = [[BURewardedVideoModel alloc] init];
    model.userId = [[NSString alloc] initWithCString:adSlot->userId encoding:NSUTF8StringEncoding];
    model.extra =  [[NSString alloc] initWithCString:adSlot->mediaExtra encoding:NSUTF8StringEncoding];
    model.rewardName =  [[NSString alloc] initWithCString:adSlot->rewardName encoding:NSUTF8StringEncoding];
    model.rewardAmount = adSlot->rewardAmount;
    
    BUAdSlot *slot1 = [[BUAdSlot alloc] init];
    slot1.ID = [[NSString alloc] initWithCString:adSlot->slotId encoding:NSUTF8StringEncoding];
    if (adSlot->viewWidth > 0 && adSlot->viewHeight > 0) {
        CGSize adSize = CGSizeMake(adSlot->viewWidth, adSlot->viewHeight);
        slot1.adSize = adSize;
    }
    slot1.mediation.bidNotify = adSlot->m_bidNotify;
    slot1.mediation.scenarioID = [[NSString alloc] initWithCString:adSlot->m_cenarioId encoding:NSUTF8StringEncoding];
    slot1.mediation.mutedIfCan = adSlot->m_isMuted;
    
    BURewardedVideoAd* rewardedVideoAd = [[BURewardedVideoAd alloc] initWithSlot:slot1 rewardedVideoModel:model];
    
    RewardVideoAd* instance = [[RewardVideoAd alloc] init];
    RewardVideoAd* again_instance = [[RewardVideoAd alloc] init];
    
    instance.rewardedVideoAd = rewardedVideoAd;
    instance.againObject = again_instance;
    
    instance.onError = onError;
    instance.onRewardVideoAdLoad = onRewardVideoAdLoad;
    instance.onRewardVideoCached = onRewardVideoCached;
    
    instance.loadContext = context;
    again_instance.loadContext = context;
    
    rewardedVideoAd.delegate = instance;
    rewardedVideoAd.rewardPlayAgainInteractionDelegate = again_instance;
    [rewardedVideoAd loadAdData];
    
    NSLog(@"CSJM_Unity  激励视屏 设置 adSlot，其中 %@, %@, %@,  %@, %@, %@",
          [NSString stringWithFormat:@"slotId-%@ ，",slot1.ID],
          [NSString stringWithFormat:@"bidNotify-%d ，",slot1.mediation.bidNotify],
          [NSString stringWithFormat:@"scenarioID-%@ ，",slot1.mediation.scenarioID],
          [NSString stringWithFormat:@"mutedIfCan-%d ，",slot1.mediation.mutedIfCan],
          [NSString stringWithFormat:@"adSize-%d-%d ，",adSlot->viewWidth,adSlot->viewHeight],
          [NSString stringWithFormat:@"rewardedVideoModel userId-%@ extra-%@ rewardName-%@ rewardAmount-%ld，",model.userId, model.extra, model.rewardName, (long)model.rewardAmount]
          );
    
    (__bridge_retained void*)instance;
}

void UnionPlatform_RewardVideoAd_SetInteractionListener(
                                                        void* rewardedVideoAdPtr,
                                                        RewardVideoAd_OnAdShow onAdShow,
                                                        RewardVideoAd_OnAdVideoBarClick onAdVideoBarClick,
                                                        RewardVideoAd_OnAdClose onAdClose,
                                                        RewardVideoAd_OnVideoComplete onVideoComplete,
                                                        RewardVideoAd_OnVideoError onVideoError,
                                                        RewardVideoAd_OnVideoSkip onVideoSkip,
                                                        ExpressRewardVideoAd_OnRewardVerify onRewardVerify,
                                                        int context) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    rewardedVideoAd.onAdShow = onAdShow;
    rewardedVideoAd.onAdVideoBarClick = onAdVideoBarClick;
    rewardedVideoAd.onAdClose = onAdClose;
    rewardedVideoAd.onVideoComplete = onVideoComplete;
    rewardedVideoAd.onVideoError = onVideoError;
    rewardedVideoAd.onVideoSkip = onVideoSkip;
    rewardedVideoAd.onRewardVerify = onRewardVerify;
    rewardedVideoAd.interactionContext = context;
}

void UnionPlatform_RewardVideoAd_ShowRewardVideoAd(void* rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    [rewardedVideoAd.rewardedVideoAd showAdFromRootViewController:GetAppController().rootViewController];
}

void UnionPlatform_RewardVideoAd_Dispose(void* rewardedVideoAdPtr) {
    (__bridge_transfer RewardVideoAd*)rewardedVideoAdPtr;
}

void UnionPlatform_RewardVideoAd_Again_SetInteractionListener(
                                                              void* rewardedVideoAdPtr,
                                                              RewardVideoAd_OnAdShow onAdShow,
                                                              RewardVideoAd_OnAdVideoBarClick onAdVideoBarClick,
                                                              RewardVideoAd_OnVideoComplete onVideoComplete,
                                                              RewardVideoAd_OnVideoError onVideoError,
                                                              RewardVideoAd_OnVideoSkip onVideoSkip,
                                                              ExpressRewardVideoAd_OnRewardVerify onRewardVerify,
                                                              int context) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    if (rewardedVideoAd.againObject) {
        rewardedVideoAd.againObject.onAdShow = onAdShow;
        rewardedVideoAd.againObject.onAdVideoBarClick = onAdVideoBarClick;
        rewardedVideoAd.againObject.onVideoComplete = onVideoComplete;
        rewardedVideoAd.againObject.onVideoError = onVideoError;
        rewardedVideoAd.againObject.onVideoSkip = onVideoSkip;
        rewardedVideoAd.againObject.onRewardVerify = onRewardVerify;
        rewardedVideoAd.againObject.interactionContext = context;
    }
}

bool UnionPlatform_rewardVideoMaterialMetaIsFromPreload(void* rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    BOOL preload = [rewardedVideoAd.rewardedVideoAd materialMetaIsFromPreload];
    return preload == YES;
}

long UnionPlatform_rewardVideoExpireTime(void * rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    return [rewardedVideoAd.rewardedVideoAd getExpireTimestamp];
}

bool UnionPlatform_rewardVideoHaveMediationManager(void * rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    return rewardedVideoAd.rewardedVideoAd.mediation ? true : false;
}

bool UnionPlatform_rewardVideoMediationisReady(void * rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    if (rewardedVideoAd.rewardedVideoAd.mediation) {
        return rewardedVideoAd.rewardedVideoAd.mediation.isReady;
    } else {
        return false;
    }
}

const char* UnionPlatform_rewardVideoMediationGetShowEcpmInfo(void * rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    BUMRitInfo *info = rewardedVideoAd.rewardedVideoAd.mediation.getShowEcpmInfo;
    if (info) {
        NSString *infoJson = [BUMRitInfo toJson:info];
        return AutonomousStringCopy([infoJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_rewardVideoMediationGetCurrentBestEcpmInfo(void * rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    BUMRitInfo *info = rewardedVideoAd.rewardedVideoAd.mediation.getCurrentBestEcpmInfo;
    if (info) {
        NSString *infoJson = [BUMRitInfo toJson:info];
        return AutonomousStringCopy([infoJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_rewardVideoMediationMultiBiddingEcpmInfos(void * rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    NSArray<BUMRitInfo *> * infos = rewardedVideoAd.rewardedVideoAd.mediation.multiBiddingEcpmInfos;
    if (infos && infos.count > 0) {
        NSMutableArray *muArr = [[NSMutableArray alloc]initWithCapacity:infos.count];
        for (BUMRitInfo *info in infos) {
            NSString *infoJson = [BUMRitInfo toJson:info];
            [muArr addObject:infoJson];
        }
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:muArr options:NSJSONWritingPrettyPrinted error:nil];
        NSString *strJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return AutonomousStringCopy([strJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_rewardVideoMediationCacheRitList(void * rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    NSArray<BUMRitInfo *> * infos = rewardedVideoAd.rewardedVideoAd.mediation.cacheRitList;
    if (infos && infos.count > 0) {
        NSMutableArray *muArr = [[NSMutableArray alloc]initWithCapacity:infos.count];
        for (BUMRitInfo *info in infos) {
            NSString *infoJson = [BUMRitInfo toJson:info];
            [muArr addObject:infoJson];
        }
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:muArr options:NSJSONWritingPrettyPrinted error:nil];
        NSString *strJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return AutonomousStringCopy([strJson UTF8String]);
    } else {
        return NULL;
    }
}

const char* UnionPlatform_rewardVideoMediationGetAdLoadInfoList(void * rewardedVideoAdPtr) {
    RewardVideoAd* rewardedVideoAd = (__bridge RewardVideoAd*)rewardedVideoAdPtr;
    NSArray<BUMAdLoadInfo *> * infos = rewardedVideoAd.rewardedVideoAd.mediation.getAdLoadInfoList;
    if (infos && infos.count > 0) {
        NSMutableArray *muArr = [[NSMutableArray alloc]initWithCapacity:infos.count];
        for (BUMAdLoadInfo *info in infos) {
            NSString *infoJson = [BUMAdLoadInfo toJson:info];
            [muArr addObject:infoJson];
        }
        NSData *jsonData = [NSJSONSerialization dataWithJSONObject:muArr options:NSJSONWritingPrettyPrinted error:nil];
        NSString *strJson = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return AutonomousStringCopy([strJson UTF8String]);
    } else {
        return NULL;
    }
}

#if defined (__cplusplus)
}
#endif
