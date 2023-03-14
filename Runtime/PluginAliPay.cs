using System;
using System.Collections;
using PluginSet.Core;

#if ENABLE_ALIPAY
namespace PluginSet.Alipay
{
    public struct AuthResult
    {
        public string auth_code;
        public string alipay_open_id;
    }
    
    [PluginRegister]
    public class PluginAliPay : PluginBase, ILoginPlugin, IPaymentPlugin
    {
        public override string Name => "Alipay";

        public bool IsEnableLogin => IsInstalled();
        public bool IsLoggedIn => !string.IsNullOrEmpty(userLoginData);
        
        public bool IsEnablePayment => IsInstalled();
        
        private string userLoginData = null;
        
        public void Login(Action<Result> callback = null, string authInfo = null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AliPayAndroidApi.Login(Name, authInfo, delegate(Result result)
            {
                if (result.Success)
                    userLoginData = result.Data;
                callback?.Invoke(result);
            });
#elif UNITY_IOS && !UNITY_EDITOR
            AliPayIosApi.Login(authInfo, delegate(Result result)
            {
                if (result.Success)
                    userLoginData = result.Data;
                result.PluginName = Name;
                callback?.Invoke(result);
            });
#else
            callback?.Invoke(new Result
            {
                PluginName = Name,
                Success = false,
                Code = PluginConstants.FailDefaultCode,
                Error = "Invalid platform for aliPay"
            });
#endif
        }

        public void Logout(Action<Result> callback = null, string _ = null)
        {
        }

        public string GetUserLoginData()
        {
            return userLoginData;
        }

        private bool IsInstalled()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return AliPayAndroidApi.IsAppInstalled();
#elif UNITY_IOS && !UNITY_EDITOR
            return AliPayIosApi.IsAliPayInstalled();
#else
            return false;
#endif
        }

        public void Pay(string productId, Action<Result> callback = null, string orderInfo = null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AliPayAndroidApi.Pay(Name, orderInfo, callback);
//#elif UNITY_IOS && !UNITY_EDITOR
#else
            callback?.Invoke(new Result
            {
                PluginName = Name,
                Success = false,
                Code = PluginConstants.FailDefaultCode,
                Error = "Invalid platform for aliPay"
            });
#endif
        }
    }
}
#endif
