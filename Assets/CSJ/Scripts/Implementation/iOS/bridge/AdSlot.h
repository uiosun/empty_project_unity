//
//  AdSlot.h
//  
//
//  Created by yujie on 2021/9/13.
//
#import "BUAdObjectProtocol.h"
#include <stdio.h>
struct AdSlotStruct {
    char slotId[512];
    int adCount;
    int width;
    int height;
    int adType;
    int adLoadType;
    int intervalTime;
    int viewHeight;
    int viewWidth;
    char mediaExtra[1024];
    char userId[512];
    int rewardAmount;
    char rewardName[512];
    int m_bidNotify;
    char m_cenarioId[1024];
    int m_isMuted;
    char m_s_AdnName[512];
    char m_s_AdnSlotId[512];
    char m_s_AppId[512];
    char m_s_Appkey[512];
};

static BOOL SupportSplashZoomout = NO;

