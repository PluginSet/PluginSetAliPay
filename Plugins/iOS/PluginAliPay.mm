#import <AlipaySDK/AlipaySDK.h>

#import "AliPayApiManager.h"

#import "PluginListener.h"
#import "PluginUtils.h"

#import "NSDictionary+SafeUseDictionary.h"

@interface PluginAliPay : NSObject<PluginListener>
@end

REGISTER_PLUGIN_LISTENER(PluginAliPay)

@implementation PluginAliPay

#pragma mark PluginListener
- (void) onOpenURL:(NSNotification *)notification
{
    NSDictionary* notifData = notification.userInfo;
    if (![notifData hasKey:@"url"]) return;
    NSURL * url = notifData[@"url"];
    if ([url.host isEqualToString:@"safepay"])
    {
        //跳转支付宝客户端进行支付，处理支付结果
        [[AlipaySDK defaultService] processOrderWithPaymentResult:url standbyCallback:^(NSDictionary *resultDic) {
            NSLog(@"result = %@",resultDic);
        }];
        
        [[AlipaySDK defaultService] processAuth_V2Result:url standbyCallback:^(NSDictionary *resultDic) {
            NSLog(@"result = %@",resultDic);
        }];
    }
    
}

-(void)OnLoginFail:(const char*) objName
{
    NSDictionary* receiveMap = @{@"status":@1};
    NSString* jsonStr = [PluginUtils dictToJson:receiveMap];
    UnitySendMessage(objName, "onAliPayLoginRet", [jsonStr UTF8String]);
}

#if defined(__cplusplus)
extern "C"
{
#endif
    //是否使用微信登陆
    bool IsAlipayInstalled()
    {
        return [AliPayApiManager IsAliPayInstalled];
    }
    
    //发起登陆
    void LoginAli(const char* objName, const char* scheme, const char* autoInfo)
    {
        NSLog(@"called alipay login");

        //判断用户是否已安装微信App
        if (IsAlipayInstalled()) {
            [AliPayApiManager Login:objName scheme:scheme authUrl:autoInfo];
        } else {
            NSLog(@"alipay is not installed, cant login");
            [[PluginAliPay GetInstance] OnLoginFail:objName];
        }
    }
    
    void Pay(const char* orderInfo, const char* scheme)
    {
    
    }
    
#if defined(__cplusplus)
}
#endif


@end
