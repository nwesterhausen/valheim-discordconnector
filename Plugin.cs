using HarmonyLib;
using BepInEx;
using BepInEx.Logging;

namespace DiscordConnector
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource StaticLogger;
        internal static PluginConfig StaticConfig;
        private Harmony _harmony;

        public Plugin()
        {
          StaticLogger = Logger;
          StaticConfig = new PluginConfig(Config);
        }

        private void Awake()
        {
            // Plugin startup logic
            StaticLogger.LogDebug($"Plugin {PluginInfo.PLUGIN_ID} is loaded!");

            if (string.IsNullOrEmpty(StaticConfig.WebHookURL))
            {
                StaticLogger.LogWarning($"No value set for WebHookURL");
                return;
            }

            if (StaticConfig.LaunchMessageEnabled)
            {
                DiscordApi.SendMessage(
                    StaticConfig.LaunchMessage
                );
            }

            _harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, PluginInfo.PLUGIN_ID);
        }

        private void OnDestroy()
        {
          _harmony.UnpatchSelf();
        }
    }
}
