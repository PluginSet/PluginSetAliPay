using PluginSet.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace PluginSet.Alipay.Editor
{
    [InitializeOnLoad]
    public static class AlipayPluginFilters
    {
        static AlipayPluginFilters()
        {
            var filter = PluginFilter.IsBuildParamsEnable<BuildAliPayParams>();
            PluginFilter.RegisterFilter("com.pluginset.alipay/Plugins/Android", filter);
            PluginFilter.RegisterFilter("com.pluginset.alipay/Plugins/iOS", filter);
        }
    }

}
