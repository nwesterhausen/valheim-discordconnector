using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.Rendering;

namespace DiscordConnector
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource StaticLogger;
        internal static PluginConfig StaticConfig;
        internal static Records StaticRecords;
        private Harmony _harmony;

        public Plugin()
        {
          StaticLogger = Logger;
          StaticConfig = new PluginConfig(Config);
          StaticRecords = new Records(Paths.GameRootPath);
        }
        
        private void Awake()
        {
            // Plugin startup logic
            StaticLogger.LogDebug($"Plugin {PluginInfo.PLUGIN_ID} is loaded!");
            if (!IsHeadless())
            {
                StaticLogger.LogInfo("Not running on a dedicated server, some features may break -- please report them!");
            }

            if (string.IsNullOrEmpty(StaticConfig.WebHookURL))
            {
                StaticLogger.LogWarning("No value set for WebHookURL");
                return;
            }

            if (StaticConfig.StatsAnnouncementEnabled)
            {
                System.Timers.Timer leaderboardTimer = new System.Timers.Timer();
                leaderboardTimer.Elapsed += SendLeaderboardAnnouncement;
                // Interval is learned from config file in minutes
                leaderboardTimer.Interval = 60 * 1000 * StaticConfig.StatsAnnouncementPeriod;
                leaderboardTimer.Start();
            }

            _harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, PluginInfo.PLUGIN_ID);
        }

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();
        }

        private void SendLeaderboardAnnouncement(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var deathLeader = StaticRecords.Retrieve(Categories.Death);
            var joinLeader = StaticRecords.Retrieve(Categories.Join);
            var shoutLeader = StaticRecords.Retrieve(Categories.Shout);
            var pingLeader = StaticRecords.Retrieve(Categories.Ping);

            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();
            if (deathLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Deaths", $"{deathLeader.Item1} ({deathLeader.Item2})"));
            }
            if (joinLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Joins|Leaves", $"{joinLeader.Item1} ({joinLeader.Item2})"));
            }
            if (shoutLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Shouts", $"{shoutLeader.Item1} ({shoutLeader.Item2})"));
            }
            if (pingLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Pings", $"{pingLeader.Item1} ({pingLeader.Item2})"));
            }

            DiscordApi.SendMessageWithFields("Current leader board for tracked stats:", leaderFields);
        }

        /// <summary>
        /// Works in Awake()
        /// </summary>
        public static bool IsHeadless() => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
  }
}
