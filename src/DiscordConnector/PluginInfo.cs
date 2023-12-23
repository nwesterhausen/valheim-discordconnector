using System.Reflection;
using DiscordConnectorLite;

#region Assembly attributes
/*
 * These attributes define various meta information of the generated DLL.
 */
[assembly: AssemblyVersion(PluginInfo.PLUGIN_VERSION)]
[assembly: AssemblyTitle(PluginInfo.PLUGIN_NAME + " (" + PluginInfo.PLUGIN_ID + ")")]
[assembly: AssemblyProduct(PluginInfo.PLUGIN_NAME)]
[assembly: AssemblyCopyright("© 2021 - 2024" + PluginInfo.PLUGIN_AUTHOR + " Repository at " + PluginInfo.PLUGIN_REPO_SHORT)]
#endregion

namespace DiscordConnectorLite
{
    internal static class PluginInfo
    {
#if NoBotSupport
        public const string PLUGIN_ID = "games.nwest.valheim.discordconnector-nobotsupport";
        public const string PLUGIN_NAME = "Valheim Discord Connector (No Discord Bot Support)";
#else
        public const string PLUGIN_ID = "games.nwest.valheim.discordconnector.lite";
        public const string PLUGIN_NAME = "Valheim Discord Connector Lite";
#endif

        public const string PLUGIN_VERSION = "3.0.0";
        public const string PLUGIN_REPO_SHORT = "github: Digitalroot-Valheim/nwesterhausen-valheim-discordconnector";
        public const string PLUGIN_AUTHOR = "Nicholas Westerhausen and Digitalroot";
    }
}
