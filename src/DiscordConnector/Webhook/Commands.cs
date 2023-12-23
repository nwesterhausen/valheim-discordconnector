namespace DiscordConnectorLite.Webhook
{
    /// <summary>
    /// Base command class, all command objects will include a command string.
    /// </summary>
    internal class Command
    {
        public string command { get; set; }
    }

    /// <summary>
    /// Command which only has string data.
    /// </summary>
    internal class StringCommand : Command
    {
        public string data { get; set; }
    }

    /// <summary>
    /// Command which passes on chat messages using SpeakerData.
    /// </summary>
    internal class SpeakerCommand : Command
    {
        public SpeakerData data { get; set; }
    }

    /// <summary>
    /// Object reference for a chat message. Has a username and the content of the message.
    /// </summary>
    internal class SpeakerData
    {
        public string username { get; set; }
        public string content { get; set; }
    }

    internal class LeaderboardData
    {

        public string type { get; set; }
        public int number { get; set; } = -1;
        public string category { get; set; }

    }
}
