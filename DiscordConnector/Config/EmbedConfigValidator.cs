using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DiscordConnector;

namespace DiscordConnector.Config;

/// <summary>
/// Provides validation methods for Discord embed configuration settings.
/// Ensures all values meet Discord API constraints and provides safe defaults.
/// Implements error handling and logging for configuration issues.
/// </summary>
internal class EmbedConfigValidator
{
    // Discord API constraints for embed content
    public const int MAX_TITLE_LENGTH = 256;
    public const int MAX_DESCRIPTION_LENGTH = 4096;
    public const int MAX_FIELDS_COUNT = 25;
    public const int MAX_FIELD_NAME_LENGTH = 256;
    public const int MAX_FIELD_VALUE_LENGTH = 1024;
    public const int MAX_FOOTER_TEXT_LENGTH = 2048;
    public const int MAX_AUTHOR_NAME_LENGTH = 256;
    public const int MAX_TOTAL_CHARACTERS = 6000;
    
    // Regex patterns for validation
    private static readonly Regex UrlPattern = new Regex(@"^https?://[\w-]+(\.[\w-]+)+([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?$", RegexOptions.Compiled);
    private static readonly Regex HexColorPattern = new Regex(@"^#([0-9A-Fa-f]{6})$", RegexOptions.Compiled);
    private static readonly Regex TimestampPattern = new Regex(@"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(\.\d+)?([Zz]|(\+|-)\d{2}:\d{2})?$", RegexOptions.Compiled);
    
    // Error messages
    private const string ERROR_TITLE_TOO_LONG = "Title exceeds maximum length of {0} characters and will be truncated.";
    private const string ERROR_DESCRIPTION_TOO_LONG = "Description exceeds maximum length of {0} characters and will be truncated.";
    private const string ERROR_FIELD_NAME_TOO_LONG = "Field name exceeds maximum length of {0} characters and will be truncated.";
    private const string ERROR_FIELD_VALUE_TOO_LONG = "Field value exceeds maximum length of {0} characters and will be truncated.";
    private const string ERROR_FOOTER_TOO_LONG = "Footer text exceeds maximum length of {0} characters and will be truncated.";
    private const string ERROR_AUTHOR_NAME_TOO_LONG = "Author name exceeds maximum length of {0} characters and will be truncated.";
    private const string ERROR_INVALID_URL = "Invalid URL format: {0}. URLs must start with http:// or https://";
    private const string ERROR_INVALID_HEX_COLOR = "Invalid hex color format: {0}. Using default color: {1}";
    private const string ERROR_INVALID_TIMESTAMP = "Invalid timestamp format: {0}. Must be ISO 8601 formatted (e.g. 2025-03-07T23:47:01+01:00)";
    private const string ERROR_EMPTY_EMBED = "Embed is empty - it has no visible content. Must include at least one of: title, description, fields, footer text, author name, image, or thumbnail.";

    // Default color values (in hex)
    public const string DEFAULT_COLOR = "#7289DA";          // Discord Blurple
    public const string DEFAULT_SERVER_START_COLOR = "#43B581"; // Green
    public const string DEFAULT_SERVER_STOP_COLOR = "#F04747";  // Red
    public const string DEFAULT_PLAYER_JOIN_COLOR = "#43B581";  // Green
    public const string DEFAULT_PLAYER_LEAVE_COLOR = "#FAA61A"; // Orange
    public const string DEFAULT_DEATH_EVENT_COLOR = "#F04747";  // Red
    public const string DEFAULT_SHOUT_MESSAGE_COLOR = "#7289DA"; // Discord Blurple
    public const string DEFAULT_OTHER_EVENT_COLOR = "#747F8D";  // Gray

    // Default footer text
    public const string DEFAULT_FOOTER_TEXT = "Valheim Server | {worldName}";

    /// <summary>
    /// Validates a title string against Discord API constraints.
    /// </summary>
    /// <param name="title">The title to validate</param>
    /// <returns>A valid title string, truncated if necessary</returns>
    public static string ValidateTitle(string? title)
    {
        try
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;
                
            if (title?.Length > MAX_TITLE_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_TITLE_TOO_LONG, MAX_TITLE_LENGTH));
                return title!.Substring(0, MAX_TITLE_LENGTH);
            }
            
            return title!;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error validating title: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Validates a description string against Discord API constraints.
    /// </summary>
    /// <param name="description">The description to validate</param>
    /// <returns>A valid description string, truncated if necessary</returns>
    public static string ValidateDescription(string? description)
    {
        try
        {
            if (string.IsNullOrEmpty(description))
                return string.Empty;
                
            if (description?.Length > MAX_DESCRIPTION_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_DESCRIPTION_TOO_LONG, MAX_DESCRIPTION_LENGTH));
                return description!.Substring(0, MAX_DESCRIPTION_LENGTH);
            }
            
            return description!;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error validating description: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Validates a field name against Discord API constraints.
    /// </summary>
    /// <param name="name">The field name to validate</param>
    /// <returns>A valid field name, truncated if necessary</returns>
    public static string ValidateFieldName(string? name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
                return "Field";
                
            if (name?.Length > MAX_FIELD_NAME_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_FIELD_NAME_TOO_LONG, MAX_FIELD_NAME_LENGTH));
                return name!.Substring(0, MAX_FIELD_NAME_LENGTH);
            }
            
            return name!;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error validating field name: {ex.Message}");
            return "Field";
        }
    }

    /// <summary>
    /// Validates a field value against Discord API constraints.
    /// </summary>
    /// <param name="value">The field value to validate</param>
    /// <returns>A valid field value, truncated if necessary</returns>
    public static string ValidateFieldValue(string? value)
    {
        try
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
                
            if (value?.Length > MAX_FIELD_VALUE_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_FIELD_VALUE_TOO_LONG, MAX_FIELD_VALUE_LENGTH));
                return value!.Substring(0, MAX_FIELD_VALUE_LENGTH);
            }
            
            return value!;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error validating field value: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Validates footer text against Discord API constraints.
    /// </summary>
    /// <param name="text">The footer text to validate</param>
    /// <returns>A valid footer text, truncated if necessary</returns>
    public static string ValidateFooterText(string? text)
    {
        try
        {
            if (string.IsNullOrEmpty(text))
                return DEFAULT_FOOTER_TEXT;
                
            if (text?.Length > MAX_FOOTER_TEXT_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_FOOTER_TOO_LONG, MAX_FOOTER_TEXT_LENGTH));
                return text!.Substring(0, MAX_FOOTER_TEXT_LENGTH);
            }
            
            return text!;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error validating footer text: {ex.Message}");
            return DEFAULT_FOOTER_TEXT;
        }
    }

    /// <summary>
    /// Validates an author name against Discord API constraints.
    /// </summary>
    /// <param name="name">The author name to validate</param>
    /// <returns>A valid author name, truncated if necessary</returns>
    public static string ValidateAuthorName(string? name)
    {
        try
        {
            if (string.IsNullOrEmpty(name))
                return "Valheim Server";
                
            if (name?.Length > MAX_AUTHOR_NAME_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_AUTHOR_NAME_TOO_LONG, MAX_AUTHOR_NAME_LENGTH));
                return name!.Substring(0, MAX_AUTHOR_NAME_LENGTH);
            }
            
            return name!;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error validating author name: {ex.Message}");
            return "Valheim Server";
        }
    }

    /// <summary>
    /// Validates a URL string.
    /// </summary>
    /// <param name="url">The URL to validate</param>
    /// <returns>The URL if valid, empty string otherwise</returns>
    public static string ValidateUrl(string? url)
    {
        try
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
                
            // Use regex pattern for more robust URL validation
            if (url == null || !UrlPattern.IsMatch(url))
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_INVALID_URL, url));
                return string.Empty;
            }
            
            // Double check with Uri class
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
                || (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_INVALID_URL, url));
                return string.Empty;
            }
            
            return url!;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error validating URL: {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// Validates a hex color code string.
    /// </summary>
    /// <param name="color">The color to validate</param>
    /// <param name="defaultColor">Default color to return if invalid</param>
    /// <returns>A valid hex color code</returns>
    public static string ValidateHexColor(string? color, string defaultColor = DEFAULT_COLOR)
    {
        try
        {
            if (string.IsNullOrEmpty(color))
            {
                return defaultColor;
            }
            
            // Use regex pattern for more robust hex color validation
            if (!HexColorPattern.IsMatch(color))
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_INVALID_HEX_COLOR, color, defaultColor));
                return defaultColor;
            }
            
            // Additional check if parsing fails
            if (color == null || !int.TryParse(color.TrimStart('#'), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning(string.Format(ERROR_INVALID_HEX_COLOR, color, defaultColor));
                return defaultColor;
            }
            
            return color!;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error validating hex color: {ex.Message}");
            return defaultColor;
        }
    }

    /// <summary>
    /// Converts a hex color code to the decimal value used by Discord API.
    /// </summary>
    /// <param name="hexColor">The hex color code (e.g., "#7289DA")</param>
    /// <returns>The decimal color value</returns>
    public static int HexColorToDecimal(string hexColor)
    {
        try
        {
            string validHexColor = ValidateHexColor(hexColor);
            string colorHex = validHexColor.TrimStart('#');
            
            if (int.TryParse(colorHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int colorValue))
                return colorValue;
                
            // If parsing fails, log and return default color
            DiscordConnectorPlugin.StaticLogger.LogWarning($"Failed to parse hex color {hexColor} to decimal");
            return 7506394; // #7289DA (Discord Blurple)
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error converting hex color to decimal: {ex.Message}");
            return 7506394; // #7289DA (Discord Blurple)
        }
    }

    /// <summary>
    /// Checks if the total character count in an embed exceeds Discord's limit.
    /// </summary>
    /// <param name="title">The embed title</param>
    /// <param name="description">The embed description</param>
    /// <param name="footerText">The footer text</param>
    /// <param name="authorName">The author name</param>
    /// <param name="fieldNames">Array of field names</param>
    /// <param name="fieldValues">Array of field values</param>
    /// <returns>true if within limits, false if exceeds</returns>
    public static bool IsWithinCharacterLimit(
        string? title,
        string? description,
        string? footerText,
        string? authorName,
        string[]? fieldNames,
        string[]? fieldValues)
    {
        try
        {
            int totalChars = 0;
            
            totalChars += string.IsNullOrEmpty(title) ? 0 : title!.Length;
            totalChars += string.IsNullOrEmpty(description) ? 0 : description!.Length;
            totalChars += string.IsNullOrEmpty(footerText) ? 0 : footerText!.Length;
            totalChars += string.IsNullOrEmpty(authorName) ? 0 : authorName!.Length;
            
            // Handle field arrays safely
            if (fieldNames != null && fieldValues != null)
            {
                // Check for field count limit
                if (fieldNames.Length > MAX_FIELDS_COUNT)
                {
                    DiscordConnectorPlugin.StaticLogger.LogWarning($"Embed contains {fieldNames.Length} fields, exceeding Discord's limit of {MAX_FIELDS_COUNT}. Some fields will be truncated.");
                }
                
                for (int i = 0; i < fieldNames.Length; i++)
                {
                    totalChars += string.IsNullOrEmpty(fieldNames[i]) ? 0 : fieldNames[i]!.Length;
                    
                    if (i < fieldValues.Length)
                        totalChars += string.IsNullOrEmpty(fieldValues[i]) ? 0 : fieldValues[i]!.Length;
                }
            }
            
            bool isWithinLimit = totalChars <= MAX_TOTAL_CHARACTERS;
            
            if (!isWithinLimit)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning($"Embed exceeds Discord's character limit. Total: {totalChars} chars, Max: {MAX_TOTAL_CHARACTERS} chars. Content will be truncated.");
            }
            
            return isWithinLimit;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error checking character limit: {ex.Message}");
            return false; // Assume we're over the limit if validation fails, to be safe
        }
    }
    
    /// <summary>
    /// Validates a complete Discord embed object to ensure it meets all Discord API requirements.
    /// Use this as a final check before sending an embed to Discord.
    /// </summary>
    /// <param name="embed">The Discord embed object to validate</param>
    /// <param name="validateEmptyContent">If true, will check that the embed has at least some content</param>
    /// <param name="fixIssues">If true, attempts to fix validation issues instead of failing</param>
    /// <returns>True if the embed is valid (or was fixed), false otherwise</returns>
    public static bool ValidateEmbed(DiscordEmbed embed, bool validateEmptyContent = true, bool fixIssues = false)
    {
        try
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Validating embed: fixIssues={fixIssues}, validateEmptyContent={validateEmptyContent}");
            if (embed == null)
            {
                DiscordConnectorPlugin.StaticLogger.LogError("Cannot validate a null embed");
                return false;
            }
            
            // Check if the embed has any content at all (if validation is enabled)
            if (validateEmptyContent)
            {
                bool hasContent = !string.IsNullOrEmpty(embed.title) || 
                                 !string.IsNullOrEmpty(embed.description) ||
                                 (embed.author != null && !string.IsNullOrEmpty(embed.author.name)) ||
                                 (embed.footer != null && !string.IsNullOrEmpty(embed.footer.text)) ||
                                 (embed.fields != null && embed.fields.Count > 0) ||
                                 (embed.image != null && !string.IsNullOrEmpty(embed.image.url)) ||
                                 (embed.thumbnail != null && !string.IsNullOrEmpty(embed.thumbnail.url));
                                 
                if (!hasContent)
                {
                    DiscordConnectorPlugin.StaticLogger.LogWarning(ERROR_EMPTY_EMBED);
                    if (fixIssues)
                    {
                        // Add a simple description as fallback content
                        embed.description = "No content provided";
                        DiscordConnectorPlugin.StaticLogger.LogInfo("Added fallback description to empty embed");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
            // Validate color
            if (embed.color != null)
            {
                string colorStr = embed.color?.ToString() ?? "";
                if (colorStr.StartsWith("#") && !HexColorPattern.IsMatch(colorStr))
                {
                    DiscordConnectorPlugin.StaticLogger.LogWarning($"Invalid hex color format: {colorStr}");
                    
                    if (fixIssues)
                    {
                        embed.color = int.Parse(DEFAULT_COLOR.Substring(1), System.Globalization.NumberStyles.HexNumber);
                        DiscordConnectorPlugin.StaticLogger.LogInfo($"Fixed invalid hex color by using default color: {DEFAULT_COLOR}");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
            // Validate timestamp format
            if (!string.IsNullOrEmpty(embed.timestamp))
            {
                if (!TimestampPattern.IsMatch(embed.timestamp))
                {
                    DiscordConnectorPlugin.StaticLogger.LogWarning($"Invalid timestamp format: {embed.timestamp}");
                    
                    if (fixIssues)
                    {
                        // Try to parse and reformat the timestamp
                        if (DateTime.TryParse(embed.timestamp, out DateTime parsedDate))
                        {
                            // Format in ISO 8601 format
                            embed.timestamp = parsedDate.ToString("yyyy-MM-ddTHH:mm:sszzz");
                            DiscordConnectorPlugin.StaticLogger.LogInfo($"Reformatted timestamp to ISO 8601 format: {embed.timestamp}");
                        }
                        else
                        {
                            // If parsing fails, use current time
                            embed.timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                            DiscordConnectorPlugin.StaticLogger.LogInfo($"Replaced invalid timestamp with current UTC time: {embed.timestamp}");
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
            // Validate URL fields
            bool urlsValid = true;
            
            // Validate author URL and icon URL
            if (embed.author != null)
            {
                if (!string.IsNullOrEmpty(embed.author.url))
                {
                    string validUrl = ValidateUrl(embed.author.url);
                    if (string.IsNullOrEmpty(validUrl) && !string.IsNullOrEmpty(embed.author.url))
                    {
                        DiscordConnectorPlugin.StaticLogger.LogWarning($"Author URL is invalid: {embed.author.url}");
                        if (fixIssues)
                        {
                            embed.author.url = string.Empty;
                            DiscordConnectorPlugin.StaticLogger.LogInfo("Cleared invalid author URL");
                        }
                        else
                        {
                            urlsValid = false;
                        }
                    }
                }
                
                if (!string.IsNullOrEmpty(embed.author.icon_url))
                {
                    string validIconUrl = ValidateUrl(embed.author.icon_url);
                    if (string.IsNullOrEmpty(validIconUrl) && !string.IsNullOrEmpty(embed.author.icon_url))
                    {
                        DiscordConnectorPlugin.StaticLogger.LogWarning($"Author icon URL is invalid: {embed.author.icon_url}");
                        if (fixIssues)
                        {
                            embed.author.icon_url = string.Empty;
                            DiscordConnectorPlugin.StaticLogger.LogInfo("Cleared invalid author icon URL");
                        }
                        else
                        {
                            urlsValid = false;
                        }
                    }
                }
            }
            
            // Validate footer icon URL
            if (embed.footer != null && !string.IsNullOrEmpty(embed.footer.icon_url))
            {
                string validIconUrl = ValidateUrl(embed.footer.icon_url);
                if (string.IsNullOrEmpty(validIconUrl) && !string.IsNullOrEmpty(embed.footer.icon_url))
                {
                    DiscordConnectorPlugin.StaticLogger.LogWarning($"Footer icon URL is invalid: {embed.footer.icon_url}");
                    if (fixIssues)
                    {
                        embed.footer.icon_url = string.Empty;
                        DiscordConnectorPlugin.StaticLogger.LogInfo("Cleared invalid footer icon URL");
                    }
                    else
                    {
                        urlsValid = false;
                    }
                }
            }
            
            // Validate image URL
            if (embed.image != null && !string.IsNullOrEmpty(embed.image.url))
            {
                string validImageUrl = ValidateUrl(embed.image.url);
                if (string.IsNullOrEmpty(validImageUrl) && !string.IsNullOrEmpty(embed.image.url))
                {
                    DiscordConnectorPlugin.StaticLogger.LogWarning($"Image URL is invalid: {embed.image.url}");
                    if (fixIssues)
                    {
                        embed.image.url = string.Empty;
                        DiscordConnectorPlugin.StaticLogger.LogInfo("Cleared invalid image URL");
                    }
                    else
                    {
                        urlsValid = false;
                    }
                }
            }
            
            // Validate thumbnail URL
            if (embed.thumbnail != null && !string.IsNullOrEmpty(embed.thumbnail.url))
            {
                string validThumbnailUrl = ValidateUrl(embed.thumbnail.url);
                if (string.IsNullOrEmpty(validThumbnailUrl) && !string.IsNullOrEmpty(embed.thumbnail.url))
                {
                    DiscordConnectorPlugin.StaticLogger.LogWarning($"Thumbnail URL is invalid: {embed.thumbnail.url}");
                    if (fixIssues)
                    {
                        embed.thumbnail.url = string.Empty;
                        DiscordConnectorPlugin.StaticLogger.LogInfo("Cleared invalid thumbnail URL");
                    }
                    else
                    {
                        urlsValid = false;
                    }
                }
            }
            
            if (!urlsValid)
            {
                return false;
            }
            
            // Check title length
            if (!string.IsNullOrEmpty(embed.title) && embed.title?.Length > MAX_TITLE_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning($"Embed title exceeds maximum length of {MAX_TITLE_LENGTH} characters");
                if (fixIssues)
                {
                    embed.title = embed.title!.Substring(0, MAX_TITLE_LENGTH);
                    DiscordConnectorPlugin.StaticLogger.LogInfo("Truncated embed title to maximum length");
                }
                else
                {
                    return false;
                }
            }
            
            // Check description length
            if (!string.IsNullOrEmpty(embed.description) && embed.description?.Length > MAX_DESCRIPTION_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning($"Embed description exceeds maximum length of {MAX_DESCRIPTION_LENGTH} characters");
                if (fixIssues)
                {
                    embed.description = embed.description!.Substring(0, MAX_DESCRIPTION_LENGTH);
                    DiscordConnectorPlugin.StaticLogger.LogInfo("Truncated embed description to maximum length");
                }
                else
                {
                    return false;
                }
            }
            
            // Check fields
            if (embed.fields != null)
            {
                // Check field count
                if (embed.fields.Count > MAX_FIELDS_COUNT)
                {
                    DiscordConnectorPlugin.StaticLogger.LogWarning($"Embed contains {embed.fields.Count} fields, exceeding maximum of {MAX_FIELDS_COUNT}");
                    if (fixIssues)
                    {
                        embed.fields = embed.fields.Take(MAX_FIELDS_COUNT).ToList();
                        DiscordConnectorPlugin.StaticLogger.LogInfo($"Trimmed embed fields to maximum of {MAX_FIELDS_COUNT}");
                    }
                    else
                    {
                        return false;
                    }
                }
                
                // Check each field
                foreach (var field in embed.fields)
                {
                    // Field must have a name
                    if (string.IsNullOrEmpty(field.name))
                    {
                        DiscordConnectorPlugin.StaticLogger.LogWarning("Embed contains a field with no name");
                        if (fixIssues)
                        {
                            field.name = "Unnamed Field";
                            DiscordConnectorPlugin.StaticLogger.LogInfo("Added default name to unnamed field");
                        }
                        else
                        {
                            return false;
                        }
                    }
                    
                    // Check field name length
                    if (field.name?.Length > MAX_FIELD_NAME_LENGTH)
                    {
                        DiscordConnectorPlugin.StaticLogger.LogWarning($"Field name '{field.name!.Substring(0, Math.Min(15, field.name.Length))}...' exceeds maximum length of {MAX_FIELD_NAME_LENGTH}");
                        if (fixIssues)
                        {
                            field.name = field.name!.Substring(0, MAX_FIELD_NAME_LENGTH);
                            DiscordConnectorPlugin.StaticLogger.LogInfo($"Truncated field name to maximum length of {MAX_FIELD_NAME_LENGTH} characters");
                        }
                        else
                        {
                            return false;
                        }
                    }
                    
                    // Check field value
                    if (string.IsNullOrEmpty(field.value))
                    {
                        DiscordConnectorPlugin.StaticLogger.LogWarning($"Field '{field.name}' has no value");
                        if (fixIssues)
                        {
                            field.value = "No content";
                            DiscordConnectorPlugin.StaticLogger.LogInfo($"Added default value to field '{field.name}'");
                        }
                        else
                        {
                            return false;
                        }
                    }
                    
                    // Check field value length
                    if (field.value?.Length > MAX_FIELD_VALUE_LENGTH)
                    {
                        DiscordConnectorPlugin.StaticLogger.LogWarning($"Value for field '{field.name}' exceeds maximum length of {MAX_FIELD_VALUE_LENGTH}");
                        if (fixIssues)
                        {
                            field.value = field.value!.Substring(0, MAX_FIELD_VALUE_LENGTH);
                            DiscordConnectorPlugin.StaticLogger.LogInfo($"Truncated value for field '{field.name}' to maximum length");
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            
            // Check footer text length
            if (embed.footer != null && !string.IsNullOrEmpty(embed.footer.text) && embed.footer.text?.Length > MAX_FOOTER_TEXT_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning($"Footer text exceeds maximum length of {MAX_FOOTER_TEXT_LENGTH} characters");
                if (fixIssues)
                {
                    embed.footer.text = embed.footer.text!.Substring(0, MAX_FOOTER_TEXT_LENGTH);
                    DiscordConnectorPlugin.StaticLogger.LogInfo("Truncated footer text to maximum length");
                }
                else
                {
                    return false;
                }
            }
            
            // Check author name length
            if (embed.author != null && !string.IsNullOrEmpty(embed.author.name) && embed.author.name?.Length > MAX_AUTHOR_NAME_LENGTH)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning($"Author name exceeds maximum length of {MAX_AUTHOR_NAME_LENGTH} characters");
                if (fixIssues)
                {
                    embed.author.name = embed.author.name!.Substring(0, MAX_AUTHOR_NAME_LENGTH);
                    DiscordConnectorPlugin.StaticLogger.LogInfo("Truncated author name to maximum length");
                }
                else
                {
                    return false;
                }
            }
            
            // Check total character count
            int totalChars = 0;
            
            // Add title length
            if (!string.IsNullOrEmpty(embed.title))
                totalChars += embed.title!.Length;
            
            // Add description length
            if (!string.IsNullOrEmpty(embed.description))
                totalChars += embed.description!.Length;
            
            // Add footer text length
            if (embed.footer != null && !string.IsNullOrEmpty(embed.footer.text))
                totalChars += embed.footer.text!.Length;
                
            // Add author name length
            if (embed.author != null && !string.IsNullOrEmpty(embed.author.name))
                totalChars += embed.author.name?.Length ?? 0;
                
            // Add field content lengths
            if (embed.fields != null)
            {
                foreach (var field in embed.fields)
                {
                    if (!string.IsNullOrEmpty(field.name))
                        totalChars += field.name?.Length ?? 0;
                        
                    if (!string.IsNullOrEmpty(field.value))
                        totalChars += field.value?.Length ?? 0;
                }
            }
            
            if (totalChars > MAX_TOTAL_CHARACTERS)
            {
                DiscordConnectorPlugin.StaticLogger.LogWarning($"Embed exceeds total character limit of {MAX_TOTAL_CHARACTERS} (has {totalChars} characters)");
                
                // If fixing issues, attempt to trim content to fit within limits
                if (fixIssues)
                {
                    int excessChars = totalChars - MAX_TOTAL_CHARACTERS;
                    DiscordConnectorPlugin.StaticLogger.LogInfo($"Attempting to trim {excessChars} characters from embed content");
                    
                    // First try to trim the description if it exists and is substantial
                    if (!string.IsNullOrEmpty(embed.description) && embed.description!.Length > 100)
                    {
                        int charsToTrim = Math.Min(excessChars + 3, embed.description!.Length - 50); // Leave at least 50 chars
                        if (charsToTrim > 0)
                        {
                            int originalLength = embed.description!.Length;
                            embed.description = embed.description!.Substring(0, embed.description!.Length - charsToTrim) + "...";
                            int newLength = embed.description!.Length;
                            DiscordConnectorPlugin.StaticLogger.LogInfo($"Trimmed description from {originalLength} to {newLength} characters (removed {originalLength - newLength} chars)");
                            return true; // Try the trimmed version
                        }
                    }
                    
                    // If still over limit, try removing some fields
                    if (embed.fields != null && embed.fields.Count > 0)
                    {
                        // Calculate total character count again now that we've possibly trimmed description
                        int currentTotal = 0;
                        if (!string.IsNullOrEmpty(embed.title)) currentTotal += embed.title?.Length ?? 0;
                        if (!string.IsNullOrEmpty(embed.description)) currentTotal += embed.description?.Length ?? 0;
                        if (embed.footer != null && !string.IsNullOrEmpty(embed.footer.text)) currentTotal += embed.footer.text?.Length ?? 0;
                        if (embed.author != null && !string.IsNullOrEmpty(embed.author.name)) currentTotal += embed.author.name?.Length ?? 0;
                        
                        // First try to identify and remove the largest fields
                        List<(int index, int size)> fieldSizes = new List<(int, int)>();
                        for (int i = 0; i < embed.fields.Count; i++)
                        {
                            int fieldSize = 0;
                            if (!string.IsNullOrEmpty(embed.fields[i].name)) fieldSize += embed.fields[i].name?.Length ?? 0;
                            if (!string.IsNullOrEmpty(embed.fields[i].value)) fieldSize += embed.fields[i].value?.Length ?? 0;
                            fieldSizes.Add((i, fieldSize));
                            currentTotal += fieldSize;
                        }
                        
                        // We need to remove enough characters
                        int charsToRemove = currentTotal - MAX_TOTAL_CHARACTERS;
                        if (charsToRemove > 0)
                        {
                            // Sort fields by size (largest first)
                            fieldSizes.Sort((a, b) => b.size.CompareTo(a.size));
                            
                            // Keep track of which fields to keep
                            bool[] keepField = new bool[embed.fields.Count];
                            for (int i = 0; i < keepField.Length; i++) keepField[i] = true;
                            
                            // Start removing largest fields until we're under the limit
                            int removedCount = 0;
                            int removedChars = 0;
                            foreach (var (index, size) in fieldSizes)
                            {
                                keepField[index] = false;
                                removedCount++;
                                removedChars += size;
                                
                                if (removedChars >= charsToRemove) break;
                            }
                            
                            // Create new array of fields to keep
                            List<DiscordField> fieldsToKeep = new List<DiscordField>();
                            for (int i = 0; i < embed.fields.Count; i++)
                            {
                                if (keepField[i]) fieldsToKeep.Add(embed.fields[i]);
                            }
                            
                            int originalCount = embed.fields.Count;
                            embed.fields = fieldsToKeep;
                            DiscordConnectorPlugin.StaticLogger.LogInfo($"Removed {removedCount} of {originalCount} fields to fit within character limit (removed approximately {removedChars} characters)");
                            return true; // Try the trimmed version
                        }
                    }
                    
                    // If no other solution works, trim the description more aggressively as a last resort
                    if (!string.IsNullOrEmpty(embed.description))
                    {
                        int originalLength = embed.description?.Length ?? 0;
                        embed.description = embed.description!.Substring(0, Math.Min(100, embed.description!.Length)) + "... (truncated)";
                        int newLength = embed.description?.Length ?? 0;
                        DiscordConnectorPlugin.StaticLogger.LogInfo($"Drastically trimmed description from {originalLength} to {newLength} characters as last resort (removed {originalLength - newLength} chars)");
                        return true;
                    }
                }
                
                return false;
            }
            
            DiscordConnectorPlugin.StaticLogger.LogDebug("Embed validation completed successfully");
            return true;
        }
        catch (Exception ex)
        {
            DiscordConnectorPlugin.StaticLogger.LogError($"Error validating embed: {ex.Message}");
            if (ex.StackTrace != null)
            {
                DiscordConnectorPlugin.StaticLogger.LogDebug($"Validation error stack trace: {ex.StackTrace}");
            }
            return false;
        }
    }
    
    /// <summary>
    /// Creates a simple valid embed for testing purposes
    /// </summary>
    /// <param name="title">Optional title, defaults to "Test Embed"</param>
    /// <param name="description">Optional description, defaults to "This is a test embed"</param>
    /// <returns>A valid Discord embed that can be used for testing</returns>
    public static DiscordEmbed CreateTestEmbed(string title = "Test Embed", string description = "This is a test embed")
    {
        return new DiscordEmbed
        {
            title = title,
            description = description,
            color = int.Parse(DEFAULT_COLOR.Substring(1), NumberStyles.HexNumber),
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            fields = new List<DiscordField> {
                new DiscordField { name = "Field 1", value = "Value 1", inline = true },
                new DiscordField { name = "Field 2", value = "Value 2", inline = true }
            },
            footer = new DiscordEmbedFooter { text = "Test Footer" },
            author = new DiscordEmbedAuthor { name = "Test Author" }
        };
    }
    
    /// <summary>
    /// Helper method for testing validation without affecting the original embed
    /// </summary>
    /// <param name="embed">The embed to test</param>
    /// <param name="validateEmptyContent">Whether to validate if the embed is empty</param>
    /// <param name="fixIssues">Whether to fix issues automatically</param>
    /// <returns>A tuple containing: (success, error message if any, fixed embed if requested)</returns>
    public static (bool Success, string? ErrorMessage, DiscordEmbed? FixedEmbed) TestValidation(
        DiscordEmbed embed, 
        bool validateEmptyContent = true, 
        bool fixIssues = false)
    {
        // Create a copy of the embed for testing
        var embedCopy = new DiscordEmbed
        {
            title = embed.title,
            description = embed.description,
            color = embed.color,
            timestamp = embed.timestamp,
            url = embed.url,
            footer = embed.footer != null ? new DiscordEmbedFooter { text = embed.footer.text, icon_url = embed.footer.icon_url } : null,
            author = embed.author != null ? new DiscordEmbedAuthor { name = embed.author.name, url = embed.author.url, icon_url = embed.author.icon_url } : null,
            image = embed.image != null ? new DiscordEmbedImage { url = embed.image.url } : null,
            thumbnail = embed.thumbnail != null ? new DiscordEmbedThumbnail { url = embed.thumbnail.url } : null
        };
        
        // Copy fields if present
        if (embed.fields != null && embed.fields.Count > 0)
        {
            embedCopy.fields = new List<DiscordField>();
            for (int i = 0; i < embed.fields.Count; i++)
            {
                embedCopy.fields.Add(new DiscordField
                {
                    name = embed.fields[i].name,
                    value = embed.fields[i].value,
                    inline = embed.fields[i].inline
                });
            }
        }
        
        // Capture log messages
        string? errorMessage = null;
        var originalLogger = DiscordConnectorPlugin.StaticLogger;
        var testLogger = new TestLogger();
        DiscordConnectorPlugin.StaticLogger = testLogger;
        
        
        try
        {
            bool result = ValidateEmbed(embedCopy, validateEmptyContent, fixIssues);
            errorMessage = testLogger.GetLastErrorOrWarning();
            return (result, errorMessage, fixIssues ? embedCopy : null);
        }
        finally
        {
            // Restore original logger
            DiscordConnectorPlugin.StaticLogger = originalLogger;
        }
    }
    
    /// <summary>
    /// Simple logger for testing that captures messages
    /// </summary>
    private class TestLogger : VDCLogger
    {
        private readonly List<(string level, string message)> logs = new();
        
        public TestLogger() : base(BepInEx.Logging.Logger.CreateLogSource("TestLogger"))
        {            
        }
        
        public override void LogDebug(string message) => logs.Add(("DEBUG", message));
        public override void LogInfo(string message) => logs.Add(("INFO", message));
        public override void LogWarning(string message) => logs.Add(("WARNING", message));
        public override void LogError(string message) => logs.Add(("ERROR", message));
        public override void LogFatal(string message) => logs.Add(("FATAL", message));
        
        public string GetLastErrorOrWarning()
        {
            var lastError = logs.FindLast(log => log.level == "ERROR" || log.level == "WARNING");
            return lastError.message ?? "No errors or warnings logged";
        }
        
        public IEnumerable<string> GetAllMessages()
        {
            return logs.Select(log => $"[{log.level}] {log.message}");
        }
    }
    
    /// <summary>
    /// Creates an embed with specific validation issues for testing purposes
    /// </summary>
    /// <param name="issueType">The type of validation issue to create</param>
    /// <returns>A Discord embed with the specified validation issue</returns>
    public static DiscordEmbed CreateProblemEmbed(EmbedValidationProblemType issueType)
    {
        switch (issueType)
        {
            case EmbedValidationProblemType.Empty:
                return new DiscordEmbed();
                
            case EmbedValidationProblemType.TitleTooLong:
                return new DiscordEmbed { 
                    title = new string('X', MAX_TITLE_LENGTH + 50),
                    description = "This embed has a title that is too long"
                };
                
            case EmbedValidationProblemType.DescriptionTooLong:
                return new DiscordEmbed { 
                    title = "Long Description",
                    description = new string('X', MAX_DESCRIPTION_LENGTH + 500)
                };
                
            case EmbedValidationProblemType.InvalidUrl:
                return new DiscordEmbed { 
                    title = "Invalid URL",
                    description = "This embed has an invalid URL",
                    url = "not-a-valid-url"
                };
                
            case EmbedValidationProblemType.InvalidColor:
                return new DiscordEmbed { 
                    title = "Invalid Color",
                    description = "This embed has an invalid hex color",
                    color = 0
                };
                
            case EmbedValidationProblemType.InvalidTimestamp:
                return new DiscordEmbed { 
                    title = "Invalid Timestamp",
                    description = "This embed has an invalid timestamp format",
                    timestamp = "2025-03-07T23:47:01Z"
                };
                
            case EmbedValidationProblemType.TooManyFields:
                var embed = new DiscordEmbed { 
                    title = "Too Many Fields",
                    description = "This embed has too many fields"
                };
                
                var fields = new List<DiscordField>();
                for (int i = 0; i < MAX_FIELDS_COUNT + 5; i++)
                {
                    fields.Add(new DiscordField { name = $"Field {i}", value = $"Value {i}" });
                }
                embed.fields = fields;
                return embed;
                
            case EmbedValidationProblemType.FieldNameTooLong:
                return new DiscordEmbed { 
                    title = "Long Field Name",
                    description = "This embed has a field with a name that is too long",
                    fields = new List<DiscordField> {
                        new DiscordField { 
                            name = new string('X', MAX_FIELD_NAME_LENGTH + 20), 
                            value = "This field name is too long" 
                        }
                    }
                };
                
            case EmbedValidationProblemType.FieldValueTooLong:
                return new DiscordEmbed { 
                    title = "Long Field Value",
                    description = "This embed has a field with a value that is too long",
                    fields = new List<DiscordField> {
                        new DiscordField { 
                            name = "Long Value Field", 
                            value = new string('X', MAX_FIELD_VALUE_LENGTH + 100) 
                        }
                    }
                };
                
            case EmbedValidationProblemType.FooterTooLong:
                return new DiscordEmbed { 
                    title = "Long Footer",
                    description = "This embed has a footer text that is too long",
                    footer = new DiscordEmbedFooter { 
                        text = new string('X', MAX_FOOTER_TEXT_LENGTH + 30) 
                    }
                };
                
            case EmbedValidationProblemType.AuthorNameTooLong:
                return new DiscordEmbed { 
                    title = "Long Author Name",
                    description = "This embed has an author name that is too long",
                    author = new DiscordEmbedAuthor { 
                        name = new string('X', MAX_AUTHOR_NAME_LENGTH + 40) 
                    }
                };
                
            case EmbedValidationProblemType.TotalCharsTooLong:
                var longEmbed = new DiscordEmbed { 
                    title = new string('X', MAX_TITLE_LENGTH - 10),
                    description = new string('Y', MAX_DESCRIPTION_LENGTH - 100),
                    footer = new DiscordEmbedFooter { text = new string('Z', MAX_FOOTER_TEXT_LENGTH - 20) },
                    author = new DiscordEmbedAuthor { name = new string('A', MAX_AUTHOR_NAME_LENGTH - 10) }
                };
                
                // Add enough fields to exceed the total char limit
                var longFields = new List<DiscordField>();
                for (int i = 0; i < 5; i++)
                {
                    longFields.Add(new DiscordField { 
                        name = $"Field {i}", 
                        value = new string('F', MAX_FIELD_VALUE_LENGTH / 2) 
                    });
                }
                longEmbed.fields = longFields;
                return longEmbed;
                
            default:
                return new DiscordEmbed { 
                    title = "Unknown Problem Type",
                    description = $"Unknown problem type: {issueType}"
                };
        }
    }
    
    /// <summary>
    /// Types of validation problems that can be created for testing
    /// </summary>
    public enum EmbedValidationProblemType
    {
        Empty,
        TitleTooLong,
        DescriptionTooLong,
        InvalidUrl,
        InvalidColor,
        InvalidTimestamp,
        TooManyFields,
        FieldNameTooLong,
        FieldValueTooLong,
        FooterTooLong,
        AuthorNameTooLong,
        TotalCharsTooLong
    }
    
    /// <summary>
    /// Example method to demonstrate how to use the validation tools
    /// </summary>
    /// <remarks>
    /// This method is provided as an example and for testing purposes.
    /// It demonstrates how to use the validation tools to test various embed scenarios.
    /// </remarks>
    public static void RunValidationTests()
    {
        DiscordConnectorPlugin.StaticLogger.LogInfo("===== Running Embed Validation Tests =====");
        
        // Test 1: Valid embed
        DiscordConnectorPlugin.StaticLogger.LogInfo("Test 1: Valid embed");
        var validEmbed = CreateTestEmbed();
        bool valid = ValidateEmbed(validEmbed);
        DiscordConnectorPlugin.StaticLogger.LogInfo($"Validation result: {valid}");
        
        // Test 2: Empty embed with auto-fix
        DiscordConnectorPlugin.StaticLogger.LogInfo("\nTest 2: Empty embed with auto-fix");
        var emptyEmbed = CreateProblemEmbed(EmbedValidationProblemType.Empty);
        var (success, error, fixedEmbed) = TestValidation(emptyEmbed, true, true);
        DiscordConnectorPlugin.StaticLogger.LogInfo($"Validation result: {success}, Error: {error}");
        if (fixedEmbed != null)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Fixed embed description: {fixedEmbed.description}");
        }
        
        // Test 3: Title too long with auto-fix
        DiscordConnectorPlugin.StaticLogger.LogInfo("\nTest 3: Title too long with auto-fix");
        var longTitleEmbed = CreateProblemEmbed(EmbedValidationProblemType.TitleTooLong);
        DiscordConnectorPlugin.StaticLogger.LogInfo($"Original title length: {longTitleEmbed.title?.Length ?? 0}");
        (success, error, fixedEmbed) = TestValidation(longTitleEmbed, true, true);
        DiscordConnectorPlugin.StaticLogger.LogInfo($"Validation result: {success}, Error: {error}");
        if (fixedEmbed != null)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Fixed title length: {fixedEmbed.title?.Length ?? 0}");
        }
        
        // Test 4: Invalid URL with auto-fix
        DiscordConnectorPlugin.StaticLogger.LogInfo("\nTest 4: Invalid URL with auto-fix");
        var invalidUrlEmbed = CreateProblemEmbed(EmbedValidationProblemType.InvalidUrl);
        DiscordConnectorPlugin.StaticLogger.LogInfo($"Original URL: {invalidUrlEmbed.url}");
        (success, error, fixedEmbed) = TestValidation(invalidUrlEmbed, true, true);
        DiscordConnectorPlugin.StaticLogger.LogInfo($"Validation result: {success}, Error: {error}");
        if (fixedEmbed != null)
        {
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Fixed URL: {fixedEmbed.url}");
        }
        
        // Test 5: Total characters too long with auto-fix
        DiscordConnectorPlugin.StaticLogger.LogInfo("\nTest 5: Total characters too long with auto-fix");
        var longEmbed = CreateProblemEmbed(EmbedValidationProblemType.TotalCharsTooLong);
        int totalChars = CalculateTotalCharacters(longEmbed);
        DiscordConnectorPlugin.StaticLogger.LogInfo($"Original total characters: {totalChars}");
        (success, error, fixedEmbed) = TestValidation(longEmbed, true, true);
        DiscordConnectorPlugin.StaticLogger.LogInfo($"Validation result: {success}, Error: {error}");
        if (fixedEmbed != null)
        {
            totalChars = CalculateTotalCharacters(fixedEmbed);
            DiscordConnectorPlugin.StaticLogger.LogInfo($"Fixed total characters: {totalChars}");
        }
        
        DiscordConnectorPlugin.StaticLogger.LogInfo("\n===== Embed Validation Tests Complete =====");
    }
    
    /// <summary>
    /// Helper method to calculate total characters in an embed
    /// </summary>
    private static int CalculateTotalCharacters(DiscordEmbed embed)
    {
        int total = 0;
        if (!string.IsNullOrEmpty(embed.title)) total += embed.title?.Length ?? 0;
        if (!string.IsNullOrEmpty(embed.description)) total += embed.description?.Length ?? 0;
        if (embed.footer != null && !string.IsNullOrEmpty(embed.footer.text)) total += embed.footer.text?.Length ?? 0;
        if (embed.author != null && !string.IsNullOrEmpty(embed.author.name)) total += embed.author.name?.Length ?? 0;
        
        if (embed.fields != null)
        {
            foreach (var field in embed.fields)
            {
                if (!string.IsNullOrEmpty(field.name)) total += field.name?.Length ?? 0;
                if (!string.IsNullOrEmpty(field.value)) total += field.value?.Length ?? 0;
            }
        }
        
        return total;
    }
}
