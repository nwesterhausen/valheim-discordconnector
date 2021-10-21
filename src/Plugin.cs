﻿using System;
using System.Collections.Generic;
using System.Timers;
using BepInEx;
using BepInEx.Logging;
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
        internal static Records StaticRecords;
        internal static string PublicIpAddress;
#if !NoBotSupport
        private static Webhook.Listener _listener;
#endif
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
                StaticLogger.LogWarning("No value set for WebHookURL! Plugin will run without using a main Discord webhook.");
            }

            if (StaticConfig.StatsAnnouncementEnabled)
            {
                System.Timers.Timer leaderboardTimer = new System.Timers.Timer();
                leaderboardTimer.Elapsed += SendLeaderboardAnnouncement;
                // Interval is learned from config file in minutes
                leaderboardTimer.Interval = 60 * 1000 * StaticConfig.StatsAnnouncementPeriod;
                leaderboardTimer.Start();
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

            var randEventTimer = new System.Timers.Timer();
            randEventTimer.Elapsed += CheckRandomEvent;
            randEventTimer.Interval = 5 * 1000; // 5 seconds
            randEventTimer.Start();

            _harmony = Harmony.CreateAndPatchAll(typeof(Plugin).Assembly, PluginInfo.PLUGIN_ID);
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
        private void CheckRandomEvent(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var res = RandEventSystem.instance;
            var re = res.GetCurrentRandomEvent();
            string re_name = "no existing random event";
            if (re != null)
            {
                re_name = re.m_name;
            }
            var ae = res.GetActiveEvent();
            string ae_name = "no active event";
            if (ae != null)
            {
                ae_name = ae.m_name;
            }
            string message = "random event check:" + Environment.NewLine +
            $"GetActiveEvent(): {ae_name}, GetCurrentRandomEvent(): {re_name}" + Environment.NewLine +
            $"[static] HaveActiveEvent(): {RandEventSystem.HaveActiveEvent()}, GetForcedEvent(): {res.GetForcedEvent()}, [static] InEvent(): {RandEventSystem.InEvent()}";

            StaticLogger.LogDebug(message);
        }

        /// <summary>
        /// Function which is a valid Timer action to provide sending leader board messages on an interval.
        /// 
        /// This should be moved to its own class and we should provide separate private functions to handle each
        /// variety of leader board. This enables greater flexibility when it comes to what kinds of leader boards
        /// can be supported.
        /// </summary>
        private void SendLeaderboardAnnouncement(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var deathLeader = StaticRecords.Retrieve(RecordCategories.Death);
            var joinLeader = StaticRecords.Retrieve(RecordCategories.Join);
            var shoutLeader = StaticRecords.Retrieve(RecordCategories.Shout);
            var pingLeader = StaticRecords.Retrieve(RecordCategories.Ping);

            List<Tuple<string, string>> leaderFields = new List<Tuple<string, string>>();
            if (StaticConfig.LeaderboardDeathEnabled && deathLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Deaths", $"{deathLeader.Item1} ({deathLeader.Item2})"));
            }
            if (StaticConfig.LeaderboardSessionEnabled && joinLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Sessions", $"{joinLeader.Item1} ({joinLeader.Item2})"));
            }
            if (StaticConfig.LeaderboardShoutEnabled && shoutLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Shouts", $"{shoutLeader.Item1} ({shoutLeader.Item2})"));
            }
            if (StaticConfig.LeaderboardPingEnabled && pingLeader.Item2 > 0)
            {
                leaderFields.Add(Tuple.Create("Most Pings", $"{pingLeader.Item1} ({pingLeader.Item2})"));
            }
            if (leaderFields.Count > 0)
            {
                DiscordApi.SendMessageWithFields("Current leader board for tracked stats:", leaderFields);
            }
            else
            {
                StaticLogger.LogInfo("Not sending a leaderboard because theirs either no leaders, or nothing allowed.");
            }

        }

        /// <summary>
        /// Works in Awake()
        /// </summary>
        public static bool IsHeadless() => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }
}
