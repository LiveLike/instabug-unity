#import "UnityAppController.h"
#import <Instabug/Instabug.h>

// OverrideAppDelegate is set up based on the following instructions:
//   https://docs.instabug.com/docs/ios-integration
//
@interface OverrideAppDelegate : UnityAppController
@end


IMPL_APP_CONTROLLER_SUBCLASS(OverrideAppDelegate)


@implementation OverrideAppDelegate

NSString *const InstabugBetaToken = @"<BetaToken>";
NSString *const InstabugLiveToken = @"<LiveToken>";

-(BOOL)application:(UIApplication*) application didFinishLaunchingWithOptions:(NSDictionary*) options
{
    [self setupInstabug];

    return [super application:application didFinishLaunchingWithOptions:options];
}

-(void)setupInstabug
{
    if ([self isRunningLive])
    {
        [Instabug startWithToken:InstabugLiveToken invocationEvent:IBGInvocationEventShake];
    }
    else
    {
        [Instabug startWithToken:InstabugBetaToken invocationEvent:IBGInvocationEventShake];
    }
}

-(BOOL)isRunningLive
{
#if TARGET_OS_SIMULATOR
    return NO;
#else
    BOOL isRunningTestFlightBeta = [[[[NSBundle mainBundle] appStoreReceiptURL] lastPathComponent] isEqualToString:@"sandboxReceipt"];
    BOOL hasEmbeddedMobileProvision = !![[NSBundle mainBundle] pathForResource:@"embedded" ofType:@"mobileprovision"];
    if (isRunningTestFlightBeta || hasEmbeddedMobileProvision)
    {
        return NO;
    }
    return YES;
#endif
}

@end