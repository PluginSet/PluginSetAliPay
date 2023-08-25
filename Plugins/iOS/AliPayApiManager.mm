#import "AliPayApiManager.h"
#import "PluginUtils.h"

@interface AliPayApiManager()
@end

@implementation AliPayApiManager

static AliPayApiManager *_instance;
+(instancetype)Instance {
	@synchronized (self) {
		if(_instance == nil){
            _instance = [[AliPayApiManager alloc] init];
		}
	}
	return _instance;
}

+(void)InitApi:(const char *)appid{
    
}

+(bool)IsAliPayInstalled{
    bool b = [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"alipay:"]];
    return b;
}

+(void)Login:(const char*)objName scheme:(const char*)scheme authUrl:(const char*)url {
     NSString* authUrl = [NSString stringWithFormat:@"%s",url];
     [AliPayApiManager Instance]._cbObjName = [NSString stringWithFormat:@"%s",objName];
     [[AlipaySDK defaultService] auth_V2WithInfo:authUrl fromScheme:[NSString stringWithUTF8String:scheme] callback:^(NSDictionary *resultDic) {
          NSString *cbObjName = [AliPayApiManager Instance]._cbObjName;
          if(cbObjName != nil){
               NSString *result = resultDic[@"result"];
               NSString *status = resultDic[@"resultStatus"];
               NSString *authCode = nil;
               if (result.length>0) {
                    NSArray *resultArr = [result componentsSeparatedByString:@"&"];
                    for (NSString *subResult in resultArr) {
                         if (subResult.length > 10 && [subResult hasPrefix:@"auth_code="]) {
                              authCode = [subResult substringFromIndex:10];
                              break;
                         }
                    }
               }
               NSLog(@"授权结果 authCode = %@", authCode?:@"");
               int nStatus = -1;
               if([status isEqual:@"9000"]){
                    nStatus = 0;
               }
               NSString * ret = [PluginUtils dictToJson:@{
                 @"status": @(nStatus),
                 @"code": authCode,
                 @"resultStatus": status
               }];
               UnitySendMessage([cbObjName UTF8String],"onAliPayLoginRet",[ret UTF8String]);
          }
     }];
}

+(void)handleOpenURL:(NSURL*)url{
     if ([url.host isEqualToString:@"safepay"]) {
          [[AlipaySDK defaultService] processOrderWithPaymentResult:url standbyCallback:^(NSDictionary *resultDic) {
               NSLog(@"result = %@",resultDic);
          }];
        
          [[AlipaySDK defaultService] processAuth_V2Result:url standbyCallback:^(NSDictionary *resultDic) {
               NSLog(@"result = %@",resultDic);
          }];
    }
}

@end
