#import "AliPayApiManager.h"
const char* ALI_LOGIN_SCHEME = "aliScheme.xxx";

extern "C" {
     void UnitySendMessage(const char *, const char *, const char *);
     void InitAliApi(const char* appid){

     }

     bool IsAliPayInstalled(){
          return [AliPayApiManager IsAliPayInstalled];
     }

     void LoginAli(const char* objName,const char* authUrl){
          [AliPayApiManager Login:objName authUrl:authUrl];
     }
}

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
    bool b = [[UIApplication sharedApplication] canOpenURL:[NSURL URLWithString:@"alipay://"]];
    return b;
}

+(void)Login:(const char*)objName authUrl:(const char*)url {
     NSString* authUrl = [NSString stringWithFormat:@"%s",url];
    [AliPayApiManager Instance]._cbObjName = [NSString stringWithFormat:@"%s",objName];
     [[AlipaySDK defaultService] auth_V2WithInfo:authUrl fromScheme:[NSString stringWithUTF8String:ALI_LOGIN_SCHEME] callback:^(NSDictionary *resultDic) {
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
               NSString* ret = [NSString stringWithFormat:@"{'status':%d,'code':'%@','resultStatus':'%@'}",nStatus,authCode,status];
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
