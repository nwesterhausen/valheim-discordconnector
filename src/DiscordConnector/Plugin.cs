using BepInEx;
using BepInEx.Logging;
using DiscordConnectorLite.Config;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

namespace DiscordConnectorLite
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource StaticLogger;
        internal static PluginConfig StaticConfig;
        public static bool RunningHeadless;
        internal static string ServerStatus => $"{(RunningHeadless ? "Dedicated Server" : "Client-run Server")}; {ServerState}; {PublicIpAddress}; {ServerWorld}";
        internal static string PublicIpAddress, ServerState, ServerWorld;
#if !NoBotSupport
        private static Webhook.Listener _listener;
#endif
        private Harmony _harmony;

        public Plugin()
        {
            StaticLogger = Logger;
            StaticConfig = new PluginConfig(Config);
            ServerState = "not started";
            ServerWorld = "";
            PublicIpAddress = "";
        }

        [UsedImplicitly]
        private void Awake()
        {
            // Plugin startup logic
            RunningHeadless = IsHeadless();

            if (!RunningHeadless)
            {
                StaticLogger.LogInfo("Not running on a dedicated server, some features may break -- please report them!");
            }

            if (string.IsNullOrEmpty(StaticConfig.WebHookURL))
            {
                StaticLogger.LogWarning("No value set for WebHookURL! Plugin will run without using a main Discord webhook.");
            }

            PublicIpAddress = IpifyAPI.PublicIpAddress();

#if !NoBotSupport
            if (StaticConfig.DiscordBotEnabled)
            {
                _listener = new Webhook.Listener();
            }
#else
            StaticLogger.LogInfo("This version of the plugin compile without any Discord Bot support.");
#endif

            _harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, PluginInfo.PLUGIN_ID);
            ServerState = "awake";
        }

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();
#if !NoBotSupport
            if (StaticConfig.DiscordBotEnabled)
            {
                _listener.Dispose();
            }
#endif
        }

        /// <summary>
        /// Works in Awake()
        /// </summary>
        private static bool IsHeadless() => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }
}
