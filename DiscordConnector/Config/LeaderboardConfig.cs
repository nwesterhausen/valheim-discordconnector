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
		this._leaderBoard1 = new LeaderBoardConfigValues(configFile, LeaderBoard1);
		this._leaderBoard2 = new LeaderBoardConfigValues(configFile, LeaderBoard2);
		this._leaderBoard3 = new LeaderBoardConfigValues(configFile, LeaderBoard3);
		this._leaderBoard4 = new LeaderBoardConfigValues(configFile, LeaderBoard4);
		this._leaderBoard5 = new LeaderBoardConfigValues(configFile, LeaderBoard5);
		this._activePlayersAnnouncementConfig = new ActivePlayersAnnouncementConfig(configFile, ActivePlayers);

		configFile.Save();
		this.LeaderBoards =
		[
			new LeaderBoardConfigReference
			{
				Type = this._leaderBoard1.type.Value,
				TimeRange = this._leaderBoard1.timeRange.Value,
				DisplayedHeading = this._leaderBoard1.displayedHeading.Value,
				NumberListings = this._leaderBoard1.numberListings.Value == 0
					? LeaderbBoard.MAX_LEADER_BOARD_SIZE
					: this._leaderBoard1.numberListings.Value,
				Enabled = this._leaderBoard1.enabled.Value,
				PeriodInMinutes = this._leaderBoard1.periodInMinutes.Value,
				Deaths = this._leaderBoard1.deaths.Value,
				Sessions = this._leaderBoard1.sessions.Value,
				Shouts = this._leaderBoard1.shouts.Value,
				Pings = this._leaderBoard1.pings.Value,
				TimeOnline = this._leaderBoard1.timeOnline.Value,
				WebhookEvent = Webhook.Event.Leaderboard1
			},
			new LeaderBoardConfigReference
			{
				Type = this._leaderBoard2.type.Value,
				TimeRange = this._leaderBoard2.timeRange.Value,
				DisplayedHeading = this._leaderBoard2.displayedHeading.Value,
				NumberListings = this._leaderBoard2.numberListings.Value == 0
					? LeaderbBoard.MAX_LEADER_BOARD_SIZE
					: this._leaderBoard2.numberListings.Value,
				Enabled = this._leaderBoard2.enabled.Value,
				PeriodInMinutes = this._leaderBoard2.periodInMinutes.Value,
				Deaths = this._leaderBoard2.deaths.Value,
				Sessions = this._leaderBoard2.sessions.Value,
				Shouts = this._leaderBoard2.shouts.Value,
				Pings = this._leaderBoard2.pings.Value,
				TimeOnline = this._leaderBoard2.timeOnline.Value,
				WebhookEvent = Webhook.Event.Leaderboard2
			},
			new LeaderBoardConfigReference
			{
				Type = this._leaderBoard3.type.Value,
				TimeRange = this._leaderBoard3.timeRange.Value,
				DisplayedHeading = this._leaderBoard3.displayedHeading.Value,
				NumberListings = this._leaderBoard3.numberListings.Value == 0
					? LeaderbBoard.MAX_LEADER_BOARD_SIZE
					: this._leaderBoard3.numberListings.Value,
				Enabled = this._leaderBoard3.enabled.Value,
				PeriodInMinutes = this._leaderBoard3.periodInMinutes.Value,
				Deaths = this._leaderBoard3.deaths.Value,
				Sessions = this._leaderBoard3.sessions.Value,
				Shouts = this._leaderBoard3.shouts.Value,
				Pings = this._leaderBoard3.pings.Value,
				TimeOnline = this._leaderBoard3.timeOnline.Value,
				WebhookEvent = Webhook.Event.Leaderboard3
			},
			new LeaderBoardConfigReference
			{
				Type = this._leaderBoard4.type.Value,
				TimeRange = this._leaderBoard4.timeRange.Value,
				DisplayedHeading = this._leaderBoard4.displayedHeading.Value,
				NumberListings = this._leaderBoard4.numberListings.Value == 0
					? LeaderbBoard.MAX_LEADER_BOARD_SIZE
					: this._leaderBoard4.numberListings.Value,
				Enabled = this._leaderBoard4.enabled.Value,
				PeriodInMinutes = this._leaderBoard4.periodInMinutes.Value,
				Deaths = this._leaderBoard4.deaths.Value,
				Sessions = this._leaderBoard4.sessions.Value,
				Shouts = this._leaderBoard4.shouts.Value,
				Pings = this._leaderBoard4.pings.Value,
				TimeOnline = this._leaderBoard4.timeOnline.Value,
				WebhookEvent = Webhook.Event.Leaderboard4
			},
			new LeaderBoardConfigReference
			{
				Type = this._leaderBoard5.type.Value,
				TimeRange = this._leaderBoard5.timeRange.Value,
				DisplayedHeading = this._leaderBoard5.displayedHeading.Value,
				NumberListings = this._leaderBoard5.numberListings.Value == 0
					? LeaderbBoard.MAX_LEADER_BOARD_SIZE
					: this._leaderBoard5.numberListings.Value,
				Enabled = this._leaderBoard5.enabled.Value,
				PeriodInMinutes = this._leaderBoard5.periodInMinutes.Value,
				Deaths = this._leaderBoard5.deaths.Value,
				Sessions = this._leaderBoard5.sessions.Value,
				Shouts = this._leaderBoard5.shouts.Value,
				Pings = this._leaderBoard5.pings.Value,
				TimeOnline = this._leaderBoard5.timeOnline.Value,
				WebhookEvent = Webhook.Event.Leaderboard5
			}
		];
	}

	// Variables
	public LeaderBoardConfigReference[] LeaderBoards { get; private set; }

	public ActivePlayersAnnouncementConfigValues ActivePlayersAnnouncement => this._activePlayersAnnouncementConfig.Value;

	public string ConfigAsJson()
	{
		var jsonString = "{";
		jsonString += $"\"leaderBoard1\":{this._leaderBoard1.ConfigAsJson()},";
		jsonString += $"\"leaderBoard2\":{this._leaderBoard2.ConfigAsJson()},";
		jsonString += $"\"leaderBoard3\":{this._leaderBoard3.ConfigAsJson()},";
		jsonString += $"\"leaderBoard4\":{this._leaderBoard4.ConfigAsJson()},";
		jsonString += $"\"leaderBoard5\":{this._leaderBoard5.ConfigAsJson()},";
		jsonString += $"\"activePlayersAnnouncement\":{this._activePlayersAnnouncementConfig.ConfigAsJson()}";
		jsonString += "}";
		return jsonString;
	}
}
