using System;
using BepInEx;
using DiscordConnector.Records;
using HarmonyLib;
using UnityEngine.Device;
using UnityEngine.Rendering;

namespace DiscordConnector;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class DiscordConnectorPlugin : BaseUnityPlugin
{
    internal const string ModName = "DiscordConnector";
    internal const string ModVersion = "3.0.0";
    internal const string Author = "nwesterhausen";
    private const string ModGUID = Author + "." + ModName;
    internal const string LegacyConfigPath = "games.nwest.valheim.discordconnector";
    internal const string LegacyModName = "discordconnector";
    
    internal static VDCLogger StaticLogger;
    internal static PluginConfig StaticConfig;
    internal static Database StaticDatabase;
    internal static LeaderbBoard StaticLeaderBoards = new LeaderbBoard();
    internal static EventWatcher StaticEventWatcher = new EventWatcher();
    private static string _publicIpAddress = "";
    private Harmony _harmony;

    public DiscordConnectorPlugin()
    {
        StaticLogger = new VDCLogger(Logger);
        StaticConfig = new PluginConfig(Config);
        StaticDatabase = new Database(Paths.GameRootPath);
    }

    private void Awake()
    {
        // Plugin startup logic
        StaticLogger.LogDebug($"Plugin {ModName} is loaded!");
        if (string.IsNullOrEmpty(StaticConfig.PrimaryWebhook.Url) && String.IsNullOrEmpty(StaticConfig.SecondaryWebhook.Url))
        {
            StaticLogger.LogWarning("No value set for WebHookURL! Plugin will run without using a main Discord webhook.");
        }

        if (StaticConfig.LeaderBoards[0].Enabled)
        {
            System.Timers.Timer leaderBoard1Timer = new System.Timers.Timer();
            leaderBoard1Timer.Elapsed += StaticLeaderBoards.LeaderBoard1.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard1Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[0].PeriodInMinutes;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Enabling LeaderBoard.1 timer with interval {Strings.HumanReadableMs(leaderBoard1Timer.Interval)}");
            leaderBoard1Timer.Start();
        }

        if (StaticConfig.LeaderBoards[1].Enabled)
        {
            System.Timers.Timer leaderBoard2Timer = new System.Timers.Timer();
            leaderBoard2Timer.Elapsed += StaticLeaderBoards.LeaderBoard2.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard2Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[1].PeriodInMinutes;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Enabling LeaderBoard.2 timer with interval {Strings.HumanReadableMs(leaderBoard2Timer.Interval)}");
            leaderBoard2Timer.Start();
        }

        if (StaticConfig.LeaderBoards[2].Enabled)
        {
            System.Timers.Timer leaderBoard3Timer = new System.Timers.Timer();
            leaderBoard3Timer.Elapsed += StaticLeaderBoards.LeaderBoard3.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard3Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[2].PeriodInMinutes;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Enabling LeaderBoard.3 timer with interval {Strings.HumanReadableMs(leaderBoard3Timer.Interval)}");
            leaderBoard3Timer.Start();
        }

        if (StaticConfig.LeaderBoards[3].Enabled)
        {
            System.Timers.Timer leaderBoard4Timer = new System.Timers.Timer();
            leaderBoard4Timer.Elapsed += StaticLeaderBoards.LeaderBoard4.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard4Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[3].PeriodInMinutes;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Enabling LeaderBoard.4 timer with interval {Strings.HumanReadableMs(leaderBoard4Timer.Interval)}");
            leaderBoard4Timer.Start();
        }

        if (StaticConfig.LeaderBoards[4].Enabled)
        {
            System.Timers.Timer leaderBoard5Timer = new System.Timers.Timer();
            leaderBoard5Timer.Elapsed += StaticLeaderBoards.LeaderBoard4.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard5Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[4].PeriodInMinutes;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Enabling LeaderBoard.5 timer with interval {Strings.HumanReadableMs(leaderBoard5Timer.Interval)}");
            leaderBoard5Timer.Start();
        }

        if (StaticConfig.ActivePlayersAnnouncement.Enabled)
        {
            System.Timers.Timer playerActivityTimer = new System.Timers.Timer();
            playerActivityTimer.Elapsed += Leaderboards.ActivePlayersAnnouncement.SendOnTimer;
            // Interval is learned from config file in minutes
            playerActivityTimer.Interval = 60 * 1000 * StaticConfig.ActivePlayersAnnouncement.PeriodInMinutes;
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Enabling Player Activity announcement with interval {Strings.HumanReadableMs(playerActivityTimer.Interval)}");
            playerActivityTimer.Start();
        }

        _harmony = Harmony.CreateAndPatchAll(typeof(DiscordConnectorPlugin).Assembly, ModGUID);
    }
    
    private void OnDestroy()
    {
        StaticDatabase.Dispose();
        _harmony.UnpatchSelf();
    }


    internal static string PublicIpAddress
    {
        get
        {
            if (!string.IsNullOrEmpty(_publicIpAddress))
            {
                return _publicIpAddress;
            }

            _publicIpAddress = PublicIPChecker.GetPublicIP();
            return _publicIpAddress;
        }
    }

    /// <summary>
    ///     Works in Awake()
    /// </summary>
    public static bool IsHeadless()
    {
        return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }
}
