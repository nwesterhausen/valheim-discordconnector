using System.IO;

using BepInEx;

using DiscordConnector.Common;

using HarmonyLib;

namespace DiscordConnector.Client;

[BepInPlugin(ModGUID, ModName, ModVersion)]
public class DiscordConnectorClientPlugin : BaseUnityPlugin
{
    internal const string ModName = "DiscordConnectorClient";
    internal const string ModVersion = "1.0.0";
    internal const string Author = "nwesterhausen";
    private const string ModGUID = Author + "." + ModName;
    internal const string LegacyConfigPath = "games.nwest.valheim.discordconnector";
    internal const string LegacyModName = "discordconnector";

    internal static VDCLogger StaticLogger;
    private Harmony _harmony;

    public DiscordConnectorClientPlugin()
    {
        StaticLogger = new VDCLogger(Logger, Path.Combine(Paths.ConfigPath, LegacyConfigPath));
    }

    private void Awake()
    {
        // Plugin startup logic
        StaticLogger.LogDebug($"Plugin {ModName} is loaded!");

        _harmony = Harmony.CreateAndPatchAll(typeof(DiscordConnectorClientPlugin).Assembly, ModGUID);
    }

    private void OnDestroy()
    {
        _harmony.UnpatchSelf();
    }
}
