using System.Threading.Tasks;
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
        internal static Database StaticDatabase;
        internal static Leaderboard StaticLeaderboards;
        internal static EventWatcher StaticEventWatcher;
        internal static ConfigWatcher StaticConfigWatcher;
        internal static string PublicIpAddress;
        private Harmony _harmony;

        public Plugin()
        {
            StaticLogger = Logger;
            StaticConfig = new PluginConfig(Config);
            StaticDatabase = new Records.Database(Paths.GameRootPath);
            StaticLeaderboards = new Leaderboard();

            StaticConfigWatcher = new ConfigWatcher();
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

            if (StaticConfig.LeaderboardConfigs[0].Enabled)
            {
                System.Timers.Timer leaderboard1Timer = new System.Timers.Timer();
                leaderboard1Timer.Elapsed += StaticLeaderboards.Leaderboard1.SendLeaderboardOnTimer;
                // Interval is learned from config file in minutes
                leaderboard1Timer.Interval = 60 * 1000 * StaticConfig.LeaderboardConfigs[0].PeriodInMinutes;
                Plugin.StaticLogger.LogInfo($"Enabling Leaderboard.1 timer with interval 1:{leaderboard1Timer.Interval} ms");
                leaderboard1Timer.Start();
            }

            if (StaticConfig.LeaderboardConfigs[1].Enabled)
            {
                System.Timers.Timer leaderboard2Timer = new System.Timers.Timer();
                leaderboard2Timer.Elapsed += StaticLeaderboards.Leaderboard2.SendLeaderboardOnTimer;
                // Interval is learned from config file in minutes
                leaderboard2Timer.Interval = 60 * 1000 * StaticConfig.LeaderboardConfigs[1].PeriodInMinutes;
                Plugin.StaticLogger.LogInfo($"Enabling Leaderboard.2 timer with interval 1:{leaderboard2Timer.Interval} ms");
                leaderboard2Timer.Start();
            }

            if (StaticConfig.LeaderboardConfigs[2].Enabled)
            {
                System.Timers.Timer leaderboard3Timer = new System.Timers.Timer();
                leaderboard3Timer.Elapsed += StaticLeaderboards.Leaderboard3.SendLeaderboardOnTimer;
                // Interval is learned from config file in minutes
                leaderboard3Timer.Interval = 60 * 1000 * StaticConfig.LeaderboardConfigs[2].PeriodInMinutes;
                Plugin.StaticLogger.LogInfo($"Enabling Leaderboard.3 timer with interval 1:{leaderboard3Timer.Interval} ms");
                leaderboard3Timer.Start();
            }

            if (StaticConfig.LeaderboardConfigs[3].Enabled)
            {
                System.Timers.Timer leaderboard4Timer = new System.Timers.Timer();
                leaderboard4Timer.Elapsed += StaticLeaderboards.Leaderboard4.SendLeaderboardOnTimer;
                // Interval is learned from config file in minutes
                leaderboard4Timer.Interval = 60 * 1000 * StaticConfig.LeaderboardConfigs[3].PeriodInMinutes;
                Plugin.StaticLogger.LogInfo($"Enabling Leaderboard.4 timer with interval 1:{leaderboard4Timer.Interval} ms");
                leaderboard4Timer.Start();
            }



            Task.Run(() =>
            {
                PublicIpAddress = IpifyAPI.PublicIpAddress();
            }).ConfigureAwait(false);

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
