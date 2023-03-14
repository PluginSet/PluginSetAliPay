#import <AlipaySDK/AlipaySDK.h>

#import "PluginListener.h"
#import "PluginUtils.h"

#import "NSDictionary+SafeUseDictionary.h"

@interface PluginAliPay : NSObject<PluginListener, WXApiDelegate>
@property int _loginRequestId;
@end

REGISTER_PLUGIN_LISTENER(PluginAliPay)

NSString* _u3dGameStr;
NSString* _u3dCallStr;
const char* u3dGameObject;
const char* u3dCallback;

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
    }
}

- (void) loginRequest
{
    int requestId = ++self._loginRequestId;
    NSString * requestState = [NSString stringWithFormat:@"request_%d", requestId];
    
    SendAuthReq * request = [SendAuthReq new];
    request.scope = @"snsapi_userinfo";
    request.state = requestState;
    [WXApi sendReq:request completion:^(BOOL success) {
        if (success) {
            NSLog(@"wechat login callback gives success");
        } else {
            NSLog(@"wechat login callback gives failure");
            [self OnCompleteFailCall:@"wechat login callback gives failure"];
        }
    }];
}

-(void)OnCompleteSuccessCall:(NSString*)msg
{
    NSDictionary* receiveMap = @{@"isSuccess":@"true",@"code":msg};
    NSString* jsonStr = [PluginUtils dictToJson:receiveMap];
    const char *p1 = [jsonStr UTF8String];
    [self OnCallUnityC:p1];
}

-(void)OnCompleteFailCall:(NSString*)msg
{
    NSDictionary* receiveMap = @{@"isSuccess":@"false",@"code":msg};
    NSString* jsonStr = [PluginUtils dictToJson:receiveMap];
    const char *p1 = [jsonStr UTF8String];
    [self OnCallUnityC:p1];
}

-(void)OnCallUnityC:(const char*) msg
{
    UnitySendMessage(u3dGameObject,u3dCallback,msg);
}

#if defined(__cplusplus)
extern "C"
{
#endif
    //是否使用微信登陆
    bool IsAppInstalled()
    {
        return [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"alipay://"]];
    }
    
    //发起登陆
    void Login(const char* autoInfo, const char* scheme)
    {
        NSLog(@"called wechat login");

        //判断用户是否已安装微信App
        if ([WXApi isWXAppInstalled]) {
            [[PluginAliPay GetInstance] loginRequest];
        } else {
            NSLog(@"wechat is not installed, cant login");
            [[PluginAliPay GetInstance] OnCompleteFailCall:@"wechat_not_installed"];
        }
    }
    
    void Pay(const char* orderInfo, const char* scheme)
    {
    
    }
    
#if defined(__cplusplus)
}
#endif


@end
