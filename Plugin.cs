using BepInEx;
using BepInEx.Logging;

namespace DiscordConnector
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("valheim_server.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource StaticLogger;
        internal static PluginConfig StaticConfig;
        private void Awake()
        {
            StaticLogger = base.Logger;
            StaticConfig = new PluginConfig(base.Config);

            // Plugin startup logic
            StaticLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_ID} is loaded!");

            if (string.IsNullOrEmpty(StaticConfig.WebHookURL))
            {
                StaticLogger.LogError($"No value set for WebHookURL");
            }
        }
    }
}
