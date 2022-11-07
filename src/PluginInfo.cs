using System.Reflection;
using DiscordConnector;

#region Assembly attributes
/*
 * These attributes define various meta information of the generated DLL.
 */
[assembly: AssemblyVersion(PluginInfo.PLUGIN_VERSION)]
[assembly: AssemblyTitle(PluginInfo.PLUGIN_NAME + " (" + PluginInfo.PLUGIN_ID + ")")]
[assembly: AssemblyProduct(PluginInfo.PLUGIN_NAME)]
[assembly: AssemblyCopyright("© 2022 " + PluginInfo.PLUGIN_AUTHOR + " Repository at " + PluginInfo.PLUGIN_REPO_SHORT)]
#endregion

namespace DiscordConnector
{
    internal static class PluginInfo
    {
        public const string PLUGIN_ID = "games.nwest.valheim.discordconnector";
        public const string PLUGIN_NAME = "Valheim Discord Connector";
        public const string PLUGIN_VERSION = "2.1.0";
        public const string PLUGIN_REPO_SHORT = "github: nwesterhausen/valheim-discordconnector";
        public const string PLUGIN_AUTHOR = "Nicholas Westerhausen";
        public const string SHORT_PLUGIN_ID = "discordconnector";
    }
}
