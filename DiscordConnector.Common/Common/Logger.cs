using System;
using System.IO;
using System.Threading.Tasks;

using BepInEx.Logging;

namespace DiscordConnector.Common;

public sealed class VdcLogger
{
    private const string LogName = "vdc.log";
    private const int MaxLogFiles = 5;
    private static ManualLogSource s_logger = null!; // set in constructor
    private static string s_logFilePath = null!; // set in constructor
    private bool _logDebugMessages;

    public VdcLogger(ManualLogSource logger, string basePath)
    {
        s_logger = logger;
        s_logFilePath = Path.Combine(basePath, LogName);
        InitializeLogFile();
        s_logger.LogInfo("Logger initialized.");
    }

    public void SetLogLevel(bool logDebugMessages)
    {
        this._logDebugMessages = logDebugMessages;
    }

    private static void InitializeLogFile()
    {
        if (!File.Exists(s_logFilePath))
        {
            return;
        }

        // versions old logs, like log.1 log.2 (up to 5)
        for (int i = MaxLogFiles; i > 1; i--)
        {
            string olderLogFile = $"{s_logFilePath}.{i}";
            string newerLogFile = $"{s_logFilePath}.{i - 1}";
            if (File.Exists(olderLogFile))
            {
                try
                {
                    File.Delete(olderLogFile);
                }
                catch (Exception ex)
                {
                    s_logger.LogError($"Error deleting old log file: {ex.Message}");
                }
            }

            // move on early if there isn't a newer log file to move
            if (!File.Exists(newerLogFile))
            {
                continue;
            }

            try
            {
                File.Move(newerLogFile, olderLogFile);
            }
            catch (Exception ex)
            {
                s_logger.LogError($"Error moving log file: {ex.Message}");
            }
        }

        // move current log to log.1, which gets moved if exists in the loop above
        try
        {
            File.Move(s_logFilePath, $"{s_logFilePath}.1");
        }
        catch (Exception ex)
        {
            s_logger.LogError($"Error moving log file: {ex.Message}");
        }

        s_logger.LogInfo("Existing log files versioned.");
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
            using StreamWriter writer = new(s_logFilePath, true);
            await writer.WriteLineAsync($"{DateTime.Now} [{severity}]: {message}");
        }
        catch (Exception ex)
        {
            s_logger.LogError($"Error writing to log file: {ex.Message}");
        }
    }


    public async Task LogDebugAsync(string message)
    {
        await LogToFileAsync("DEBUG", message);
        if (_logDebugMessages)
        {
            s_logger.LogInfo(message);
        }
    }

    public async Task LogInfoAsync(string message)
    {
        await LogToFileAsync("INFO", message);
        s_logger.LogInfo(message);
    }

    public async Task LogWarningAsync(string message)
    {
        await LogToFileAsync("WARNING", message);
        s_logger.LogWarning(message);
    }

    public async Task LogErrorAsync(string message)
    {
        await LogToFileAsync("ERROR", message);
        s_logger.LogError(message);
    }

    public async Task LogFatalAsync(string message)
    {
        await LogToFileAsync("FATAL", message);
        s_logger.LogFatal(message);
    }

    public void LogDebug(string message)
    {
        LogDebugAsync(message).GetAwaiter().GetResult();
    }

    public void LogInfo(string message)
    {
        LogInfoAsync(message).GetAwaiter().GetResult();
    }

    public void LogWarning(string message)
    {
        LogWarningAsync(message).GetAwaiter().GetResult();
    }

    public void LogError(string message)
    {
        LogErrorAsync(message).GetAwaiter().GetResult();
    }

    public void LogFatal(string message)
    {
        LogFatalAsync(message).GetAwaiter().GetResult();
    }
}
