using System;
using System.Runtime.InteropServices;
using PluginSet.Core;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#if UNITY_IOS && ENABLE_ALIPAY
public static class AliPayIosApi
{
    private static AliPayIOSApiLoginCallBack loginCbIns;
    private class AliPayIOSApiLoginCallBack : MonoBehaviour
    {
        public Action<Result> loginCb = null;
        private void onAliPayLoginRet(string jsonInfo)
        {
            if (this.loginCb != null)
            {
                var info = (JObject)JsonConvert.DeserializeObject(jsonInfo);
                var ret = new Result();
                ret.Success = (int)info["status"] == 0;
                    
                if (ret.Success)
                {
                    ret.Data = jsonInfo;
                    ret.DataObject = info;
                }
                this.loginCb(ret);
            }
        }
    }

    private static class AliPayIOSApiInst
    {

            [DllImport("__Internal")]
            public static extern bool IsAlipayInstalled();

            [DllImport("__Internal")]
            public static extern void LoginAli(string objName, string scheme, string authUrl);
    }

    public static bool IsAliPayInstalled()
    {
        return AliPayIOSApiInst.IsAlipayInstalled();
    }

    public static void Login(string authInfo, string scheme, Action<Result> callback)
    {
        if (!loginCbIns)
        {
            loginCbIns = new GameObject("AliPayIOSApiLoginCallBack").AddComponent<AliPayIOSApiLoginCallBack>();
            UnityEngine.GameObject.DontDestroyOnLoad(loginCbIns.gameObject);
        }
        loginCbIns.loginCb = callback;
        AliPayIOSApiInst.LoginAli("AliPayIOSApiLoginCallBack", scheme, authInfo);
    }
}

#endif
