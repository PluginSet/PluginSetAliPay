#import <Foundation/Foundation.h>
#import <AlipaySDK/AlipaySDK.h>

@interface AliPayApiManager : NSObject
@property(nonatomic, strong) NSString *_cbObjName;

+ (instancetype)Instance;
+ (void)InitApi:(const char *)appid;
+ (bool)IsAliPayInstalled;
+ (void)Login:(const char *)objName authUrl:(const char *)url;
+ (void)handleOpenURL:(NSURL *)url;
@end