using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class LeaderBoardConfig
{
    // config header strings
    private const string LeaderBoard1 = "LeaderBoard.1";
    private const string LeaderBoard2 = "LeaderBoard.2";
    private const string LeaderBoard3 = "LeaderBoard.3";
    private const string LeaderBoard4 = "LeaderBoard.4";
    private const string LeaderBoard5 = "LeaderBoard.5";
    private const string ActivePlayers = "ActivePlayers.Announcement";

    public const string ConfigExtension = "leaderBoards";
    private readonly ActivePlayersAnnouncementConfig _activePlayersAnnouncementConfig;

    // Config Definitions
    private readonly LeaderBoardConfigValues _leaderBoard1;
    private readonly LeaderBoardConfigValues _leaderBoard2;
    private readonly LeaderBoardConfigValues _leaderBoard3;
    private readonly LeaderBoardConfigValues _leaderBoard4;
    private readonly LeaderBoardConfigValues _leaderBoard5;

    public LeaderBoardConfig(ConfigFile configFile)
    {
        _leaderBoard1 = new LeaderBoardConfigValues(configFile, LeaderBoard1);
        _leaderBoard2 = new LeaderBoardConfigValues(configFile, LeaderBoard2);
        _leaderBoard3 = new LeaderBoardConfigValues(configFile, LeaderBoard3);
        _leaderBoard4 = new LeaderBoardConfigValues(configFile, LeaderBoard4);
        _leaderBoard5 = new LeaderBoardConfigValues(configFile, LeaderBoard5);
        _activePlayersAnnouncementConfig = new ActivePlayersAnnouncementConfig(configFile, ActivePlayers);

        configFile.Save();
        LeaderBoards =
        [
            new LeaderBoardConfigReference
            {
                Type = _leaderBoard1.type.Value,
                TimeRange = _leaderBoard1.timeRange.Value,
                DisplayedHeading = _leaderBoard1.displayedHeading.Value,
                NumberListings =
                    _leaderBoard1.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : _leaderBoard1.numberListings.Value,
                Enabled = _leaderBoard1.enabled.Value,
                PeriodInMinutes = _leaderBoard1.periodInMinutes.Value,
                Deaths = _leaderBoard1.deaths.Value,
                Sessions = _leaderBoard1.sessions.Value,
                Shouts = _leaderBoard1.shouts.Value,
                Pings = _leaderBoard1.pings.Value,
                TimeOnline = _leaderBoard1.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard1
            },
            new LeaderBoardConfigReference
            {
                Type = _leaderBoard2.type.Value,
                TimeRange = _leaderBoard2.timeRange.Value,
                DisplayedHeading = _leaderBoard2.displayedHeading.Value,
                NumberListings =
                    _leaderBoard2.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : _leaderBoard2.numberListings.Value,
                Enabled = _leaderBoard2.enabled.Value,
                PeriodInMinutes = _leaderBoard2.periodInMinutes.Value,
                Deaths = _leaderBoard2.deaths.Value,
                Sessions = _leaderBoard2.sessions.Value,
                Shouts = _leaderBoard2.shouts.Value,
                Pings = _leaderBoard2.pings.Value,
                TimeOnline = _leaderBoard2.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard2
            },
            new LeaderBoardConfigReference
            {
                Type = _leaderBoard3.type.Value,
                TimeRange = _leaderBoard3.timeRange.Value,
                DisplayedHeading = _leaderBoard3.displayedHeading.Value,
                NumberListings =
                    _leaderBoard3.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : _leaderBoard3.numberListings.Value,
                Enabled = _leaderBoard3.enabled.Value,
                PeriodInMinutes = _leaderBoard3.periodInMinutes.Value,
                Deaths = _leaderBoard3.deaths.Value,
                Sessions = _leaderBoard3.sessions.Value,
                Shouts = _leaderBoard3.shouts.Value,
                Pings = _leaderBoard3.pings.Value,
                TimeOnline = _leaderBoard3.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard3
            },
            new LeaderBoardConfigReference
            {
                Type = _leaderBoard4.type.Value,
                TimeRange = _leaderBoard4.timeRange.Value,
                DisplayedHeading = _leaderBoard4.displayedHeading.Value,
                NumberListings =
                    _leaderBoard4.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : _leaderBoard4.numberListings.Value,
                Enabled = _leaderBoard4.enabled.Value,
                PeriodInMinutes = _leaderBoard4.periodInMinutes.Value,
                Deaths = _leaderBoard4.deaths.Value,
                Sessions = _leaderBoard4.sessions.Value,
                Shouts = _leaderBoard4.shouts.Value,
                Pings = _leaderBoard4.pings.Value,
                TimeOnline = _leaderBoard4.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard4
            },
            new LeaderBoardConfigReference
            {
                Type = _leaderBoard5.type.Value,
                TimeRange = _leaderBoard5.timeRange.Value,
                DisplayedHeading = _leaderBoard5.displayedHeading.Value,
                NumberListings =
                    _leaderBoard5.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : _leaderBoard5.numberListings.Value,
                Enabled = _leaderBoard5.enabled.Value,
                PeriodInMinutes = _leaderBoard5.periodInMinutes.Value,
                Deaths = _leaderBoard5.deaths.Value,
                Sessions = _leaderBoard5.sessions.Value,
                Shouts = _leaderBoard5.shouts.Value,
                Pings = _leaderBoard5.pings.Value,
                TimeOnline = _leaderBoard5.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard5
            }
        ];
    }

    // Variables
    public LeaderBoardConfigReference[] LeaderBoards { get; private set; }

    public ActivePlayersAnnouncementConfigValues ActivePlayersAnnouncement => _activePlayersAnnouncementConfig.Value;

    public string ConfigAsJson()
    {
        string jsonString = "{";
        jsonString += $"\"leaderBoard1\":{_leaderBoard1.ConfigAsJson()},";
        jsonString += $"\"leaderBoard2\":{_leaderBoard2.ConfigAsJson()},";
        jsonString += $"\"leaderBoard3\":{_leaderBoard3.ConfigAsJson()},";
        jsonString += $"\"leaderBoard4\":{_leaderBoard4.ConfigAsJson()},";
        jsonString += $"\"leaderBoard5\":{_leaderBoard5.ConfigAsJson()},";
        jsonString += $"\"activePlayersAnnouncement\":{_activePlayersAnnouncementConfig.ConfigAsJson()}";
        jsonString += "}";
        return jsonString;
    }
}
