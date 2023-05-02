namespace DiscordConnector;
class VDCLogger
{
    private static BepInEx.Logging.ManualLogSource _logger;
    private bool _logDebugMessages = false;

    public VDCLogger(BepInEx.Logging.ManualLogSource logger)
    {
        _logger = logger;
        _logger.LogInfo("Logger initialized.");
    }

    internal void SetLogLevel(bool logDebugMessages)
    {
        _logDebugMessages = logDebugMessages;
    }

    public void LogDebug(string message)
    {
        _logger.LogDebug(message);
        if (_logDebugMessages)
        {
            _logger.LogInfo(message);
        }
    }

    public void LogInfo(string message)
    {
        _logger.LogInfo(message);
    }

    public void LogWarning(string message)
    {
        _logger.LogWarning(message);
    }

    public void LogError(string message)
    {
        _logger.LogError(message);
    }

    public void LogFatal(string message)
    {
        _logger.LogFatal(message);
    }
}
