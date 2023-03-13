using System;
using PluginSet.Core;
using UnityEngine;

#if UNITY_ANDROID
namespace PluginSet.Alipay
{

    internal interface IAliPayApiCallback
    {
        void onSuccess(string json);

        void onFail(int errorCode);
    }

    internal abstract class AliPayCallbackAdapter : AndroidJavaProxy, IAliPayApiCallback
    {
        public AliPayCallbackAdapter()
        : base("com.pluginset.alipaysdk.IAlipayApiCallback")
        {

        }

        public abstract void onSuccess(string json);

        public abstract void onFail(int errorCode);
    }


    internal class AliPayLoginCallback : AliPayCallbackAdapter
    {
        private struct Response
        {
            public int code;
            public string authCode;
            public string msg;

            public Response(int code, string authCode, string msg)
            {
                this.code = code;
                this.authCode = authCode;
                this.msg = msg;
            }
        }

        private string pluginName;
        private Action<Result> callback;

        public AliPayLoginCallback(string pName, Action<Result> cb)
        {
            pluginName = pName;
            callback = cb;
        }

        public override void onSuccess(string json)
        {
            var resp = JsonUtility.FromJson<Response>(json);
            MainThread.Run(() =>
            {
                callback?.Invoke(new Result
                {
                    Success = true,
                    PluginName = pluginName,
                    Code = resp.code,
                    Error = resp.msg,
                    Data = resp.authCode,
                });
            });
        }

        public override void onFail(int errorCode)
        {
            MainThread.Run(() =>
            {
                callback?.Invoke(new Result
                {
                    Success = false,
                    PluginName = pluginName,
                    Code = errorCode
                });
            });
        }

    }


    internal class AliPayPaymentCallback : AliPayCallbackAdapter
    {
        private struct Response
        {
            public string prepayId;
            public string returnKey;
            public string extData;

            public Response(string prepayId, string returnKey, string extData)
            {
                this.prepayId = prepayId;
                this.returnKey = returnKey;
                this.extData = extData;
            }
        }

        private Action<Result> callback;
        private string pluginName;
        private string prodcutId;
        private string extraData;

        public AliPayPaymentCallback(string pName, string pId, string extra, Action<Result> cb)
        {
            callback = cb;
            pluginName = pName;
            prodcutId = pId;
            extraData = extra;
        }

        public override void onSuccess(string json)
        {
            var resp = JsonUtility.FromJson<Response>(json);
            MainThread.Run(() =>
            {
                callback?.Invoke(new Result
                {
                    Success = true,
                    Code = 0,
                    PluginName = pluginName,
                    Data = resp.extData
                });
            });
        }

        public override void onFail(int errorCode)
        {
            MainThread.Run(() =>
            {
                callback?.Invoke(new Result
                {
                    Success = false,
                    Code = errorCode,
                    PluginName = pluginName,
                    Data = extraData,
                });
            });
        }
    }


    public static class AliPayAndroidApi
    {
        private static AndroidJavaClass AlipayApi = new AndroidJavaClass("com.pluginset.alipaysdk.AlipayApi");

        private static AndroidJavaObject _alipayApiInst;
        private static AndroidJavaObject AlipayApiInst
        {
            get
            {
                if (_alipayApiInst == null)
                {
                    _alipayApiInst = AlipayApi.CallStatic<AndroidJavaObject>("GetInstance");
                }
                return _alipayApiInst;
            }
        }

        public static void InitApi(string appId)
        {
            AlipayApiInst.Call("InitApi", AndroidHelper.CurrentActivity, appId);
        }

        public static bool IsAliPayInstalled()
        {
            return AlipayApiInst.Call<bool>("IsAlipayInstalled");
        }


        public static void Login(string pluginName, string authInfo, Action<Result> callback)
        {
            AlipayApiInst.Call("Login", authInfo, new AliPayLoginCallback(pluginName, callback));
        }

        public static void Pay(string pluginName, string productId, string jsonData, Action<Result> callback)
        {
            AlipayApiInst.Call("Pay", productId, jsonData, new AliPayPaymentCallback(pluginName, productId, jsonData, callback));
        }
    }
}
#endif