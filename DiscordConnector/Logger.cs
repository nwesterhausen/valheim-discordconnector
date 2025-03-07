using System;
using System.IO;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;

namespace DiscordConnector;

internal class VDCLogger
{
    private static ManualLogSource _logger;
    private static string _logFilePath;
    private static readonly string LOG_NAME = "vdc.log";
    private bool _logDebugMessages;

    public VDCLogger(ManualLogSource logger)
    {
        _logger = logger;
        _logFilePath = Path.Combine(Paths.ConfigPath, DiscordConnectorPlugin.LegacyConfigPath, LOG_NAME);
        InitializeLogFile();
        _logger.LogInfo("Logger initialized.");
    }

    internal void SetLogLevel(bool logDebugMessages)
    {
        _logDebugMessages = logDebugMessages;
    }

    private void InitializeLogFile()
    {
        if (File.Exists(_logFilePath))
        {
            // versions old logs, like log.1 log.2 (up to 5)
            for (int i = 5; i > 1; i--)
            {
                string oldLogFilePath = $"{_logFilePath}.{i}";
                string newLogFilePath = $"{_logFilePath}.{i - 1}";
                if (File.Exists(oldLogFilePath))
                {
                    try
                    {
                        File.Delete(oldLogFilePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error deleting old log file: {ex.Message}");
                    }
                }

                if (File.Exists(newLogFilePath))
                {
                    try
                    {
                        File.Move(newLogFilePath, oldLogFilePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error moving log file: {ex.Message}");
                    }
                }
            }

            // move current log to log.1, which gets moved if exists in the loop above
            try
            {
                File.Move(_logFilePath, $"{_logFilePath}.1");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error moving log file: {ex.Message}");
            }

            _logger.LogInfo("Existing log files versioned.");
        }
    }

    /// <summary>
    ///     Write to a log for just this plugin. This will be in "BepInEx/config/plugin-id/vdc.log".
    /// </summary>
    /// <param name="severity">The severity to include, e.g. "WARN" or "DEBUG"</param>
    /// <param name="message">The message to log</param>
    /// <returns>
    ///     /// This will attempt to write to the log file. If it fails, it will log an error to the BepInEx logger.
    ///     Nothing is returned from this method.
    /// </returns>
    private async Task LogToFileAsync(string severity, string message)
    {
        try
        {
            using (StreamWriter writer = new(_logFilePath, true))
            {
                await writer.WriteLineAsync($"{DateTime.Now} [{severity}]: {message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error writing to log file: {ex.Message}");
        }
    }


    public async Task LogDebugAsync(string message)
    {
        await LogToFileAsync("DEBUG", message);
        if (_logDebugMessages)
        {
            _logger.LogInfo(message);
        }
    }

    public async Task LogInfoAsync(string message)
    {
        await LogToFileAsync("INFO", message);
        _logger.LogInfo(message);
    }

    public async Task LogWarningAsync(string message)
    {
        await LogToFileAsync("WARNING", message);
        _logger.LogWarning(message);
    }

    public async Task LogErrorAsync(string message)
    {
        await LogToFileAsync("ERROR", message);
        _logger.LogError(message);
    }

    public async Task LogFatalAsync(string message)
    {
        await LogToFileAsync("FATAL", message);
        _logger.LogFatal(message);
    }

    public virtual void LogDebug(string message)
    {
        LogDebugAsync(message).GetAwaiter().GetResult();
    }

    public virtual void LogInfo(string message)
    {
        LogInfoAsync(message).GetAwaiter().GetResult();
    }

    public virtual void LogWarning(string message)
    {
        LogWarningAsync(message).GetAwaiter().GetResult();
    }

    public virtual void LogError(string message)
    {
        LogErrorAsync(message).GetAwaiter().GetResult();
    }

    public virtual void LogFatal(string message)
    {
        LogFatalAsync(message).GetAwaiter().GetResult();
    }
}
