using System;

namespace DiscordConnector.Records.Models;

public class PlayerStats
{
	public PlayerStats(string networkId, bool isPlayFab = true)
	{
		this.Id = networkId;

		if (isPlayFab)
		{
			this.PlayFabId = networkId;
			return;
		}

		this.SteamId = networkId;
	}

	public string Id { get; set; }
	public string PlayFabId { get; set; } = string.Empty;
	public string SteamId { get; set; } = string.Empty;
	public string PlayerName { get; set; } = string.Empty;
	public bool IsLoggedIn { get; set; }
	public DateTime FirstSeen { get; set; }
	public DateTime LastSeen { get; set; }
	public TimeSpan TotalTimePlayed { get; set; }
	public int TotalSessions { get; set; }
	public DateTime FirstDeath { get; set; }
	public DateTime LastDeath { get; set; }
	public int TotalDeaths { get; set; }
	public DateTime FirstPing { get; set; }
	public DateTime LastPing { get; set; }
	public int TotalPings { get; set; }
	public DateTime FirstJoin { get; set; }
	public DateTime LastJoin { get; set; }
	public int TotalJoins { get; set; }
	public DateTime FirstLeave { get; set; }
	public DateTime LastLeave { get; set; }
	public int TotalLeaves { get; set; }
	public DateTime FirstShout { get; set; }
	public DateTime LastShout { get; set; }
	public int TotalShouts { get; set; }
}
