using System;

namespace DiscordConnector.Utilities.Network;

public static class PublicIPChecker
{

  /// <summary>
  /// Get the public IP address of the server from https://ifconfig.me/ip
  /// </summary>
  /// <returns>The public IP address of the server</returns>
  public static string GetPublicIP()
  {
    string address = string.Empty;

    try
    {
      using System.Net.WebClient client = new();
      address = client.DownloadString("https://ifconfig.me/ip");
    }
    catch (Exception e)
    {
      Console.WriteLine($"DiscordConnector.Utilities: Error getting public IP address: {e.Message}");
    }

    return address;
  }
}
