namespace DiscordConnector.Webhook
{
    /// <summary>
    /// Helper class definition which defines the structure of (valid) outgoing JSON
    /// </summary>
    internal class Response
    {
        public string status { get; set; }
        public string version => $"{PluginInfo.PLUGIN_ID}-{PluginInfo.PLUGIN_VERSION}";
    }

    internal class MessageResponse : Response
    {
        public string message { get; set; }
    }
}
