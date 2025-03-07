using System;
using System.Collections.Generic;
using System.Linq;
using DiscordConnector.Config;
using UnityEngine;

namespace DiscordConnector;

/// <summary>
///     Builder class for creating Discord embed messages with a fluent interface.
///     Handles configuration-based field filtering, validation, and fallback handling.
///     Implements Discord's embed structure with support for multi-row field arrangements.
/// </summary>
internal class EmbedBuilder
{
    // Discord embed object being constructed
    private readonly DiscordEmbed _embed;
    
    // Field tracking for organizing fields into rows
    private readonly List<DiscordField> _fields;
    private int _currentRowFieldCount;
    private const int MAX_FIELDS_PER_ROW = 3;
    
    // Reference to the configuration for field visibility and other settings
    private readonly PluginConfig _config;
    
    /// <summary>
    ///     Initializes a new instance of the EmbedBuilder class with default settings.
    /// </summary>
    public EmbedBuilder()
    {
        _embed = new DiscordEmbed();
        _fields = new List<DiscordField>();
        _currentRowFieldCount = 0;
        _config = DiscordConnectorPlugin.StaticConfig;
    }
    
    /// <summary>
    ///     Sets the title of the embed if enabled in configuration.
    /// </summary>
    /// <param name="title">The title to set</param>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder SetTitle(string? title)
    {
        if (_config.EmbedTitleEnabled)
        {
            _embed.title = EmbedConfigValidator.ValidateTitle(title);
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
        if (_config.EmbedDescriptionEnabled)
        {
            _embed.description = EmbedConfigValidator.ValidateDescription(description);
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
        _embed.url = EmbedConfigValidator.ValidateUrl(url);
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
        if (string.IsNullOrEmpty(_config.EmbedUrlTemplate))
        {
            return this;
        }
        
        string url = _config.EmbedUrlTemplate;
        
        if (variables != null)
        {
            foreach (var pair in variables)
            {
                url = url.Replace($"{{{pair.Key}}}", pair.Value ?? string.Empty);
            }
        }
        
        return SetUrl(url);
    }
    
    /// <summary>
    ///     Sets the color of the embed sidebar using decimal color representation (0-16777215).
    /// </summary>
    /// <param name="color">The decimal color value</param>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder SetColor(int color)
    {
        // Manual implementation of clamp since Math.Clamp may not be available in this framework version
        _embed.color = Math.Max(0, Math.Min(color, 16777215));
        return this;
    }
    
    /// <summary>
    ///     Sets the color of the embed sidebar using a hex color code (e.g., "#7289DA").
    /// </summary>
    /// <param name="hexColor">The hex color code</param>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder SetColor(string hexColor)
    {
        _embed.color = EmbedConfigValidator.HexColorToDecimal(hexColor);
        return this;
    }
    
    /// <summary>
    ///     Sets the color based on the event type using configuration settings.
    /// </summary>
    /// <param name="eventType">The event type</param>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder SetColorForEvent(Webhook.Event eventType)
    {
        string hexColor = EmbedConfigValidator.DEFAULT_COLOR;
        
        // Determine appropriate color based on event type
        if (Webhook.ServerLaunchEvents.Contains(eventType) || Webhook.ServerStartEvents.Contains(eventType))
        {
            hexColor = _config.EmbedServerStartColor;
        }
        else if (Webhook.ServerStopEvents.Contains(eventType) || Webhook.ServerShutdownEvents.Contains(eventType))
        {
            hexColor = _config.EmbedServerStopColor;
        }
        else if (Webhook.ServerSaveEvents.Contains(eventType))
        {
            hexColor = _config.EmbedServerStartColor;
        }
        else if (Webhook.PlayerJoinEvents.Contains(eventType))
        {
            hexColor = _config.EmbedPlayerJoinColor;
        }
        else if (Webhook.PlayerLeaveEvents.Contains(eventType))
        {
            hexColor = _config.EmbedPlayerLeaveColor;
        }
        else if (Webhook.PlayerDeathEvents.Contains(eventType))
        {
            hexColor = _config.EmbedDeathEventColor;
        }
        else if (Webhook.PlayerShoutEvents.Contains(eventType))
        {
            hexColor = _config.EmbedShoutMessageColor;
        }
        else if (Webhook.WorldEvents.Contains(eventType))
        {
            hexColor = _config.EmbedWorldEventColor;
        }
        else
        {
            hexColor = _config.EmbedOtherEventColor;
        }
        
        return SetColor(hexColor);
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
        if (_config.EmbedAuthorEnabled)
        {
            _embed.author = new DiscordEmbedAuthor
            {
                name = EmbedConfigValidator.ValidateAuthorName(name),
                url = EmbedConfigValidator.ValidateUrl(url),
                icon_url = EmbedConfigValidator.ValidateUrl(iconUrl)
            };
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
        if (_config.EmbedThumbnailEnabled && !string.IsNullOrEmpty(url))
        {
            _embed.thumbnail = new DiscordEmbedThumbnail
            {
                url = EmbedConfigValidator.ValidateUrl(url)
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
        if (!string.IsNullOrEmpty(url))
        {
            _embed.image = new DiscordEmbedImage
            {
                url = EmbedConfigValidator.ValidateUrl(url)
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
        if (_config.EmbedFooterEnabled)
        {
            _embed.footer = new DiscordEmbedFooter
            {
                text = EmbedConfigValidator.ValidateFooterText(text),
                icon_url = EmbedConfigValidator.ValidateUrl(iconUrl)
            };
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
        if (!_config.EmbedFooterEnabled || string.IsNullOrEmpty(_config.EmbedFooterText))
        {
            return this;
        }
        
        string footerText = _config.EmbedFooterText;
        
        if (variables != null)
        {
            foreach (var pair in variables)
            {
                footerText = footerText.Replace($"{{{pair.Key}}}", pair.Value);
            }
        }
        
        return SetFooter(footerText, iconUrl);
    }
    
    /// <summary>
    ///     Sets the timestamp of the embed if enabled in configuration.
    ///     If no timestamp is provided, the current UTC time is used.
    /// </summary>
    /// <param name="timestamp">The timestamp as a DateTimeOffset (optional)</param>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder SetTimestamp(DateTimeOffset? timestamp = null)
    {
        if (_config.EmbedTimestampEnabled)
        {
            _embed.timestamp = (timestamp ?? DateTimeOffset.UtcNow).ToString("o");
        }
        return this;
    }
    
    /// <summary>
    ///     Adds a field to the embed.
    ///     Fields will be displayed in the order they are added.
    /// </summary>
    /// <param name="name">The field name</param>
    /// <param name="value">The field value</param>
    /// <param name="inline">Whether the field should display inline</param>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder AddField(string? name, string? value, bool inline = false)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(value))
        {
            return this;
        }
        
        if (_fields.Count >= EmbedConfigValidator.MAX_FIELDS_COUNT)
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning($"Cannot add more than {EmbedConfigValidator.MAX_FIELDS_COUNT} fields to a Discord embed.");
            return this;
        }
        
        _fields.Add(new DiscordField
        {
            name = EmbedConfigValidator.ValidateFieldName(name),
            value = EmbedConfigValidator.ValidateFieldValue(value),
            inline = inline
        });
        
        if (inline)
        {
            _currentRowFieldCount++;
            if (_currentRowFieldCount >= MAX_FIELDS_PER_ROW)
            {
                _currentRowFieldCount = 0;
            }
        }
        else
        {
            // Non-inline fields break the row
            _currentRowFieldCount = 0;
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
    public EmbedBuilder AddInlineField(string? name, string? value)
    {
        return AddField(name, value, true);
    }
    
    /// <summary>
    ///     Adds a formatted position field to the embed based on a Vector3 position.
    ///     This creates a field with the position formatted according to the configured format.
    /// </summary>
    /// <param name="position">The Vector3 position to format</param>
    /// <param name="inline">Whether the field should be displayed inline</param>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder AddPositionField(Vector3 position, bool inline = true)
    {
        string positionText = MessageTransformer.FormatVector3AsPos(position);
        return AddField("Position", positionText, inline);
    }
    
    /// <summary>
    ///     Forces a new row in the field layout by adding an empty non-inline field if needed.
    ///     Only adds a row break if there are already inline fields in the current row.
    /// </summary>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder AddRowBreak()
    {
        if (_currentRowFieldCount > 0)
        {
            // Add invisible field with empty string as content and no inline
            _fields.Add(new DiscordField
            {
                name = "\u200B", // Zero-width space
                value = "\u200B", // Zero-width space
                inline = false
            });
            _currentRowFieldCount = 0;
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
        
        foreach (Tuple<string, string> field in fields)
        {
            AddField(field.Item1, field.Item2, inline);
        }
        
        return this;
    }
    
    /// <summary>
    ///     Adds multiple inline fields from a list of tuples, automatically organizing them into rows.
    ///     Each tuple contains field name and value.
    /// </summary>
    /// <param name="fields">List of field name/value tuples</param>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder AddInlineFields(List<Tuple<string, string>> fields)
    {
        return AddFields(fields, true);
    }
    
    /// <summary>
    ///     Adds a field with position information if position sending is enabled.
    /// </summary>
    /// <param name="position">The position vector</param>
    /// <returns>The current EmbedBuilder instance for method chaining</returns>
    public EmbedBuilder AddPositionField(Vector3 position)
    {
        if (_config.SendPositionsEnabled)
        {
            string formattedPos = MessageTransformer.FormatVector3AsPos(position);
            AddField("Coordinates", formattedPos);
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
        if (_fields.Count == 0)
        {
            return this;
        }
        
        // If no field display order is defined, use the current order
        if (_config.EmbedFieldDisplayOrder == null || _config.EmbedFieldDisplayOrder.Count == 0)
        {
            return this;
        }
        
        // The field identifiers from the display order configuration are already available as a list
        List<string> fieldOrder = _config.EmbedFieldDisplayOrder;
        
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
    ///     Builds and returns the Discord embed object.
    ///     Applies all fields and performs final validation.
    /// </summary>
    /// <returns>The built DiscordEmbed object</returns>
    public DiscordEmbed Build()
    {
        // Set fields on the embed
        if (_fields.Count > 0)
        {
            _embed.fields = _fields;
        }
        
        // Validate the total character count
        if (_fields.Count > 0 && !IsWithinCharacterLimit())
        {
            DiscordConnectorPlugin.StaticLogger.LogWarning("Discord embed exceeds maximum character limit. Some content may be truncated.");
            TruncateToFitCharacterLimit();
        }
        
        return _embed;
    }
    
    /// <summary>
    ///     Checks if the total character count in the embed is within Discord's limits.
    /// </summary>
    /// <returns>true if within limits, false if exceeds</returns>
    private bool IsWithinCharacterLimit()
    {
        string[] fieldNames = _fields.Select(f => f.name ?? string.Empty).ToArray();
        string[] fieldValues = _fields.Select(f => f.value ?? string.Empty).ToArray();
        
        return EmbedConfigValidator.IsWithinCharacterLimit(
            _embed.title,
            _embed.description,
            _embed.footer?.text,
            _embed.author?.name,
            fieldNames,
            fieldValues
        );
    }
    
    /// <summary>
    ///     Truncates embed content to fit within Discord's character limits.
    /// </summary>
    private void TruncateToFitCharacterLimit()
    {
        // Truncate the description first if it exists and is long
        if (!string.IsNullOrEmpty(_embed.description) && _embed.description?.Length > 1000)
        {
            _embed.description = _embed.description!.Substring(0, 1000) + "...";
        }
        
        // If there are fields, truncate their values
        if (_fields.Count > 0)
        {
            foreach (DiscordField field in _fields)
            {
                if (!string.IsNullOrEmpty(field.value) && field.value?.Length > 500)
                {
                    field.value = field.value!.Substring(0, 500) + "...";
                }
            }
        }
        
        // If still too large, remove fields starting from the end
        while (_fields.Count > 1 && !IsWithinCharacterLimit())
        {
            _fields.RemoveAt(_fields.Count - 1);
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
            if (!string.IsNullOrEmpty(_embed.description))
            {
                return _embed.description;
            }
            
            // If no description, try to use title
            if (!string.IsNullOrEmpty(_embed.title))
            {
                return _embed.title;
            }
            
            // If no title, try to use author name
            if (_embed.author != null && !string.IsNullOrEmpty(_embed.author.name))
            {
                return _embed.author.name!;
            }
            
            // If no core content, try to concatenate field values with a max length
            if (_fields.Count > 0)
            {
                const int MAX_FIELD_LENGTH = 100;
                const int MAX_FIELDS = 3;
                
                var fieldTexts = new List<string>();
                int fieldsToUse = Math.Min(_fields.Count, MAX_FIELDS);
                
                for (int i = 0; i < fieldsToUse; i++)
                {
                    if (!string.IsNullOrEmpty(_fields[i].name) && !string.IsNullOrEmpty(_fields[i].value))
                    {
                        string fieldText = $"{_fields[i].name!}: {_fields[i].value!}";
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
