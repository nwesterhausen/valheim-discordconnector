using BepInEx;
using BepInEx.Logging;
using DiscordConnector.Records;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;

namespace DiscordConnector
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource StaticLogger;
        internal static PluginConfig StaticConfig;
        internal static RecordsOld StaticRecords;
        internal static Database StaticDatabase;
        internal static Leaderboard StaticLeaderboards;
        internal static EventWatcher StaticEventWatcher;
        internal static string PublicIpAddress;
        private Harmony _harmony;

        public Plugin()
        {
            StaticLogger = Logger;
            StaticConfig = new PluginConfig(Config);
            StaticRecords = new RecordsOld(Paths.GameRootPath);
            StaticDatabase = new Records.Database(Paths.GameRootPath);
            StaticLeaderboards = new Leaderboard();
        }

        private void Awake()
        {
            // Plugin startup logic
            StaticLogger.LogDebug($"Plugin {PluginInfo.PLUGIN_ID} is loaded!");
            if (!IsHeadless())
            {
                StaticLogger.LogInfo("Not running on a dedicated server, some features may break -- please report them!");
            }
            else
            {
                StaticEventWatcher = new EventWatcher();
            }

            if (string.IsNullOrEmpty(StaticConfig.WebHookURL))
            {
                StaticLogger.LogWarning("No value set for WebHookURL! Plugin will run without using a main Discord webhook.");
            }

            if (StaticConfig.StatsAnnouncementEnabled)
            {
                System.Timers.Timer leaderboardTimer = new System.Timers.Timer();
                leaderboardTimer.Elapsed += StaticLeaderboards.OverallHighest.SendLeaderboardOnTimer;
                leaderboardTimer.Elapsed += StaticLeaderboards.OverallLowest.SendLeaderboardOnTimer;
                leaderboardTimer.Elapsed += StaticLeaderboards.TopPlayers.SendLeaderboardOnTimer;
                // Interval is learned from config file in minutes
                leaderboardTimer.Interval = 60 * 1000 * StaticConfig.StatsAnnouncementPeriod;
                leaderboardTimer.Start();
            }

            PublicIpAddress = IpifyAPI.PublicIpAddress();

            _harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, PluginInfo.PLUGIN_ID);
        }

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();
            StaticDatabase.Dispose();
        }

        /// <summary>
        /// Works in Awake()
        /// </summary>
        public static bool IsHeadless() => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }
}
