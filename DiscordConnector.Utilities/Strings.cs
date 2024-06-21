using System;

namespace DiscordConnector.Utilities;

public static class Strings
{
  public static string HumanReadableMs(double ms)
  {
    TimeSpan t = TimeSpan.FromMilliseconds(ms);

    if (t.Milliseconds == t.Seconds && t.Seconds == 0)
    {
      return string.Format(
      "{0:D2}h:{1:D2}m",
      t.Hours,
      t.Minutes);
    }

    return string.Format(
        "{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
        t.Hours,
        t.Minutes,
        t.Seconds,
        t.Milliseconds);
  }
}
