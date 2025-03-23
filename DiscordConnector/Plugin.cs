using System.IO;

using BepInEx;

using DiscordConnector.Common;
using DiscordConnector.Leaderboards;
using DiscordConnector.Records;

using HarmonyLib;

using Jotunn.Entities;
using Jotunn.Managers;

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
    internal static LeaderbBoard StaticLeaderBoards = new();
    internal static EventWatcher StaticEventWatcher = new();
    private static string _publicIpAddress = "";
    private Harmony _harmony;

    internal static CustomRPC ChatMessageRPC;

    public DiscordConnectorPlugin()
    {
        StaticLogger = new VDCLogger(Logger, Path.Combine(Paths.ConfigPath, LegacyConfigPath));
        StaticConfig = new PluginConfig(Config);
        StaticDatabase = new Database(Paths.GameRootPath);
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

    private void Awake()
    {
        // Plugin startup logic
        StaticLogger.LogDebug($"Plugin {ModName} is loaded!");

        ChatMessageRPC = NetworkManager.Instance.AddRPC(
            RPC.Common.RPC_OnNewChatMessage,
            RPC.RPCServer.RPC_OnNewChatMessage,
            RPC.Client.RPC_OnNewChatMessage);

        if (string.IsNullOrEmpty(StaticConfig.PrimaryWebhook.Url) &&
            string.IsNullOrEmpty(StaticConfig.SecondaryWebhook.Url))
        {
            StaticLogger.LogWarning(
                "No value set for WebHookURL! Plugin will run without using a main Discord webhook.");
        }

        if (StaticConfig.LeaderBoards[0].Enabled)
        {
            System.Timers.Timer leaderBoard1Timer = new();
            leaderBoard1Timer.Elapsed += StaticLeaderBoards.LeaderBoard1.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard1Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[0].PeriodInMinutes;
            StaticLogger.LogInfo(
                $"Enabling LeaderBoard.1 timer with interval {Strings.HumanReadableMs(leaderBoard1Timer.Interval)}");
            leaderBoard1Timer.Start();
        }

        if (StaticConfig.LeaderBoards[1].Enabled)
        {
            System.Timers.Timer leaderBoard2Timer = new();
            leaderBoard2Timer.Elapsed += StaticLeaderBoards.LeaderBoard2.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard2Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[1].PeriodInMinutes;
            StaticLogger.LogInfo(
                $"Enabling LeaderBoard.2 timer with interval {Strings.HumanReadableMs(leaderBoard2Timer.Interval)}");
            leaderBoard2Timer.Start();
        }

        if (StaticConfig.LeaderBoards[2].Enabled)
        {
            System.Timers.Timer leaderBoard3Timer = new();
            leaderBoard3Timer.Elapsed += StaticLeaderBoards.LeaderBoard3.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard3Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[2].PeriodInMinutes;
            StaticLogger.LogInfo(
                $"Enabling LeaderBoard.3 timer with interval {Strings.HumanReadableMs(leaderBoard3Timer.Interval)}");
            leaderBoard3Timer.Start();
        }

        if (StaticConfig.LeaderBoards[3].Enabled)
        {
            System.Timers.Timer leaderBoard4Timer = new();
            leaderBoard4Timer.Elapsed += StaticLeaderBoards.LeaderBoard4.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard4Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[3].PeriodInMinutes;
            StaticLogger.LogInfo(
                $"Enabling LeaderBoard.4 timer with interval {Strings.HumanReadableMs(leaderBoard4Timer.Interval)}");
            leaderBoard4Timer.Start();
        }

        if (StaticConfig.LeaderBoards[4].Enabled)
        {
            System.Timers.Timer leaderBoard5Timer = new();
            leaderBoard5Timer.Elapsed += StaticLeaderBoards.LeaderBoard4.SendLeaderBoardOnTimer;
            // Interval is learned from config file in minutes
            leaderBoard5Timer.Interval = 60 * 1000 * StaticConfig.LeaderBoards[4].PeriodInMinutes;
            StaticLogger.LogInfo(
                $"Enabling LeaderBoard.5 timer with interval {Strings.HumanReadableMs(leaderBoard5Timer.Interval)}");
            leaderBoard5Timer.Start();
        }

        if (StaticConfig.ActivePlayersAnnouncement.Enabled)
        {
            System.Timers.Timer playerActivityTimer = new();
            playerActivityTimer.Elapsed += ActivePlayersAnnouncement.SendOnTimer;
            // Interval is learned from config file in minutes
            playerActivityTimer.Interval = 60 * 1000 * StaticConfig.ActivePlayersAnnouncement.PeriodInMinutes;
            StaticLogger.LogInfo(
                $"Enabling Player Activity announcement with interval {Strings.HumanReadableMs(playerActivityTimer.Interval)}");
            playerActivityTimer.Start();
        }

        _harmony = Harmony.CreateAndPatchAll(typeof(DiscordConnectorPlugin).Assembly, ModGUID);
    }

    private void OnDestroy()
    {
        StaticDatabase.Dispose();
        _harmony.UnpatchSelf();
    }

    /// <summary>
    ///     Works in Awake()
    /// </summary>
    public static bool IsHeadless()
    {
        return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }
}
