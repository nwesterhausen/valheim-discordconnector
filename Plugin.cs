using HarmonyLib;
using BepInEx;
using BepInEx.Logging;
using System;
using System.Collections.Generic;

namespace DiscordConnector
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource StaticLogger;
        internal static PluginConfig StaticConfig;
        internal static Records StaticRecords;
        private void Awake()
        {
            StaticLogger = base.Logger;
            StaticConfig = new PluginConfig(base.Config);
            var harmony = new Harmony(PluginInfo.PLUGIN_ID);

            // Plugin startup logic
            StaticLogger.LogDebug($"Plugin {PluginInfo.PLUGIN_ID} is loaded!");
            if (!BepInEx.Paths.ProcessName.Equals("valheim_server"))
            {
                StaticLogger.LogInfo("Not running on a dedicated server, some features may break -- please report them!");
            }
            else if (StaticConfig.LaunchMessageEnabled)
            {

                DiscordApi.SendMessage(
                    StaticConfig.LaunchMessage
                );

            }

            if (string.IsNullOrEmpty(StaticConfig.WebHookURL))
            {
                StaticLogger.LogWarning($"No value set for WebHookURL");
                return;
            }

            StaticRecords = new Records(BepInEx.Paths.GameRootPath);

            harmony.PatchAll();
        }
    }
}
