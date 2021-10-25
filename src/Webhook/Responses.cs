#if !NoBotSupport
using Newtonsoft.Json.Linq;

namespace DiscordConnector.Webhook
{
    /// <summary>
    /// Helper class definition which defines the structure of (valid) outgoing JSON
    /// </summary>
    internal class Response
    {
        public string status => $"{Plugin.ServerStatus}";
        public string version => $"{PluginInfo.PLUGIN_ID}-{PluginInfo.PLUGIN_VERSION}";
        public int statusCode = 200;
    }

    internal class MessageResponse : Response
    {
        public string message { get; set; }
    }

    internal class UnauthorizedResponse : Response
    {
        public new string status => "check your authorization header";
        public new string version => $"{PluginInfo.PLUGIN_ID}-{PluginInfo.PLUGIN_VERSION}";
        public new int statusCode = 401;
    }

    internal class JObjectResponse : Response
    {
        public JObject data { get; set; }
    }
}
#endif
