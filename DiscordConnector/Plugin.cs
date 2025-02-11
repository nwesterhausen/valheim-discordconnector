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
    internal static VDCLogger StaticLogger;
    internal static PluginConfig StaticConfig;
    internal static Database StaticDatabase;
    internal static LeaderbBoard StaticLeaderBoards;
    internal static EventWatcher StaticEventWatcher;
    
    internal const string ModName = "DiscordConnector";
    internal const string ModVersion = "3.0.0";
    internal const string Author = "nwesterhausen";
    private const string ModGUID = Author + "." + ModName;
    
    internal static string PublicIpAddress
    {
        get
        {
            if (!String.IsNullOrEmpty(_publicIpAddress))
            {
                return _publicIpAddress;
            }
            _publicIpAddress = PublicIPChecker.GetPublicIP();
            return _publicIpAddress;
        }
    }    
        private static string _publicIpAddress;
    private readonly Harmony _harmony = new(ModGUID);
    
    /// <summary>
    /// Works in Awake()
    /// </summary>
    public static bool IsHeadless() => SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    
    public DiscordConnectorPlugin()
    {
        StaticLogger = new VDCLogger(Logger);
        StaticConfig = new PluginConfig(Config);
        StaticDatabase = new Records.Database(Paths.GameRootPath);
        StaticLeaderBoards = new LeaderbBoard();

        _publicIpAddress = "";
    }
}
