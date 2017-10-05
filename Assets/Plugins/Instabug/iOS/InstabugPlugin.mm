#import <Instabug/Instabug.h>

static bool isRunningLive() {
#if TARGET_OS_SIMULATOR
    return NO;
#else
    BOOL isRunningTestFlightBeta = [[[[NSBundle mainBundle] appStoreReceiptURL] lastPathComponent] isEqualToString:@"sandboxReceipt"];
    BOOL hasEmbeddedMobileProvision = !![[NSBundle mainBundle] pathForResource:@"embedded" ofType:@"mobileprovision"];
    if (isRunningTestFlightBeta || hasEmbeddedMobileProvision) {
        return NO;
    }
    return YES;
#endif
}

extern "C" void initInstabug(const char* betaToken, const char* liveToken) {
    
    NSString *nsBetaToken = [NSString stringWithUTF8String:betaToken];
    NSString *nsLiveToken = [NSString stringWithUTF8String:liveToken];
    
    if (isRunningLive()) {
        [Instabug startWithToken:nsLiveToken invocationEvent:IBGInvocationEventShake];
    } else {
        [Instabug startWithToken:nsBetaToken invocationEvent:IBGInvocationEventShake];
    }
}
