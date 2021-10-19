namespace DiscordConnector.Webhook
{
    /// <summary>
    /// Helper class definition which defines the structure of (valid) outgoing JSON
    /// </summary>
    internal class Response
    {
        public string status { get; set; }
    }

    internal class StringResponse : Response
    {
        public string data { get; set; }
    }
}
