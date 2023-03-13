using System;
using System.Collections;
using PluginSet.Core;

#if ENABLE_ALIPAY
namespace PluginSet.Alipay
{
    [PluginRegister]
    public class PluginAliPay : PluginBase, IStartPlugin, ILoginPlugin
    {
        public override string Name => "Alipay";

        public int StartOrder => 0;

        public bool IsRunning { get; private set; }

        public bool IsEnableLogin => IsInstalled();
        public bool IsLoggedIn => !string.IsNullOrEmpty(userLoginData);
        
        public bool IsEnablePayment => IsInstalled();
        
        private string userLoginData = null;

        private string _appId;

        protected override void Init(PluginSetConfig config)
        {
            var aliPay = config.Get<PluginAliPayConfig>("AliPay");
            _appId = aliPay.AppId;
        }
        
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

        public IEnumerator StartPlugin()
        {
            if (IsRunning)
                yield break;
            
#if UNITY_ANDROID && !UNITY_EDITOR
            AliPayAndroidApi.InitApi(_appId);
#elif UNITY_IOS && !UNITY_EDITOR
            AliPayIosApi.InitApi(_appId);
#endif
            IsRunning = true;
        }

        public void DisposePlugin(bool isAppQuit = false)
        {
        }

        private bool IsInstalled()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return AliPayAndroidApi.IsAliPayInstalled();
#elif UNITY_IOS && !UNITY_EDITOR
            return AliPayIosApi.IsAliPayInstalled();
#else
            return false;
#endif
        }
    }
}
#endif
