using BepInEx;
using BepInEx.Logging;

namespace DiscordConnector
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("valheim_server.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        internal static new PluginConfig Config;
        private void Awake()
        {
            Logger = base.Logger;
            Config = new PluginConfig(base.Config);

            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_ID} is loaded!");
        }
    }
}
