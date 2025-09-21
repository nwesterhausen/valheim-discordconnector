using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace DiscordConnector;

internal class EventWatcher
{
	private readonly System.Timers.Timer randEventTimer;
	private float PreviousElapsed;
	private Vector3 PreviousEventPos;
	private string PreviousEventStartMessage, PreviousEventEndMessage;

	private bool WasRunning, HadActiveEvent;

	public EventWatcher()
	{
		this.WasRunning = false;
		this.HadActiveEvent = false;
		this.PreviousElapsed = 0;
		this.PreviousEventStartMessage = "";
		this.PreviousEventEndMessage = "";
		this.PreviousEventPos = new Vector3();


		this.randEventTimer = new System.Timers.Timer();
		this.randEventTimer.Elapsed += this.CheckRandomEvent;
		this.randEventTimer.Interval = 1 * 1000; // 1 seconds
	}

	/// <summary>
	///     Activate the EventWatcher after the Event System has loaded! Otherwise it will provide false-positives.
	/// </summary>
	public void Activate() => this.randEventTimer.Start();

	public void Dispose() => this.randEventTimer.Stop();

	public void CheckRandomEvent(object sender, ElapsedEventArgs elapsedEventArgs)
	{
		if (Status.HaveActiveEvent)
		{
			/// <summary>
			/// Printing a detailed debug message with all the pieces we gather from Status.
			/// </summary>
			var message =
				$"Currently an event: {Status.HaveActiveEvent}. {Status.StartMessage} | {Status.EndMessage}" +
				Environment.NewLine +
				$"Event: {Status.Name} at {Status.Pos}. Status.IsRunning: {Status.IsRunning}. {Status.Elapsed} of {Status.Duration} seconds completed." +
				Environment.NewLine +
				$"PreviousEventStartMsg: {this.PreviousEventStartMessage}, PreviousEventEndMsg: {this.PreviousEventEndMessage}, PreviousEventPos: {this.PreviousEventPos}" +
				Environment.NewLine +
				$"Involved Players: {string.Join(",", Status.InvolvedPlayersList())}";
			if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventCheck)
			{
				DiscordConnectorPlugin.StaticLogger.LogDebug(message);
			}

			if (Status.IsRunning)
			{
				/// <summary>
				/// This checks for what has changed from the last time we checked the Random Event status.
				/// If 
				///     there was no event active before
				/// and current event is running
				///         Then
				///     The change is to START
				/// </summary>
				if (!this.HadActiveEvent)
				{
					this.TriggerEventStart();
					if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventChange)
					{
						DiscordConnectorPlugin.StaticLogger.LogDebug(message);
					}
				}

				/// <summary>
				/// This checks for what has changed from the last time we checked the Random Event status.
				/// If 
				///     an event was listed as active
				/// and an event was not running
				/// and current event is running
				///         Then
				///     The change is from PAUSED to RESUMED
				/// </summary>
				if (this.HadActiveEvent && !this.WasRunning)
				{
					this.TriggerEventResumed();
					if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventChange)
					{
						DiscordConnectorPlugin.StaticLogger.LogDebug(message);
					}
				}
			}
			else
			{
				/// <summary>
				/// This checks for what has changed from the last time we checked the Random Event status.
				/// If 
				///     an event was not listed as active
				/// and current event is not running
				///         OR
				/// If 
				///     an event was listed as active
				/// and an event was running
				/// and current event is not running
				///         Then
				///     The change is from RESUMED to PAUSED
				/// </summary>
				if (!this.HadActiveEvent
				    || (this.HadActiveEvent && this.WasRunning))
				{
					this.TriggerEventPaused();
					if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventChange)
					{
						DiscordConnectorPlugin.StaticLogger.LogDebug(message);
					}
				}
			}

			if (Status.Pos != Vector3.zero)
			{
				this.PreviousEventStartMessage = Status.StartMessage;
				this.PreviousEventEndMessage = Status.EndMessage;
				this.PreviousEventPos = Status.Pos;
			}
		}
		else
		{
			if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventCheck)
			{
				DiscordConnectorPlugin.StaticLogger.LogDebug(
					$"PreviousEventStartMsg: {this.PreviousEventStartMessage}, PreviousEventEndMsg: {this.PreviousEventEndMessage}, PreviousEventPos: {this.PreviousEventPos}" +
					Environment.NewLine +
					"Event check ran, no current events (or world isn't loaded yet)."
				);
			}

			/// <summary>
			/// This checks for what has changed from the last time we checked the Random Event status.
			/// If 
			///     an event was listed as active
			///         Then
			///     The change is to STOP
			/// </summary>
			if (this.HadActiveEvent)
			{
				this.TriggerEventStop();
				if (DiscordConnectorPlugin.StaticConfig.DebugEveryEventChange)
				{
					DiscordConnectorPlugin.StaticLogger.LogDebug("Event stopped!");
				}
			}
		}

		this.HadActiveEvent = Status.HaveActiveEvent;
		this.WasRunning = Status.IsRunning;
		this.PreviousElapsed = Status.Elapsed;
	}

	internal void TriggerEventStart()
	{
		if (DiscordConnectorPlugin.StaticConfig.EventStartMessageEnabled)
		{
			var message = MessageTransformer.FormatEventStartMessage(
				DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
				Status.StartMessage,
				Status.EndMessage
				// string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
			);
			if (!DiscordConnectorPlugin.StaticConfig.EventStartPosEnabled)
			{
				DiscordApi.SendMessage(Webhook.Event.EventStart, message);
				return;
			}

			if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
			{
				DiscordApi.SendMessage(Webhook.Event.EventStart, message, Status.Pos);
			}
			else
			{
				message = MessageTransformer.FormatEventStartMessage(
					DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
					Status.EndMessage,
					Status.StartMessage,
					// string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
					Status.Pos
				);
				DiscordApi.SendMessage(Webhook.Event.EventStart, message);
			}
		}
	}

	internal void TriggerEventPaused()
	{
		if (DiscordConnectorPlugin.StaticConfig.EventPausedMessageEnabled)
		{
			var message = MessageTransformer.FormatEventMessage(
				DiscordConnectorPlugin.StaticConfig.EventPausedMessage,
				Status.StartMessage,
				Status.EndMessage
				// string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
			);
			if (!DiscordConnectorPlugin.StaticConfig.EventPausedPosEnabled)
			{
				DiscordApi.SendMessage(Webhook.Event.EventPaused, message);
				return;
			}

			if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
			{
				DiscordApi.SendMessage(Webhook.Event.EventPaused, message, Status.Pos);
			}
			else
			{
				message = MessageTransformer.FormatEventMessage(
					DiscordConnectorPlugin.StaticConfig.EventPausedMessage,
					Status.StartMessage,
					Status.EndMessage,
					// string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
					Status.Pos
				);
				DiscordApi.SendMessage(Webhook.Event.EventPaused, message);
			}
		}
	}

	internal void TriggerEventResumed()
	{
		if (DiscordConnectorPlugin.StaticConfig.EventResumedMessageEnabled)
		{
			var message = MessageTransformer.FormatEventMessage(
				DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
				Status.StartMessage,
				Status.EndMessage
				// string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
			);
			if (!DiscordConnectorPlugin.StaticConfig.EventResumedPosEnabled)
			{
				DiscordApi.SendMessage(Webhook.Event.EventResumed, message);
				return;
			}

			if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
			{
				DiscordApi.SendMessage(Webhook.Event.EventResumed, message, Status.Pos);
			}
			else
			{
				message = MessageTransformer.FormatEventMessage(
					DiscordConnectorPlugin.StaticConfig.EventResumedMessage,
					Status.StartMessage,
					Status.EndMessage,
					// string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
					Status.Pos
				);
				DiscordApi.SendMessage(Webhook.Event.EventResumed, message);
			}
		}
	}

	internal void TriggerEventStop()
	{
		if (DiscordConnectorPlugin.StaticConfig.EventStopMessageEnabled)
		{
			var message = MessageTransformer.FormatEventEndMessage(
				DiscordConnectorPlugin.StaticConfig.EventStopMessage, this.PreviousEventStartMessage, this.PreviousEventEndMessage
				// string.Join(",", involvedPlayers.ToArray()) //! Removed with event changes 
			);
			if (!DiscordConnectorPlugin.StaticConfig.EventStopPosEnabled)
			{
				DiscordApi.SendMessage(Webhook.Event.EventStop, message);
				return;
			}

			if (DiscordConnectorPlugin.StaticConfig.DiscordEmbedsEnabled || !message.Contains("%POS%"))
			{
				DiscordApi.SendMessage(Webhook.Event.EventStop, message, this.PreviousEventPos);
			}
			else
			{
				message = MessageTransformer.FormatEventEndMessage(
					DiscordConnectorPlugin.StaticConfig.EventStopMessage, this.PreviousEventStartMessage, this.PreviousEventEndMessage,
					// string.Join(",", involvedPlayers.ToArray()), //! Removed with event changes 
					this.PreviousEventPos
				);
				DiscordApi.SendMessage(Webhook.Event.EventStop, message);
			}
		}
	}

	private static class Status
	{
		/// <summary>
		///     True if there is currently an active event on the map.
		/// </summary>
		public static bool HaveActiveEvent => RandEventSystem.HaveActiveEvent();

		public static RandomEvent? Event
		{
			get
			{
				if (!HaveActiveEvent)
				{
					return null;
				}

				return RandEventSystem.instance.GetCurrentRandomEvent();
			}
		}

		public static string Name => HaveActiveEvent && Event != null ? Event.m_name : "";
		public static float Duration => HaveActiveEvent && Event != null ? Event.m_duration : 0;
		public static float Elapsed => HaveActiveEvent && Event != null ? Event.m_time : 0;
		public static bool IsRunning => HaveActiveEvent && RandEventSystem.instance.IsAnyPlayerInEventArea(Event);
		public static Vector3 Pos => HaveActiveEvent && Event != null ? Event.m_pos : new Vector3(0, 0, 0);

		public static string EndMessage =>
			HaveActiveEvent && Event != null ? Localization.instance.Localize(Event.m_endMessage) : "";

		public static string StartMessage => HaveActiveEvent && Event != null
			? Localization.instance.Localize(Event.m_startMessage)
			: "";

		public static string[] InvolvedPlayersList()
		{
			List<string> playerList = new();
			if (!HaveActiveEvent)
			{
				return playerList.ToArray();
			}

			foreach (var playerInfo in ZNet.instance.GetPlayerList())
			{
				if (!playerInfo.m_publicPosition)
				{
					if (DiscordConnectorPlugin.StaticConfig.DebugEveryPlayerPosCheck)
					{
						DiscordConnectorPlugin.StaticLogger.LogDebug(
							$"Unable to check location for {playerInfo.m_name} because their location is not public.");
					}
				}
				else if (RandEventSystem.instance.IsInsideRandomEventArea(Event, playerInfo.m_position))
				{
					playerList.Add(playerInfo.m_name);
					if (DiscordConnectorPlugin.StaticConfig.DebugEveryPlayerPosCheck)
					{
						DiscordConnectorPlugin.StaticLogger.LogDebug(
							$"{playerInfo.m_name} is at {playerInfo.m_position}");
					}
				}
			}

			return playerList.ToArray();
		}
	}
}
