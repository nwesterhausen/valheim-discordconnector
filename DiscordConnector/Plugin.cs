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
    private readonly Harmony _harmony = new(ModGUID);

    public DiscordConnectorPlugin()
    {
        StaticLogger = new VDCLogger(Logger);
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

    /// <summary>
    ///     Works in Awake()
    /// </summary>
    public static bool IsHeadless()
    {
        return SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null;
    }
}
