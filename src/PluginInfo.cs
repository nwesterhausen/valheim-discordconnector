﻿using System.Reflection;
using DiscordConnector;

#region Assembly attributes
/*
 * These attributes define various meta information of the generated DLL.
 */
[assembly: AssemblyVersion(PluginInfo.PLUGIN_VERSION)]
[assembly: AssemblyTitle(PluginInfo.PLUGIN_NAME + " (" + PluginInfo.PLUGIN_ID + ")")]
[assembly: AssemblyProduct(PluginInfo.PLUGIN_NAME)]
[assembly: AssemblyCopyright("© 2021 " + PluginInfo.PLUGIN_AUTHOR + " Repository at " + PluginInfo.PLUGIN_REPO_SHORT)]
#endregion

namespace DiscordConnector
{
    internal static class PluginInfo
    {
#if NoBotSupport
        public const string PLUGIN_ID = "games.nwest.valheim.discordconnector-nobotsupport";
        public const string PLUGIN_NAME = "Valheim Discord Connector (No Discord Bot Support)";
#else
        public const string PLUGIN_ID = "games.nwest.valheim.discordconnector";
        public const string PLUGIN_NAME = "Valheim Discord Connector";
#endif
        public const string PLUGIN_VERSION = "1.4.2";
        public const string PLUGIN_REPO_SHORT = "github: nwesterhausen/valheim-discordconnector";
        public const string PLUGIN_AUTHOR = "Nicholas Westerhausen";
    }
}
