using System;
using PluginSet.Core;
using UnityEngine;

#if UNITY_ANDROID && ENABLE_ALIPAY
namespace PluginSet.Alipay
{
    internal class AliPayAuthCallback : PluginSetCallback
    {
        public AliPayAuthCallback(string pluginName, Action<Result> callback)
        :base(delegate(string result)
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
        }, delegate(int code, string message)
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
        })
        {
        }
    }


    internal class AliPayPaymentCallback : PluginSetCallback
    {
        public AliPayPaymentCallback(string pluginName, Action<PurchaseResult> callback)
        :base(delegate(string result)
        {
            MainThread.Run(delegate
            {
                callback?.Invoke(new PurchaseResult
                {
                    PluginName = pluginName,
                    Success = true,
                    Code = 0,
                    Data = result,
                    DataObject = null,
                });
            });
        }, delegate(int code, string message) { 
            MainThread.Run(delegate
            {
                callback?.Invoke(new PurchaseResult
                {
                    PluginName = pluginName,
                    Success = false,
                    Code = code,
                    Error = message,
                });
            });
         })
        {
        }
    }


    public static class AliPayAndroidApi
    {
        private static AndroidJavaClass _aliPayApi;

        private static AndroidJavaClass AliPayApi
        {
            get
            {
                if (_aliPayApi == null)
                    _aliPayApi = new AndroidJavaClass("com.pluginset.alipay.AliPayApi");

                return _aliPayApi;
            }
        }


        private static AndroidJavaObject CurrentActivity => AndroidHelper.CurrentActivity;
        

        public static bool IsAppInstalled()
        {
            return AliPayApi.CallStatic<bool>("IsAppInstalled", CurrentActivity);
        }

        public static void Login(string pluginName, string authInfo, Action<Result> callback)
        {
            AliPayApi.CallStatic("Login", CurrentActivity, authInfo, new AliPayAuthCallback(pluginName, callback));
        }

        public static void Pay(string pluginName, string orderInfo, Action<PurchaseResult> callback)
        {
            AliPayApi.Call("Pay", CurrentActivity, orderInfo, new AliPayPaymentCallback(pluginName, callback));
        }
    }
}
#endif