using PluginSet.Core.Editor;
using UnityEngine;

namespace PluginSet.Alipay.Editor
{
    [BuildChannelsParams("AliPay", "支付宝SDK参数")]
    public class BuildAliPayParams : ScriptableObject
    {
        [Tooltip("支付宝未安装时API是否可用")]
        public bool Enable;
        
        [Tooltip("支付宝SDK提供的AppId参数")]
        public string AppId;

        public string Scheme;
    }
}