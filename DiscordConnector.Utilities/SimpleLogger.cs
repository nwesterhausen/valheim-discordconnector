using System;
using System.IO;
using System.Text;

namespace DiscordConnector.Utilities;

public class SimpleLogger
{
  private readonly string LogPath;
  private readonly StringBuilder LogBuffer = new();
  /// <summary>
  /// Time between flushes of the log buffer to the log file.
  /// </summary>
  private static readonly int FLUSH_INTERVAL = 1000;
  private readonly System.Timers.Timer flushTimer;

  public enum LogLevel
  {
    DEBUG,
    INFO,
    WARN,
    ERROR,
  }

  public SimpleLogger(string logPath)
  {
    LogPath = logPath;

    // Start the flush timer
    flushTimer = new(FLUSH_INTERVAL);
    flushTimer.Elapsed += FlushBuffer;
    flushTimer.Start();
  }

  public void Dispose()
  {
    flushTimer.Stop();
    flushTimer.Dispose();
    FlushBuffer();
  }

  private void FlushBuffer()
  {
    if (LogBuffer.Length == 0)
    {
      return;
    }

    try
    {
      File.AppendAllText(LogPath, LogBuffer.ToString());
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error writing log buffer to file: {ex.Message}");
    }

    LogBuffer.Clear();
  }

  /// <summary>
  /// Timed event to flush the log buffer to the log file.
  ///
  /// If the buffer is empty, this method does nothing.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  private void FlushBuffer(object sender, System.Timers.ElapsedEventArgs e)
  {
    FlushBuffer();
  }

  private void Log(LogLevel level, string message)
  {
    LogBuffer.AppendLine($"[{level}] {DateTime.Now}: {message}");
  }

  public void LogDebug(string message)
  {
    Log(LogLevel.DEBUG, message);
  }

  public void LogInfo(string message)
  {
    Log(LogLevel.INFO, message);
  }

  public void LogWarn(string message)
  {
    Log(LogLevel.WARN, message);
  }

  public void LogWarning(string message)
  {
    LogWarn(message);
  }

  public void LogError(string message)
  {
    Log(LogLevel.ERROR, message);
  }

}
