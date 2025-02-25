using BepInEx.Configuration;

namespace DiscordConnector.Config;

internal class LeaderBoardConfig
{
    // config header strings
    private const string LEADER_BOARD_1 = "LeaderBoard.1";
    private const string LEADER_BOARD_2 = "LeaderBoard.2";
    private const string LEADER_BOARD_3 = "LeaderBoard.3";
    private const string LEADER_BOARD_4 = "LeaderBoard.4";
    private const string LEADER_BOARD_5 = "LeaderBoard.5";
    private const string ACTIVE_PLAYERS = "ActivePlayers.Announcement";
    private static ConfigFile config;

    public static string ConfigExtension = "leaderBoards";
    private ActivePlayersAnnouncementConfig activePlayersAnnouncementConfig;

    // Config Definitions
    private LeaderBoardConfigValues leaderBoard1;
    private LeaderBoardConfigValues leaderBoard2;
    private LeaderBoardConfigValues leaderBoard3;
    private LeaderBoardConfigValues leaderBoard4;
    private LeaderBoardConfigValues leaderBoard5;

    public LeaderBoardConfig(ConfigFile configFile)
    {
        config = configFile;
        
        leaderBoard1 = new LeaderBoardConfigValues(config, LEADER_BOARD_1);
        leaderBoard2 = new LeaderBoardConfigValues(config, LEADER_BOARD_2);
        leaderBoard3 = new LeaderBoardConfigValues(config, LEADER_BOARD_3);
        leaderBoard4 = new LeaderBoardConfigValues(config, LEADER_BOARD_4);
        leaderBoard5 = new LeaderBoardConfigValues(config, LEADER_BOARD_5);
        activePlayersAnnouncementConfig = new ActivePlayersAnnouncementConfig(config, ACTIVE_PLAYERS);

        config.Save();
        LeaderBoards = new[]
        {
            new LeaderBoardConfigReference
            {
                Type = leaderBoard1.type.Value,
                TimeRange = leaderBoard1.timeRange.Value,
                DisplayedHeading = leaderBoard1.displayedHeading.Value,
                NumberListings =
                    leaderBoard1.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : leaderBoard1.numberListings.Value,
                Enabled = leaderBoard1.enabled.Value,
                PeriodInMinutes = leaderBoard1.periodInMinutes.Value,
                Deaths = leaderBoard1.deaths.Value,
                Sessions = leaderBoard1.sessions.Value,
                Shouts = leaderBoard1.shouts.Value,
                Pings = leaderBoard1.pings.Value,
                TimeOnline = leaderBoard1.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard1
            },
            new LeaderBoardConfigReference
            {
                Type = leaderBoard2.type.Value,
                TimeRange = leaderBoard2.timeRange.Value,
                DisplayedHeading = leaderBoard2.displayedHeading.Value,
                NumberListings =
                    leaderBoard2.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : leaderBoard2.numberListings.Value,
                Enabled = leaderBoard2.enabled.Value,
                PeriodInMinutes = leaderBoard2.periodInMinutes.Value,
                Deaths = leaderBoard2.deaths.Value,
                Sessions = leaderBoard2.sessions.Value,
                Shouts = leaderBoard2.shouts.Value,
                Pings = leaderBoard2.pings.Value,
                TimeOnline = leaderBoard2.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard2
            },
            new LeaderBoardConfigReference
            {
                Type = leaderBoard3.type.Value,
                TimeRange = leaderBoard3.timeRange.Value,
                DisplayedHeading = leaderBoard3.displayedHeading.Value,
                NumberListings =
                    leaderBoard3.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : leaderBoard3.numberListings.Value,
                Enabled = leaderBoard3.enabled.Value,
                PeriodInMinutes = leaderBoard3.periodInMinutes.Value,
                Deaths = leaderBoard3.deaths.Value,
                Sessions = leaderBoard3.sessions.Value,
                Shouts = leaderBoard3.shouts.Value,
                Pings = leaderBoard3.pings.Value,
                TimeOnline = leaderBoard3.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard3
            },
            new LeaderBoardConfigReference
            {
                Type = leaderBoard4.type.Value,
                TimeRange = leaderBoard4.timeRange.Value,
                DisplayedHeading = leaderBoard4.displayedHeading.Value,
                NumberListings =
                    leaderBoard4.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : leaderBoard4.numberListings.Value,
                Enabled = leaderBoard4.enabled.Value,
                PeriodInMinutes = leaderBoard4.periodInMinutes.Value,
                Deaths = leaderBoard4.deaths.Value,
                Sessions = leaderBoard4.sessions.Value,
                Shouts = leaderBoard4.shouts.Value,
                Pings = leaderBoard4.pings.Value,
                TimeOnline = leaderBoard4.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard4
            },
            new LeaderBoardConfigReference
            {
                Type = leaderBoard5.type.Value,
                TimeRange = leaderBoard5.timeRange.Value,
                DisplayedHeading = leaderBoard5.displayedHeading.Value,
                NumberListings =
                    leaderBoard5.numberListings.Value == 0
                        ? LeaderbBoard.MAX_LEADER_BOARD_SIZE
                        : leaderBoard5.numberListings.Value,
                Enabled = leaderBoard5.enabled.Value,
                PeriodInMinutes = leaderBoard5.periodInMinutes.Value,
                Deaths = leaderBoard5.deaths.Value,
                Sessions = leaderBoard5.sessions.Value,
                Shouts = leaderBoard5.shouts.Value,
                Pings = leaderBoard5.pings.Value,
                TimeOnline = leaderBoard5.timeOnline.Value,
                WebhookEvent = Webhook.Event.Leaderboard5
            }
        };
    }

    // Variables
    public LeaderBoardConfigReference[] LeaderBoards { get; private set; }

    public ActivePlayersAnnouncementConfigValues ActivePlayersAnnouncement => activePlayersAnnouncementConfig.Value;

    public string ConfigAsJson()
    {
        string jsonString = "{";
        jsonString += $"\"leaderBoard1\":{leaderBoard1.ConfigAsJson()},";
        jsonString += $"\"leaderBoard2\":{leaderBoard2.ConfigAsJson()},";
        jsonString += $"\"leaderBoard3\":{leaderBoard3.ConfigAsJson()},";
        jsonString += $"\"leaderBoard4\":{leaderBoard4.ConfigAsJson()},";
        jsonString += $"\"leaderBoard5\":{leaderBoard5.ConfigAsJson()},";
        jsonString += $"\"activePlayersAnnouncement\":{activePlayersAnnouncementConfig.ConfigAsJson()}";
        jsonString += "}";
        return jsonString;
    }
}
