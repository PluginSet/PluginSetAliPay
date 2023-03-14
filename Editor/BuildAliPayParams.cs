using PluginSet.Core.Editor;
using UnityEngine;

namespace PluginSet.Alipay.Editor
{
    [BuildChannelsParams("AliPay", "支付宝SDK参数")]
    public class BuildAliPayParams : ScriptableObject
    {
        [Tooltip("支付宝未安装时API是否可用")]
        public bool Enable;

        [Tooltip("支付宝唤起该应用使用的Scheme（IOS用）")]
        public string Scheme;
    }
}