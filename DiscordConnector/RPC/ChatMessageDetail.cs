using System;

using Jotunn.Utils;

using UnityEngine;

namespace DiscordConnector.RPC;

public class ChatMessageDetail(Vector3 pos, Talker.Type type, string text)
{
    public Vector3 Pos { get; private set; } = pos;
    public Talker.Type TalkerType { get; private set; } = type;
    public string Text { get; private set; } = text;
    public static string EmptyTextMessage { get; } = "#empty#";

    private static readonly char Separator = '|';
    private static readonly int ExpectedParts = 5;

    private string EncodeSelf()
    {
        string pos = $"{Pos.x}{Separator}{Pos.y}{Separator}{Pos.z}";
        string talkerType = ((int)TalkerType).ToString();
        return $"{pos}{Separator}{talkerType}{Separator}{Text}";
    }

    private static ChatMessageDetail DecodeSelf(string encoded)
    {
        var parts = encoded.Split(Separator);
        if (parts.Length != ExpectedParts)
        {
            throw new ArgumentException(
                $"Invalid number of parts in encoded string ({parts.Length} instead of {ExpectedParts})");
        }

        if (!float.TryParse(parts[0], out float x))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Failed to parse X component: {parts[0]}");
        }

        if (!float.TryParse(parts[1], out float y))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Failed to parse Y component: {parts[1]}");
        }

        if (!float.TryParse(parts[2], out float z))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Failed to parse Z component: {parts[2]}");
        }

        var pos = new Vector3(x, y, z);

        if (!int.TryParse(parts[3], out int talkerTypeInt))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Failed to parse Talker.Type: {parts[3]}");
        }

        if (!Enum.IsDefined(typeof(Talker.Type), talkerTypeInt))
        {
            DiscordConnectorPlugin.StaticLogger.LogDebug(
                $"Parsed int for Talker.Type is not a valid value: {talkerTypeInt}");
        }

        var type = (Talker.Type)talkerTypeInt;

        return new ChatMessageDetail(pos, type, parts[4]);
    }

    public ZPackage ToZPackage()
    {
        string encoded = EncodeSelf();

        try
        {
            ZPackage pkg = new();
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Encoding ZPackage with {encoded}");

            pkg.Write(encoded);

            DiscordConnectorPlugin.StaticLogger.LogDebug($"ZPackage Write() complete. Size: {pkg.Size()}");

            return pkg;
        }
        catch (Exception e)
        {
            DiscordConnectorPlugin.StaticLogger.LogError(e.Message);
            throw new Exception($"Failed to encode ZPackage with {encoded}", e);
        }
    }

    public static ChatMessageDetail FromZPackage(ZPackage pkg)
    {
        if (pkg == null || pkg.Size() == 0)
        {
            DiscordConnectorPlugin.StaticLogger.LogError("ZPackage is null or empty");
            return new ChatMessageDetail(Vector3.zero, Talker.Type.Normal, EmptyTextMessage);
        }

        DiscordConnectorPlugin.StaticLogger.LogDebug($"Reading {pkg.Size()}B ZPackage");

        try
        {
            string encoded = pkg.ReadString();
            DiscordConnectorPlugin.StaticLogger.LogDebug($"Decoded ZPackage with {encoded}");
            return DecodeSelf(encoded);
        }
        catch (Exception e)
        {
            DiscordConnectorPlugin.StaticLogger.LogError(e.Message);
        }

        // Return a default ChatMessageDetail if we failed to read the package
        return new ChatMessageDetail(Vector3.zero, Talker.Type.Normal, EmptyTextMessage);
    }

    public override string ToString()
    {
        return
            $"ChatMessageDetail: Pos={Pos}, Type={TalkerType}, Text={Text}";
    }
}
