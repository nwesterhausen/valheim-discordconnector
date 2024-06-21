using System;
using System.IO;

namespace DiscordConnector.Utilities;

public class Hashing
{
  public static string GetMD5Checksum(string filename)
  {
    using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
    using FileStream stream = File.OpenRead(filename);
    byte[] hash = md5.ComputeHash(stream);
    return BitConverter.ToString(hash).Replace("-", "");
  }
}
