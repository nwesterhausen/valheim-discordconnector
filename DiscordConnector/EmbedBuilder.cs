using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordConnector;

/// <summary>
///     Builder class for creating Discord embed messages with a fluent interface.
///     Handles configuration-based field filtering, validation, and fallback handling.
///     Implements Discord's embed structure with support for multi-row field arrangements.
/// </summary>
internal class EmbedBuilder
{
	private const int MAX_FIELDS_PER_ROW = 3;

	// Constants for Discord limits
	public const int MAX_FIELDS_COUNT = 25;
	public const string DEFAULT_COLOR = "#7289DA"; // Discord Blurple
	public const string DEFAULT_SERVER_START_COLOR = "#43B581"; // Green
	public const string DEFAULT_SERVER_STOP_COLOR = "#F04747"; // Red
	public const string DEFAULT_PLAYER_JOIN_COLOR = "#43B581"; // Green
	public const string DEFAULT_PLAYER_LEAVE_COLOR = "#FAA61A"; // Orange
	public const string DEFAULT_DEATH_EVENT_COLOR = "#F04747"; // Red
	public const string DEFAULT_SHOUT_MESSAGE_COLOR = "#7289DA"; // Discord Blurple
	public const string DEFAULT_POSITION_MESSAGE_COLOR = "#7289DA"; // Discord Blurple
	public const string DEFAULT_OTHER_EVENT_COLOR = "#747F8D"; // Gray

	// Reference to the configuration for field visibility and other settings
	private readonly PluginConfig _config;

	// Discord embed object being constructed
	private readonly DiscordEmbed _embed;

	// Field tracking for organizing fields into rows
	private readonly List<DiscordField> _fields;
	private int _currentRowFieldCount;

	/// <summary>
	///     Initializes a new instance of the EmbedBuilder class with default settings.
	/// </summary>
	public EmbedBuilder()
	{
		this._embed = new DiscordEmbed();
		this._fields = new List<DiscordField>();
		this._currentRowFieldCount = 0;
		this._config = DiscordConnectorPlugin.StaticConfig;
	}

	/// <summary>
	///     Sets the title of the embed if enabled in configuration.
	/// </summary>
	/// <param name="title">The title to set</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetTitle(string? title)
	{
		if (this._config.EmbedTitleEnabled)
		{
			this._embed.title = title;
		}

		return this;
	}

	/// <summary>
	///     Sets the description of the embed if enabled in configuration.
	/// </summary>
	/// <param name="description">The description to set</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetDescription(string? description)
	{
		if (this._config.EmbedDescriptionEnabled)
		{
			this._embed.description = description;
		}

		return this;
	}

	/// <summary>
	///     Sets the URL of the embed.
	///     When set, makes the embed title a clickable link.
	/// </summary>
	/// <param name="url">The URL to set</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetUrl(string? url)
	{
		this._embed.url = url;
		return this;
	}

	/// <summary>
	///     Sets the URL of the embed using the configured URL template.
	///     Variable placeholders in the template will be replaced with the provided values.
	/// </summary>
	/// <param name="variables">Dictionary of variable names and their values</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetUrlFromTemplate(Dictionary<string, string>? variables = null)
	{
		if (string.IsNullOrEmpty(this._config.EmbedUrlTemplate))
		{
			return this;
		}

		var url = this._config.EmbedUrlTemplate;

		if (variables != null)
		{
			foreach (var pair in variables)
			{
				url = url.Replace($"{{{pair.Key}}}", pair.Value ?? string.Empty);
			}
		}

		return this.SetUrl(url);
	}

	/// <summary>
	///     Sets the color of the embed sidebar using decimal color representation (0-16777215).
	/// </summary>
	/// <param name="color">The decimal color value</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetColor(int color)
	{
		// Manual implementation of clamp since Math.Clamp may not be available in this framework version
		this._embed.color = Math.Max(0, Math.Min(color, 16777215));
		return this;
	}

	/// <summary>
	///     Sets the color of the embed sidebar using a hex color code (e.g., "#7289DA").
	/// </summary>
	/// <param name="hexColor">The hex color code</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetColor(string hexColor)
	{
		this._embed.color = this.HexColorToDecimal(hexColor);
		return this;
	}

	/// <summary>
	///     Converts a hex color string to its decimal representation.
	/// </summary>
	/// <param name="hexColor">The hex color string to convert</param>
	/// <returns>The decimal value of the color</returns>
	private int HexColorToDecimal(string hexColor)
	{
		if (string.IsNullOrEmpty(hexColor))
		{
			hexColor = DEFAULT_COLOR;
		}

		if (!hexColor.StartsWith("#"))
		{
			hexColor = "#" + hexColor;
		}

		try
		{
			// Remove the # if present and parse
			var colorValue = hexColor.TrimStart('#');

			// Ensure we have a 6-character hex string
			if (colorValue.Length != 6)
			{
				colorValue = DEFAULT_COLOR.TrimStart('#');
			}

			return Convert.ToInt32(colorValue, 16);
		}
		catch
		{
			// If there's any error in parsing, use the default color
			return Convert.ToInt32(DEFAULT_COLOR.TrimStart('#'), 16);
		}
	}

	/// <summary>
	///     Sets the color based on the event type using configuration settings.
	/// </summary>
	/// <param name="eventType">The event type</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetColorForEvent(Webhook.Event eventType)
	{
		var hexColor = DEFAULT_COLOR;

		// Determine appropriate color based on event type
		if (Webhook.ServerLaunchEvents.Contains(eventType) || Webhook.ServerStartEvents.Contains(eventType))
		{
			hexColor = this._config.EmbedServerStartColor;
		}
		else if (Webhook.ServerStopEvents.Contains(eventType) || Webhook.ServerShutdownEvents.Contains(eventType))
		{
			hexColor = this._config.EmbedServerStopColor;
		}
		else if (Webhook.ServerSaveEvents.Contains(eventType))
		{
			hexColor = this._config.EmbedServerStartColor;
		}
		else if (Webhook.PlayerJoinEvents.Contains(eventType))
		{
			hexColor = this._config.EmbedPlayerJoinColor;
		}
		else if (Webhook.PlayerLeaveEvents.Contains(eventType))
		{
			hexColor = this._config.EmbedPlayerLeaveColor;
		}
		else if (Webhook.PlayerDeathEvents.Contains(eventType))
		{
			hexColor = this._config.EmbedDeathEventColor;
		}
		else if (Webhook.PlayerShoutEvents.Contains(eventType))
		{
			hexColor = this._config.EmbedShoutMessageColor;
		}
		else if (Webhook.PlayerPingEvents.Contains(eventType))
		{
			hexColor = this._config.EmbedPositionMessageColor;
		}
		else if (Webhook.WorldEvents.Contains(eventType))
		{
			hexColor = this._config.EmbedWorldEventColor;
		}
		else
		{
			hexColor = this._config.EmbedOtherEventColor;
		}

		return this.SetColor(hexColor);
	}

	/// <summary>
	///     Sets the author information of the embed if enabled in configuration.
	/// </summary>
	/// <param name="name">The author name</param>
	/// <param name="url">The URL to make the author name clickable (optional)</param>
	/// <param name="iconUrl">The URL of the author icon (optional)</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetAuthor(string? name, string? url = null, string? iconUrl = null)
	{
		if (this._config.EmbedAuthorEnabled)
		{
			this._embed.author = new DiscordEmbedAuthor { name = name, url = url, icon_url = iconUrl };
		}

		return this;
	}

	/// <summary>
	///     Sets the thumbnail image in the top-right of the embed if enabled in configuration.
	/// </summary>
	/// <param name="url">The URL of the thumbnail image</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetThumbnail(string? url)
	{
		// Only set thumbnail if it's enabled and we have a valid URL
		if (this._config.EmbedThumbnailEnabled && !string.IsNullOrEmpty(url))
		{
			// At this point, we've verified url is not null or empty
			// But we use null-coalescing to satisfy the compiler
			this._embed.thumbnail = new DiscordEmbedThumbnail
			{
				url = url! // Using null-forgiving operator as we've already checked it's not null
			};
		}

		return this;
	}

	/// <summary>
	///     Sets the main image of the embed.
	/// </summary>
	/// <param name="url">The URL of the image</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetImage(string? url)
	{
		// Only set image if we have a valid URL
		if (!string.IsNullOrEmpty(url))
		{
			// At this point, we've verified url is not null or empty
			// But we use null-forgiving to satisfy the compiler
			this._embed.image = new DiscordEmbedImage
			{
				url = url! // Using null-forgiving operator as we've already checked it's not null
			};
		}

		return this;
	}

	/// <summary>
	///     Sets the footer information of the embed if enabled in configuration.
	/// </summary>
	/// <param name="text">The footer text</param>
	/// <param name="iconUrl">The URL of the footer icon (optional)</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetFooter(string? text, string? iconUrl = null)
	{
		if (this._config.EmbedFooterEnabled)
		{
			this._embed.footer = new DiscordEmbedFooter { text = text, icon_url = iconUrl };
		}

		return this;
	}

	/// <summary>
	///     Sets the footer information using the configured footer text template.
	///     Variable placeholders in the template will be replaced with the provided values.
	/// </summary>
	/// <param name="variables">Dictionary of variable names and their values</param>
	/// <param name="iconUrl">The URL of the footer icon (optional)</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetFooterFromTemplate(Dictionary<string, string>? variables = null, string? iconUrl = null)
	{
		if (!this._config.EmbedFooterEnabled || string.IsNullOrEmpty(this._config.EmbedFooterText))
		{
			return this;
		}

		var footerText = this._config.EmbedFooterText;

		if (variables != null)
		{
			foreach (var pair in variables)
			{
				footerText = footerText.Replace($"{{{pair.Key}}}", pair.Value);
			}
		}

		return this.SetFooter(footerText, iconUrl);
	}

	/// <summary>
	///     Sets the timestamp of the embed if enabled in configuration.
	///     If no timestamp is provided, the current UTC time is used.
	/// </summary>
	/// <param name="timestamp">The timestamp as a DateTimeOffset (optional)</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder SetTimestamp(DateTimeOffset? timestamp = null)
	{
		if (this._config.EmbedTimestampEnabled)
		{
			this._embed.timestamp = (timestamp ?? DateTimeOffset.UtcNow).ToString("o");
		}

		return this;
	}

	/// <summary>
	///     Adds a field to the embed if fields are enabled in configuration.
	/// </summary>
	/// <param name="name">The field name</param>
	/// <param name="value">The field value</param>
	/// <param name="inline">Whether the field should be displayed inline</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder AddField(string? name, string? value, bool inline = false)
	{
		// Skip field if either name or value is null or empty
		if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
		{
			return this;
		}

		// Check for field count limit - Discord allows max 25 fields
		if (this._fields.Count >= MAX_FIELDS_COUNT)
		{
			DiscordConnectorPlugin.StaticLogger.LogWarning(
				$"Cannot add more than {MAX_FIELDS_COUNT} fields to a Discord embed.");
			return this;
		}

		// Add the field
		this._fields.Add(new DiscordField { name = name, value = value, inline = inline });

		// Update row tracking for inline fields
		if (inline)
		{
			this._currentRowFieldCount++;
			if (this._currentRowFieldCount >= MAX_FIELDS_PER_ROW)
			{
				// Start a new row
				this._currentRowFieldCount = 0;
			}
		}
		else
		{
			// Non-inline fields always start a new row
			this._currentRowFieldCount = 0;
		}

		return this;
	}

	/// <summary>
	///     Adds an inline field to the embed.
	///     Inline fields appear side-by-side (up to 3 per row).
	/// </summary>
	/// <param name="name">The field name</param>
	/// <param name="value">The field value</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder AddInlineField(string? name, string? value) => this.AddField(name, value, true);

	/// <summary>
	///     Adds a formatted position field to the embed based on a Vector3 position.
	///     This creates a field with the position formatted according to the configured format.
	/// </summary>
	/// <param name="position">The Vector3 position to format</param>
	/// <param name="inline">Whether the field should be displayed inline</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder AddPositionField(Vector3 position, bool inline = true)
	{
		var positionText = MessageTransformer.FormatVector3AsPos(position);
		return this.AddField("Position", positionText, inline);
	}

	/// <summary>
	///     Forces a new row in the field layout by adding an empty non-inline field if needed.
	///     Only adds a row break if there are already inline fields in the current row.
	/// </summary>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder AddRowBreak()
	{
		if (this._currentRowFieldCount > 0)
		{
			// Add invisible field with empty string as content and no inline
			this._fields.Add(new DiscordField
			{
				name = "\u200B", // Zero-width space
				value = "\u200B", // Zero-width space
				inline = false
			});
			this._currentRowFieldCount = 0;
		}

		return this;
	}

	/// <summary>
	///     Adds multiple fields from a list of tuples.
	///     Each tuple contains field name and value.
	/// </summary>
	/// <param name="fields">List of field name/value tuples</param>
	/// <param name="inline">Whether the fields should display inline</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder AddFields(List<Tuple<string, string>> fields, bool inline = false)
	{
		if (fields == null || fields.Count == 0)
		{
			return this;
		}

		foreach (var field in fields)
		{
			this.AddField(field.Item1, field.Item2, inline);
		}

		return this;
	}

	/// <summary>
	///     Adds multiple inline fields from a list of tuples, automatically organizing them into rows.
	///     Each tuple contains field name and value.
	/// </summary>
	/// <param name="fields">List of field name/value tuples</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder AddInlineFields(List<Tuple<string, string>> fields) => this.AddFields(fields, true);

	/// <summary>
	///     Adds a field with position information if position sending is enabled.
	/// </summary>
	/// <param name="position">The position vector</param>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder AddPositionField(Vector3 position)
	{
		if (this._config.SendPositionsEnabled)
		{
			var formattedPos = MessageTransformer.FormatVector3AsPos(position);
			this.AddField("Coordinates", formattedPos);
		}

		return this;
	}

	/// <summary>
	///     Organizes fields to ensure they are arranged according to the configuration settings.
	///     Can be used to group and order fields before building the embed.
	/// </summary>
	/// <returns>The current EmbedBuilder instance for method chaining</returns>
	public EmbedBuilder OrganizeFields()
	{
		if (this._fields.Count == 0)
		{
			return this;
		}

		// If no field display order is defined, use the current order
		if (this._config.EmbedFieldDisplayOrder == null || this._config.EmbedFieldDisplayOrder.Count == 0)
		{
			return this;
		}

		// The field identifiers from the display order configuration are already available as a list
		var fieldOrder = this._config.EmbedFieldDisplayOrder;

		// If the order is empty, return
		if (fieldOrder.Count == 0)
		{
			return this;
		}

		// Attempt to reorder fields based on the provided identifiers
		// This is a placeholder - in a real implementation, you'd have logic to identify field types
		// and reorder them according to the configuration

		return this;
	}

	/// <summary>
	///     Builds the DiscordEmbed object with all the configured settings.
	/// </summary>
	/// <returns>A fully configured DiscordEmbed object</returns>
	public DiscordEmbed Build()
	{
		if (this._fields.Count > 0)
		{
			this._embed.fields = this._fields;
		}

		// Check for total character limit
		if (!this.IsWithinCharacterLimit())
		{
			DiscordConnectorPlugin.StaticLogger.LogWarning(
				"Embed exceeds Discord's character limit and will be truncated.");
			this.TruncateToFitCharacterLimit();
		}

		return this._embed;
	}

	/// <summary>
	///     Checks if the current embed is within Discord's character limit.
	/// </summary>
	/// <returns>True if the embed is within limits, false otherwise</returns>
	private bool IsWithinCharacterLimit()
	{
		// Calculate total character count for the embed
		var totalChars = 0;

		totalChars += this._embed.title?.Length ?? 0;
		totalChars += this._embed.description?.Length ?? 0;
		totalChars += this._embed.footer?.text?.Length ?? 0;
		totalChars += this._embed.author?.name?.Length ?? 0;

		if (this._fields != null && this._fields.Count > 0)
		{
			foreach (var field in this._fields)
			{
				totalChars += field.name?.Length ?? 0;
				totalChars += field.value?.Length ?? 0;
			}
		}

		// Discord's limit is 6000 characters
		return totalChars <= 6000;
	}

	/// <summary>
	///     Truncates embed content to fit within Discord's character limits.
	/// </summary>
	private void TruncateToFitCharacterLimit()
	{
		// Truncate the description first if it exists and is long
		if (!string.IsNullOrEmpty(this._embed.description) && this._embed.description?.Length > 1000)
		{
			this._embed.description = this._embed.description!.Substring(0, 1000) + "...";
		}

		// If there are fields, truncate their values
		if (this._fields.Count > 0)
		{
			foreach (var field in this._fields)
			{
				if (!string.IsNullOrEmpty(field.value) && field.value?.Length > 500)
				{
					field.value = field.value!.Substring(0, 500) + "...";
				}
			}
		}

		// If still too large, remove fields starting from the end
		while (this._fields.Count > 1 && !this.IsWithinCharacterLimit())
		{
			this._fields.RemoveAt(this._fields.Count - 1);
		}
	}

	/// <summary>
	///     Gets the description for use in fallback messages when embed creation fails.
	///     This provides a way to extract important content when falling back to plain text.
	/// </summary>
	/// <returns>The description string or null if not available</returns>
	public string? GetDescriptionForFallback()
	{
		try
		{
			// Return the description if available
			if (!string.IsNullOrEmpty(this._embed.description))
			{
				return this._embed.description;
			}

			// If no description, try to use title
			if (!string.IsNullOrEmpty(this._embed.title))
			{
				return this._embed.title;
			}

			// If no title, try to use author name
			if (this._embed.author != null && !string.IsNullOrEmpty(this._embed.author.name))
			{
				return this._embed.author.name!;
			}

			// If no core content, try to concatenate field values with a max length
			if (this._fields.Count > 0)
			{
				const int MAX_FIELD_LENGTH = 100;
				const int MAX_FIELDS = 3;

				List<string> fieldTexts = new();
				var fieldsToUse = Math.Min(this._fields.Count, MAX_FIELDS);

				for (var i = 0; i < fieldsToUse; i++)
				{
					if (!string.IsNullOrEmpty(this._fields[i].name) && !string.IsNullOrEmpty(this._fields[i].value))
					{
						var fieldText = $"{this._fields[i].name!}: {this._fields[i].value!}";
						if (fieldText?.Length > MAX_FIELD_LENGTH)
						{
							fieldText = fieldText!.Substring(0, MAX_FIELD_LENGTH) + "...";
						}

						// Ensure fieldText is not null before adding to the list
						if (fieldText != null)
						{
							fieldTexts.Add(fieldText);
						}
					}
				}

				if (fieldTexts.Count > 0)
				{
					return string.Join(" | ", fieldTexts);
				}
			}

			// Nothing useful found
			return null;
		}
		catch (Exception ex)
		{
			// Log but don't throw - this is already a fallback method
			DiscordConnectorPlugin.StaticLogger.LogError($"Error extracting fallback content: {ex.Message}");
			return null;
		}
	}
}
