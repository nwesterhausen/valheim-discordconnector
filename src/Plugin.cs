﻿using System;
using BepInEx;
using BepInEx.Logging;
using DiscordConnector.Records;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;

namespace DiscordConnector;

[BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource StaticLogger;
    internal static PluginConfig StaticConfig;
    internal static Database StaticDatabase;
    internal static LeaderBoard StaticLeaderBoards;
    internal static EventWatcher StaticEventWatcher;
    internal static ConfigWatcher StaticConfigWatcher;
    internal static string PublicIpAddress
    {
        /// <summary>
        /// Return the public IP address if we already know it. If we don't know it, find out.
        /// We avoid always getting the IP address to only get it when needed.
        /// </summary>
        /// <value>The public IP address of the server</value>
        get
        {
            if (!String.IsNullOrEmpty(_publicIpAddress))
            {
                return _publicIpAddress;
            }
            _publicIpAddress = ZNet.GetPublicIP();
            return _publicIpAddress;
        }
    }
    private static string _publicIpAddress;
    private Harmony _harmony;

    public Plugin()
    {
        StaticLogger = Logger;
        StaticConfig = new PluginConfig(Config);
        StaticDatabase = new Records.Database(Paths.GameRootPath);
        StaticLeaderBoards = new LeaderBoard();

        StaticConfigWatcher = new ConfigWatcher();

        _publicIpAddress = "";
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

        if (StaticConfig.LeaderBoards[0].Enabled)
        {
            System.Timers.Timer leaderBoard1Timer = new System.Timers.Timer();
            leaderBoard1Timer.Elapsed += StaticLeaderBoards.LeaderBoard1.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard1Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[0].PeriodInMinutes;
            Plugin.StaticLogger.LogInfo($"Enabling LeaderBoard.1 timer with interval {Strings.HumanReadableMs(leaderBoard1Timer.Interval)}");
            leaderBoard1Timer.Start();
        }

        if (StaticConfig.LeaderBoards[1].Enabled)
        {
            System.Timers.Timer leaderBoard2Timer = new System.Timers.Timer();
            leaderBoard2Timer.Elapsed += StaticLeaderBoards.LeaderBoard2.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard2Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[1].PeriodInMinutes;
            Plugin.StaticLogger.LogInfo($"Enabling LeaderBoard.2 timer with interval {Strings.HumanReadableMs(leaderBoard2Timer.Interval)}");
            leaderBoard2Timer.Start();
        }

        if (StaticConfig.LeaderBoards[2].Enabled)
        {
            System.Timers.Timer leaderBoard3Timer = new System.Timers.Timer();
            leaderBoard3Timer.Elapsed += StaticLeaderBoards.LeaderBoard3.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard3Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[2].PeriodInMinutes;
            Plugin.StaticLogger.LogInfo($"Enabling LeaderBoard.3 timer with interval {Strings.HumanReadableMs(leaderBoard3Timer.Interval)}");
            leaderBoard3Timer.Start();
        }

        if (StaticConfig.LeaderBoards[3].Enabled)
        {
            System.Timers.Timer leaderBoard4Timer = new System.Timers.Timer();
            leaderBoard4Timer.Elapsed += StaticLeaderBoards.LeaderBoard4.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard4Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[3].PeriodInMinutes;
            Plugin.StaticLogger.LogInfo($"Enabling LeaderBoard.4 timer with interval {Strings.HumanReadableMs(leaderBoard4Timer.Interval)}");
            leaderBoard4Timer.Start();
        }

        if (StaticConfig.LeaderBoards[4].Enabled)
        {
            System.Timers.Timer leaderBoard5Timer = new System.Timers.Timer();
            leaderBoard5Timer.Elapsed += StaticLeaderBoards.LeaderBoard4.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard5Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[4].PeriodInMinutes;
            Plugin.StaticLogger.LogInfo($"Enabling LeaderBoard.5 timer with interval {Strings.HumanReadableMs(leaderBoard5Timer.Interval)}");
            leaderBoard5Timer.Start();
        }

        if (StaticConfig.ActivePlayersAnnouncement.Enabled)
        {
            System.Timers.Timer playerActivityTimer = new System.Timers.Timer();
            playerActivityTimer.Elapsed += LeaderBoards.ActivePlayersAnnouncement.SendOnTimer;
            // Interval is learned from config file in minutes
            playerActivityTimer.Interval = 60 * 1000 * StaticConfig.ActivePlayersAnnouncement.PeriodInMinutes;
            Plugin.StaticLogger.LogInfo($"Enabling Player Activity announcement with interval {Strings.HumanReadableMs(playerActivityTimer.Interval)}");
            playerActivityTimer.Start();
        }

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
