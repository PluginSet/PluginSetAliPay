using System;
using PluginSet.Core;
using UnityEngine;

#if UNITY_ANDROID && ENABLE_ALIPAY
namespace PluginSet.Alipay
{
    internal class AliPayAuthCallback : IPluginSetCallback
    {
        private string pluginName;
        private Action<Result> callback;
        public AliPayAuthCallback(string pluginName, Action<Result> callback)
        {
            this.pluginName = pluginName;
            this.callback = callback;
        }
        
        public void onSuccess(string result)
        {
            MainThread.Run(delegate
            {
                callback?.Invoke(new Result
                {
                    PluginName = pluginName,
                    Success = true,
                    Code = 0,
                    Data = result,
                    DataObject = JsonUtility.FromJson<AuthResult>(result),
                });
            });
        }

        public void onFailed(int code, string message)
        {
            MainThread.Run(delegate
            {
                callback?.Invoke(new Result
                {
                    PluginName = pluginName,
                    Success = false,
                    Code = code,
                    Error = message,
                });
            });
        }
    }


    internal class AliPayPaymentCallback : IPluginSetCallback
    {
        private string pluginName;
        private Action<Result> callback;
        public AliPayPaymentCallback(string pluginName, Action<Result> callback)
        {
            this.pluginName = pluginName;
            this.callback = callback;
        }
        
        public void onSuccess(string result)
        {
            MainThread.Run(delegate
            {
                callback?.Invoke(new Result
                {
                    PluginName = pluginName,
                    Success = true,
                    Code = 0,
                    Data = result,
                });
            });
        }

        public void onFailed(int code, string message)
        {
            MainThread.Run(delegate
            {
                callback?.Invoke(new Result
                {
                    PluginName = pluginName,
                    Success = false,
                    Code = code,
                    Error = message,
                });
            });
        }
    }


    public static class AliPayAndroidApi
    {
        private static AndroidJavaClass AliPayApi = new AndroidJavaClass("com.pluginset.alipay.AliPayApi");
        private static AndroidJavaObject CurrentActivity => AndroidHelper.CurrentActivity;

        public static bool IsAppInstalled()
        {
            return AliPayApi.CallStatic<bool>("IsAppInstalled");
        }

        public static void Login(string pluginName, string authInfo, Action<Result> callback)
        {
            AliPayApi.CallStatic("Login", CurrentActivity, authInfo, new AliPayAuthCallback(pluginName, callback));
        }

        public static void Pay(string pluginName, string orderInfo, Action<Result> callback)
        {
            AliPayApi.Call("Pay", CurrentActivity, orderInfo, new AliPayPaymentCallback(pluginName, callback));
        }
    }
}
#endif