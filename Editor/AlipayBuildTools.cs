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
            
            var buildParams = context.BuildChannels.Get<BuildAliPayParams>();
            if (!buildParams.Enable)
                return;

            context.AddLinkAssembly("PluginSet.AliPay");
            context.Symbols.Add("ENABLE_ALIPAY");

            var pluginConfig = context.Get<PluginSetConfig>("pluginsConfig");
            var config = pluginConfig.AddConfig<PluginAliPayConfig>("AliPay");
            config.Scheme = buildParams.Scheme;
            
            Global.CopyDependenciesInLib("com.pluginset.alipay");
        }
        
        [AndroidProjectModify]
        public static void OnAndroidManifestModify(BuildProcessorContext context, AndroidProjectManager projectManager)
        {
            var buildParams = context.BuildChannels.Get<BuildAliPayParams>();
            if (!buildParams.Enable)
                return;

            // 根据Android官方给出的适配方案，在主工程的AndroidManifest.xml 中增加 <queries> 标签，即可解决以上影响，代码如下：
            var doc = projectManager.LauncherManifest;
            if (projectManager.LauncherGradle.TargetSdkVersion >= 28)
            {
                doc.addQueries("com.eg.android.AlipayGphone");
                doc.addQueries("hk.alipay.wallet");
            }
        }
        

        [iOSXCodeProjectModify]
        public static void OnIosXCodeProjectModify(BuildProcessorContext context, PBXProjectManager project)
        {
            if (context.BuildTarget != BuildTarget.iOS)
                return;
            
            var buildParams = context.BuildChannels.Get<BuildAliPayParams>();
            if (!buildParams.Enable)
                return;
            
            project.PlistDocument.AddXcodeURLType("aliLogin", "Editor", buildParams.Scheme);
        }
    }
}