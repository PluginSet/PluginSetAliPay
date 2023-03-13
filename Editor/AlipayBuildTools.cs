using System.IO;
using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.Alipay.Editor
{
    [BuildTools]
    public static class BuildSettings
    {
        [OnSyncEditorSetting]
        public static void OnSyncExportSetting(BuildProcessorContext context)
        {
            if (context.BuildTarget != BuildTarget.Android && context.BuildTarget != BuildTarget.iOS)
                return;
            
            var buildParams = context.BuildChannels.Get<BuildAliPayParams>("AliPay");
            if (!buildParams.Enable)
                return;

            context.AddLinkAssembly("PluginSet.AliPay");
            context.Symbols.Add("ENABLE_ALIPAY");

            var pluginConfig = context.Get<PluginSetConfig>("pluginsConfig");
            var config = pluginConfig.Get<PluginAliPayConfig>("AliPay");
            config.AppId = buildParams.AppId;
            
            Global.CopyDependenciesInLib("com.pluginset.alipay");
        }
        

        [iOSXCodeProjectModify]
        public static void OnIosXCodeProjectModify(BuildProcessorContext context, PBXProjectManager project)
        {
            if (context.BuildTarget != BuildTarget.iOS)
                return;
            
            var buildParams = context.BuildChannels.Get<BuildAliPayParams>("AliPay");
            if (!buildParams.Enable)
                return;
            
            project.PlistDocument.AddXcodeURLType("aliLogin", "Editor", buildParams.Scheme);
            
            // TODO FIX
            var sFullPath = Path.Combine(project.ProjectPath, "Libraries", "com.pluginset.alipay", "Plugins", "iOS", "AliPayApiManager.mm");
            var sCtx = File.ReadAllText(sFullPath);
            sCtx = sCtx.Replace("aliScheme.xxx", buildParams.Scheme);
            File.WriteAllText(sFullPath, sCtx);
        }
    }
}